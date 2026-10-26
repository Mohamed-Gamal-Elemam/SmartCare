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

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<DoctorDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDoctors([FromQuery] DoctorSpecParams specParams)
    {
        var result = await _doctorService.GetDoctorsAsync(specParams);
        return Ok(ApiResponse<PaginatedResponse<DoctorDto>>.Ok(result, "Doctors retrieved successfully."));
    }

    [HttpGet("admin")]
    [Authorize(Roles = RoleConstants.Admin)]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<DoctorDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAdminDoctors([FromQuery] DoctorSpecParams specParams)
    {
        var result = await _doctorService.GetAdminDoctorsAsync(specParams);
        return Ok(ApiResponse<PaginatedResponse<DoctorDto>>.Ok(result, "Doctors retrieved successfully for admin."));
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<DoctorDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDoctorById(Guid id)
    {
        var doctor = await _doctorService.GetDoctorByIdAsync(id);
        return Ok(ApiResponse<DoctorDto>.Ok(doctor, "Doctor retrieved successfully."));
    }

    [HttpGet("user/{userId:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<DoctorDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDoctorByUserId(Guid userId)
    {
        var doctor = await _doctorService.GetDoctorByUserIdAsync(userId);
        return Ok(ApiResponse<DoctorDto>.Ok(doctor, "Doctor retrieved successfully."));
    }

    [HttpPost]
    [Authorize(Roles = RoleConstants.Admin)]
    [ProducesResponseType(typeof(ApiResponse<DoctorDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateDoctor([FromBody] CreateDoctorDto createDoctorDto)
    {
        var createdDoctor = await _doctorService.CreateDoctorAsync(createDoctorDto);
        return CreatedAtAction(nameof(GetDoctorById), new { id = createdDoctor.Id }, ApiResponse<DoctorDto>.Ok(createdDoctor, "Doctor created successfully."));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = $"{RoleConstants.Admin},{RoleConstants.Doctor}")]
    [ProducesResponseType(typeof(ApiResponse<DoctorDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateDoctor(Guid id, [FromBody] UpdateDoctorDto updateDoctorDto)
    {
        var updatedDoctor = await _doctorService.UpdateDoctorAsync(id, updateDoctorDto);
        return Ok(ApiResponse<DoctorDto>.Ok(updatedDoctor, "Doctor profile updated successfully."));
    }

    [HttpPatch("{id:guid}/activate")]
    [Authorize(Roles = RoleConstants.Admin)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ActivateDoctor(Guid id)
    {
        var result = await _doctorService.ActivateDoctorAsync(id);
        return Ok(ApiResponse<bool>.Ok(result, "Doctor activated successfully."));
    }

    [HttpPatch("{id:guid}/deactivate")]
    [Authorize(Roles = RoleConstants.Admin)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeactivateDoctor(Guid id)
    {
        var result = await _doctorService.DeactivateDoctorAsync(id);
        return Ok(ApiResponse<bool>.Ok(result, "Doctor deactivated successfully."));
    }
}