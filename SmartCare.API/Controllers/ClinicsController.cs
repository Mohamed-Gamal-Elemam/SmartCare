using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartCare.Core.Specifications.Clinics;
using SmartCare.Services.Abstraction.DTOs.Clinics;
using SmartCare.Services.Abstraction.Services;
using SmartCare.Shared.Responses;

namespace SmartCare.API.Controllers
{
    [Authorize]
    public class ClinicsController : BaseApiController
    {
        private readonly IClinicService _clinicService;

        public ClinicsController(IClinicService clinicService)
        {
            _clinicService = clinicService;
        }

        /// <summary>
        /// Get active clinics for Patients and Public.
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<PaginatedResponse<ClinicDto>>>> GetAll([FromQuery] ClinicSpecParams specParams)
        {
            var result = await _clinicService.GetAllActiveAsync(specParams);
            return Ok(result);
        }

        /// <summary>
        /// Get all clinics with full filters for Admin only.
        /// </summary>
        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<PaginatedResponse<ClinicDto>>>> GetAllForAdmin([FromQuery] ClinicSpecParams specParams)
        {
            var result = await _clinicService.GetAllForAdminAsync(specParams);
            return Ok(result);
        }

        /// <summary>
        /// Get clinic details by ID.
        /// </summary>
        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<ClinicDto>>> GetById(Guid id)
        {
            var result = await _clinicService.GetByIdAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// Create a new clinic (Admin only).
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<ClinicDto>>> Create([FromBody] CreateClinicDto dto)
        {
            var result = await _clinicService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result);
        }

        /// <summary>
        /// Update clinic details (Admin only).
        /// </summary>
        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<ClinicDto>>> Update(Guid id, [FromBody] UpdateClinicDto dto)
        {
            var result = await _clinicService.UpdateAsync(id, dto);
            return Ok(result);
        }

        /// <summary>
        /// Activate clinic (Admin only).
        /// </summary>
        [HttpPatch("{id:guid}/activate")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<bool>>> Activate(Guid id)
        {
            var result = await _clinicService.ActivateAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// Deactivate clinic (Admin only).
        /// </summary>
        [HttpPatch("{id:guid}/deactivate")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<bool>>> Deactivate(Guid id)
        {
            var result = await _clinicService.DeactivateAsync(id);
            return Ok(result);
        }
    }
}
