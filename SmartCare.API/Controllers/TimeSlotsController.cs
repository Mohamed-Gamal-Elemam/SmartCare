using Microsoft.AspNetCore.Mvc;
using SmartCare.Services.Abstraction;
using SmartCare.Services.Abstraction.Services;

namespace SmartCare.API.Controllers;

[ApiController]
[Route("api/doctors/{doctorId}/time-slots")]
public class TimeSlotsController : ControllerBase
{
    private readonly ITimeSlotService _timeSlotService;

    public TimeSlotsController(ITimeSlotService timeSlotService)
    {
        _timeSlotService = timeSlotService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAvailableSlots( Guid doctorId, [FromQuery] DateOnly date)
    {
        var slots = await _timeSlotService.GetAvailableSlotsAsync( doctorId, date);

        return Ok(slots);
    }
}