using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartCare.Services.Abstraction.DTOs.DoctorAvailability;
using SmartCare.Services.Abstraction.Exceptions;
using SmartCare.Services.Abstraction.Services;
using SmartCare.Shared.Constants;

namespace SmartCare.API.Controllers
{
    public class DoctorAvailabilityController : BaseApiController
    {
        private readonly IDoctorAvailabilityService _availabilityService;

        public DoctorAvailabilityController(IDoctorAvailabilityService availabilityService)
        {
            _availabilityService = availabilityService;
        }

        /// <summary>
        /// Get all weekly working schedules and breaks for a specific doctor.
        /// </summary>
        [HttpGet("doctors/{doctorId:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetDoctorAvailabilities(Guid doctorId)
        {
            var result = await _availabilityService.GetDoctorAvailabilitiesAsync(doctorId);
            return Ok(result);
        }

        /// <summary>
        /// Get all exceptions (day offs / modified hours) for a specific doctor.
        /// </summary>
        [HttpGet("doctors/{doctorId:guid}/exceptions")]
        [AllowAnonymous]
        public async Task<IActionResult> GetDoctorExceptions(Guid doctorId)
        {
            var result = await _availabilityService.GetDoctorExceptionsAsync(doctorId);
            return Ok(result);
        }

        /// <summary>
        /// Create or Update weekly working schedule & breaks (Doctor only).
        /// </summary>
        [HttpPost("schedule")]
        [Authorize(Roles = RoleConstants.Doctor)]
        public async Task<IActionResult> SetAvailability([FromBody] CreateDoctorAvailabilityDto dto)
        {
            var doctorId = GetDoctorIdFromClaims();

            var result = await _availabilityService.SetAvailabilityAsync(doctorId, dto);
            return Ok(result);
        }

        /// <summary>
        /// Add an exception date (e.g. Day off, Vacation, Custom hours) (Doctor only).
        /// </summary>
        [HttpPost("exceptions")]
        [Authorize(Roles = RoleConstants.Doctor)]
        public async Task<IActionResult> AddException([FromBody] CreateDoctorAvailabilityExceptionDto dto)
        {
            var doctorId = GetDoctorIdFromClaims();

            var result = await _availabilityService.AddExceptionAsync(doctorId, dto);
            return Ok(result);
        }

        /// <summary>
        /// Delete a specific availability schedule (Doctor only).
        /// </summary>
        [HttpDelete("{availabilityId:guid}")]
        [Authorize(Roles = RoleConstants.Doctor)]
        public async Task<IActionResult> DeleteAvailability(Guid availabilityId)
        {
            var doctorId = GetDoctorIdFromClaims();

            var success = await _availabilityService.DeleteAvailabilityAsync(availabilityId, doctorId);
            if (!success)
                throw new NotFoundException("Availability schedule not found or unauthorized.");

            return NoContent();
        }

        /// <summary>
        /// Delete a specific exception (Doctor only).
        /// </summary>
        [HttpDelete("exceptions/{exceptionId:guid}")]
        [Authorize(Roles = RoleConstants.Doctor)]
        public async Task<IActionResult> DeleteException(Guid exceptionId)
        {
            var doctorId = GetDoctorIdFromClaims();

            var success = await _availabilityService.DeleteExceptionAsync(exceptionId, doctorId);
            if (!success)
                throw new NotFoundException("Exception record not found or unauthorized.");

            return NoContent();
        }

        private Guid GetDoctorIdFromClaims()
        {
            var doctorIdClaim = User.FindFirst("DoctorId")?.Value;

            if (string.IsNullOrEmpty(doctorIdClaim) || !Guid.TryParse(doctorIdClaim, out var doctorId))
                throw new BadRequestException("Invalid or missing Doctor Identity in token.");

            return doctorId;
        }
    }
}