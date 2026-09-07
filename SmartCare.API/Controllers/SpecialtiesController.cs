using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartCare.Services.Abstraction.DTOs.Specialty;
using SmartCare.Services.Abstraction.Services;
using SmartCare.Shared.Constants;

namespace SmartCare.API.Controllers;


public class SpecialtiesController : BaseApiController
{
    private readonly ISpecialtyService _specialtyService;

    public SpecialtiesController(ISpecialtyService specialtyService)
    {
        _specialtyService = specialtyService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult> GetAll()
    {
        var response = await _specialtyService.GetAllAsync();

        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult> GetById(Guid id)
    {
        var response = await _specialtyService.GetByIdAsync(id);

        return Ok(response);
    }

    [HttpPost]
    [Authorize(Roles = RoleConstants.Admin)]
    public async Task<ActionResult> Create(CreateSpecialtyDto dto)
    {
        var response = await _specialtyService.CreateAsync(dto);

        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = RoleConstants.Admin)]
    public async Task<ActionResult> Update(Guid id, UpdateSpecialtyDto dto)
    {
        var response = await _specialtyService.UpdateAsync(id, dto);

        return Ok(response);
    }

    [HttpPut("{id:guid}/deactivate")]
    [Authorize(Roles = RoleConstants.Admin)]
    public async Task<ActionResult> Deactivate(Guid id)
    {
        var response = await _specialtyService.DeactivateAsync(id);

        return Ok(response);
    }
}