using AutoMapper;
using SmartCare.Core.Entities;
using SmartCare.Core.Interfaces;
using SmartCare.Core.Interfaces.Repositories;
using SmartCare.Services.Abstraction.DTOs.Specialty;
using SmartCare.Services.Abstraction.Exceptions;
using SmartCare.Services.Abstraction.Services;
using SmartCare.Shared.Responses;

namespace SmartCare.Services.Services;

public class SpecialtyService : ISpecialtyService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IGenericRepository<Specialty, Guid> _specialtyRepository;

    public SpecialtyService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _specialtyRepository = _unitOfWork.GetRepository<Specialty, Guid>();
    }

    public async Task<ApiResponse<IReadOnlyList<SpecialtyResponseDto>>> GetAllAsync()
    {
        var specialties = await _specialtyRepository.GetAllAsync(s => s.IsActive);
        var result = _mapper.Map<IReadOnlyList<SpecialtyResponseDto>>(specialties);

        return ApiResponse<IReadOnlyList<SpecialtyResponseDto>>.Ok(result, "Specialties retrieved successfully.");
    }

    public async Task<ApiResponse<SpecialtyResponseDto>> GetByIdAsync(Guid id)
    {
        var specialty = await _specialtyRepository.GetByIdAsync(id);

        if (specialty is null || !specialty.IsActive)
            throw new NotFoundException("Specialty not found.");

        var result = _mapper.Map<SpecialtyResponseDto>(specialty);

        return ApiResponse<SpecialtyResponseDto>.Ok(result, "Specialty retrieved successfully.");
    }

    public async Task<ApiResponse<SpecialtyResponseDto>> CreateAsync(CreateSpecialtyDto dto)
    {
        var name = dto.Name.Trim();

        var exists = await _specialtyRepository.AnyAsync(s => s.Name == name);

        if (exists)
            throw new ConflictException("A specialty with the same name already exists.");

        var specialty = _mapper.Map<Specialty>(dto);

        specialty.Id = Guid.NewGuid();
        specialty.Name = name;
        specialty.IsActive = true;
        specialty.CreatedAt = DateTime.UtcNow;

        await _specialtyRepository.AddAsync(specialty);
        await _unitOfWork.SaveChangesAsync();

        var result = _mapper.Map<SpecialtyResponseDto>(specialty);

        return ApiResponse<SpecialtyResponseDto>.Ok(result, "Specialty created successfully.");
    }

    public async Task<ApiResponse<SpecialtyResponseDto>> UpdateAsync(Guid id, UpdateSpecialtyDto dto)
    {
        var specialty = await _specialtyRepository.GetByIdAsync(id);

        if (specialty is null || !specialty.IsActive)
            throw new NotFoundException("Specialty not found.");

        var name = dto.Name.Trim();

        var duplicateExists = await _specialtyRepository.AnyAsync(s => s.Id != id && s.Name == name);

        if (duplicateExists)
            throw new ConflictException("A specialty with the same name already exists.");

        specialty.Name = name;
        specialty.Description = dto.Description?.Trim();
        specialty.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync();

        var result = _mapper.Map<SpecialtyResponseDto>(specialty);

        return ApiResponse<SpecialtyResponseDto>.Ok(result, "Specialty updated successfully.");
    }

    public async Task<ApiResponse<bool>> DeactivateAsync(Guid id)
    {
        var specialty = await _specialtyRepository.GetByIdAsync(id);

        if (specialty is null || !specialty.IsActive)
            throw new NotFoundException("Specialty not found.");

        specialty.IsActive = false;
        specialty.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<bool>.Ok(true, "Specialty deactivated successfully.");
    }
}