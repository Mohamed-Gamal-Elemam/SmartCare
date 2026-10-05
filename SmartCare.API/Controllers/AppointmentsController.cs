
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartCare.Core.Specifications.Appointments;
using SmartCare.Services.Abstraction.DTOs.Appointments;
using SmartCare.Services.Abstraction.Services;
using SmartCare.Shared.Constants;

namespace SmartCare.API.Controllers;


[Authorize]
public class AppointmentsController : BaseApiController
{
    private readonly IAppointmentService _appointmentService;

    public AppointmentsController( IAppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
    }

    [HttpPost]
    [Authorize(Roles = RoleConstants.Patient)]
    public async Task<IActionResult> Create( [FromBody] CreateAppointmentDto dto)
    {
        var patientId = GetCurrentUserId();

        var response = await _appointmentService.CreateAsync( dto,patientId);

        return Ok(response);
    }

    [HttpGet("my")]
    [Authorize(Roles = RoleConstants.Patient)]
    public async Task<IActionResult> GetMyAppointments( [FromQuery] AppointmentSpecParams specParams)
    {
        var patientId = GetCurrentUserId();

        var response = await _appointmentService.GetMyAppointmentsAsync( patientId, specParams);

        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var userId = GetCurrentUserId();
        var role = GetCurrentUserRole();

        var response = await _appointmentService.GetByIdAsync( id, userId, role);

        return Ok(response);
    }

    [HttpGet]
    [Authorize(Roles = $"{RoleConstants.Admin},{RoleConstants.Doctor}")]
    public async Task<IActionResult> GetAll( [FromQuery] AppointmentSpecParams specParams)
    {
        var userId = GetCurrentUserId();
        var role = GetCurrentUserRole();

        var response = await _appointmentService.GetAllAsync( specParams, userId, role);

        return Ok(response);
    }

    [HttpPut("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel( Guid id, [FromBody] CancelAppointmentDto dto)
    {
        var userId = GetCurrentUserId();

        var response = await _appointmentService.CancelAsync(  id,  userId,  dto.CancelReason);

        return Ok(response);
    }

    [HttpPut("{id:guid}/reschedule")]
    [Authorize(Roles = "Patient")]
    public async Task<IActionResult> Reschedule( Guid id, [FromBody] RescheduleAppointmentDto dto)
    {
        var patientId = GetCurrentUserId();

        var response = await _appointmentService.RescheduleAsync( id, patientId, dto);

        return Ok(response);
    }

    [HttpPut("{id:guid}/confirm")]
    [Authorize(Roles = RoleConstants.Doctor)]
    public async Task<IActionResult> Confirm(Guid id)
    {
        var doctorUserId = GetCurrentUserId();

        var response =  await _appointmentService.ConfirmAsync( id, doctorUserId);

        return Ok(response);
    }

    [HttpPut("{id:guid}/complete")]
    [Authorize(Roles = RoleConstants.Doctor)]
    public async Task<IActionResult> Complete(Guid id)
    {
        var doctorUserId = GetCurrentUserId();

        var response =
            await _appointmentService.CompleteAsync(id,doctorUserId);

        return Ok(response);
    }

    private Guid GetCurrentUserId()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userId, out var id))
            throw new UnauthorizedAccessException( "Invalid user identity.");

        return id;
    }

    private string GetCurrentUserRole()
    {
        return User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
    }
}

