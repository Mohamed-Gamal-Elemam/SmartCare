using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartCare.Services.Abstraction.DTOs.Doctors;
using SmartCare.Services.Abstraction.Services;
using SmartCare.Shared.Constants;
using SmartCare.Shared.Responses;

namespace SmartCare.API.Controllers;


public class DoctorsController : BaseApiController
{
    private readonly IDoctorService _doctorService;

    public DoctorsController(IDoctorService doctorService)
    {
        _doctorService = doctorService;
    }

    /// <summary>
    /// GET /api/doctors
    /// Public / Patient: Browse active doctors with search, specialty filter, and pagination.
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PaginatedResponse<DoctorDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDoctors([FromQuery] DoctorSpecParams specParams)
    {
        var result = await _doctorService.GetDoctorsAsync(specParams);
        return Ok(result);
    }

    /// <summary>
    /// GET /api/doctors/admin
    /// Admin Only: View all doctors (Active and Inactive) with full filtering.
    /// </summary>
    [HttpGet("admin")]
    [Authorize(Roles = RoleConstants.Admin)]
    [ProducesResponseType(typeof(PaginatedResponse<DoctorDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAdminDoctors([FromQuery] DoctorSpecParams specParams)
    {
        var result = await _doctorService.GetAdminDoctorsAsync(specParams);
        return Ok(result);
    }

    /// <summary>
    /// GET /api/doctors/{id}
    /// Public / Patient: Retrieve details of a specific doctor by Doctor ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(DoctorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDoctorById(Guid id)
    {
        var doctor = await _doctorService.GetDoctorByIdAsync(id);
        return Ok(doctor);
    }

    /// <summary>
    /// GET /api/doctors/user/{userId}
    /// Authorized: Get doctor profile using the associated Identity User Guid.
    /// </summary>
    [HttpGet("user/{userId:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(DoctorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDoctorByUserId(Guid userId)
    {
        var doctor = await _doctorService.GetDoctorByUserIdAsync(userId);
        return Ok(doctor);
    }















    /// <summary>
    /// POST /api/doctors
    /// Admin Only: Create a new doctor profile.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = RoleConstants.Admin)]
    [ProducesResponseType(typeof(DoctorDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateDoctor([FromBody] CreateDoctorDto createDoctorDto)
    {
        var createdDoctor = await _doctorService.CreateDoctorAsync(createDoctorDto);
        return CreatedAtAction(nameof(GetDoctorById), new { id = createdDoctor.Id }, createdDoctor);
    }

    /// <summary>
    /// PUT /api/doctors/{id}
    /// Admin or Doctor: Update doctor bio, experience, fee, clinic, or specialty.
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = $"{RoleConstants.Admin},{RoleConstants.Doctor}")]
    [ProducesResponseType(typeof(DoctorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateDoctor(Guid id, [FromBody] UpdateDoctorDto updateDoctorDto)
    {
        var updatedDoctor = await _doctorService.UpdateDoctorAsync(id, updateDoctorDto);
        return Ok(updatedDoctor);
    }

    /// <summary>
    /// PATCH /api/doctors/{id}/activate
    /// Admin Only: Activate a doctor profile.
    /// </summary>
    [HttpPatch("{id:guid}/activate")]
    [Authorize(Roles = RoleConstants.Admin)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ActivateDoctor(Guid id)
    {
        var result = await _doctorService.ActivateDoctorAsync(id);
        return Ok(new { success = result, message = "Doctor activated successfully." });
    }

    /// <summary>
    /// PATCH /api/doctors/{id}/deactivate
    /// Admin Only: Deactivate a doctor profile.
    /// </summary>
    [HttpPatch("{id:guid}/deactivate")]
    [Authorize(Roles = RoleConstants.Admin)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeactivateDoctor(Guid id)
    {
        var result = await _doctorService.DeactivateDoctorAsync(id);
        return Ok(new { success = result, message = "Doctor deactivated successfully." });
    }
}