using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartCare.Services.Abstraction.DTOs.DoctorAvailability;
using SmartCare.Services.Abstraction.Exceptions;
using SmartCare.Services.Abstraction.Services;
using SmartCare.Shared.Constants;

namespace SmartCare.API.Controllers;

public class DoctorAvailabilityController : BaseApiController
{
    private readonly IDoctorAvailabilityService _availabilityService;

    public DoctorAvailabilityController(IDoctorAvailabilityService availabilityService)
    {
        _availabilityService = availabilityService;
    }

    // =========================================================
    // GET AVAILABILITIES
    // =========================================================

    /// <summary>
    /// Get all weekly working schedules and breaks for a specific doctor.
    /// </summary>
    [HttpGet("doctors/{doctorId:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetDoctorAvailabilities(Guid doctorId)
    {
        var response = await _availabilityService.GetDoctorAvailabilitiesAsync(doctorId);
        return Ok(response);
    }

    // =========================================================
    // GET EXCEPTIONS
    // =========================================================

    /// <summary>
    /// Get all exceptions for a specific doctor.
    /// </summary>
    [HttpGet("doctors/{doctorId:guid}/exceptions")]
    [AllowAnonymous]
    public async Task<IActionResult> GetDoctorExceptions(Guid doctorId)
    {
        var response = await _availabilityService.GetDoctorExceptionsAsync(doctorId);
        return Ok(response);
    }

    // =========================================================
    // CREATE AVAILABILITY
    // =========================================================

    /// <summary>
    /// Create a new weekly working schedule.
    /// </summary>
    [HttpPost("schedule")]
    [Authorize(Roles = RoleConstants.Doctor)]
    public async Task<IActionResult> CreateAvailability([FromBody] CreateDoctorAvailabilityDto dto)
    {
        var doctorId = GetDoctorIdFromClaims();
        var response = await _availabilityService.CreateAvailabilityAsync(doctorId, dto);

        return Ok(response);
    }

    // =========================================================
    // UPDATE AVAILABILITY
    // =========================================================

    /// <summary>
    /// Update an existing weekly working schedule.
    /// </summary>
    [HttpPut("schedule/{availabilityId:guid}")]
    [Authorize(Roles = RoleConstants.Doctor)]
    public async Task<IActionResult> UpdateAvailability(Guid availabilityId, [FromBody] UpdateDoctorAvailabilityDto dto)
    {
        var doctorId = GetDoctorIdFromClaims();
        var response = await _availabilityService.UpdateAvailabilityAsync(doctorId, availabilityId, dto);

        return Ok(response);
    }

    // =========================================================
    // ADD EXCEPTION
    // =========================================================

    /// <summary>
    /// Add an exception date such as vacation, day off, or modified working hours.
    /// </summary>
    [HttpPost("exceptions")]
    [Authorize(Roles = RoleConstants.Doctor)]
    public async Task<IActionResult> AddException([FromBody] CreateDoctorAvailabilityExceptionDto dto)
    {
        var doctorId = GetDoctorIdFromClaims();
        var response = await _availabilityService.AddExceptionAsync(doctorId, dto);

        return Ok(response);
    }

    // =========================================================
    // DELETE AVAILABILITY
    // =========================================================

    /// <summary>
    /// Delete a specific availability schedule.
    /// </summary>
    [HttpDelete("{availabilityId:guid}")]
    [Authorize(Roles = RoleConstants.Doctor)]
    public async Task<IActionResult> DeleteAvailability(Guid availabilityId)
    {
        var doctorId = GetDoctorIdFromClaims();
        var response = await _availabilityService.DeleteAvailabilityAsync(availabilityId, doctorId);

        return Ok(response);
    }

    // =========================================================
    // DELETE EXCEPTION
    // =========================================================

    /// <summary>
    /// Delete a specific availability exception.
    /// </summary>
    [HttpDelete("exceptions/{exceptionId:guid}")]
    [Authorize(Roles = RoleConstants.Doctor)]
    public async Task<IActionResult> DeleteException(Guid exceptionId)
    {
        var doctorId = GetDoctorIdFromClaims();
        var response = await _availabilityService.DeleteExceptionAsync(exceptionId, doctorId);

        return Ok(response);
    }

    // =========================================================
    // DOCTOR CLAIM
    // =========================================================

    private Guid GetDoctorIdFromClaims()
    {
        var doctorIdClaim = User.FindFirst("DoctorId")?.Value;

        if (string.IsNullOrEmpty(doctorIdClaim) || !Guid.TryParse(doctorIdClaim, out var doctorId))
        {
            throw new BadRequestException("Invalid or missing Doctor Identity in token.");
        }

        return doctorId;
    }
}