using AutoMapper;
using SmartCare.Core.Entities.Climic;
using SmartCare.Core.Interfaces;
using SmartCare.Core.Specifications.Clinics;
using SmartCare.Services.Abstraction.DTOs.Clinic;
using SmartCare.Services.Abstraction.DTOs.Clinics;
using SmartCare.Services.Abstraction.Exceptions;
using SmartCare.Services.Abstraction.Services;
using SmartCare.Shared.Responses;

namespace SmartCare.Services.Services;

public class ClinicService : IClinicService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ClinicService( IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApiResponse<ClinicDto>> GetByIdAsync(Guid id)
    {
        var repo = _unitOfWork.GetRepository<Clinic, Guid>();
        var spec = new ClinicSpecification(id);

        var clinics = await repo.GetBySpecificationAsync(spec);
        var clinic = clinics.FirstOrDefault();

        if (clinic is null)
            throw new NotFoundException($"Clinic with ID '{id}' was not found.");

        var clinicDto = _mapper.Map<ClinicDto>(clinic);
        return ApiResponse<ClinicDto>.Ok(clinicDto);
    }

    public async Task<ApiResponse<PaginatedResponse<ClinicDto>>> GetAllActiveAsync(ClinicSpecParams specParams)
    {
        specParams.IsActive = true;
        return await GetPaginatedClinicsAsync(specParams);
    }

    public async Task<ApiResponse<PaginatedResponse<ClinicDto>>> GetAllForAdminAsync(ClinicSpecParams specParams)
    {
        return await GetPaginatedClinicsAsync(specParams);
    }

    public async Task<ApiResponse<ClinicDto>> CreateAsync(CreateClinicDto dto)
    {
        var repo = _unitOfWork.GetRepository<Clinic, Guid>();

        var isNameExists = await repo.AnyAsync(c => c.Name.ToLower() == dto.Name.Trim().ToLower());

        if (isNameExists)
            throw new ConflictException("A clinic with the same name already exists.");

        var clinic = _mapper.Map<Clinic>(dto);

        await repo.AddAsync(clinic);
        await _unitOfWork.SaveChangesAsync();

        var clinicDto = _mapper.Map<ClinicDto>(clinic);
        return ApiResponse<ClinicDto>.Ok(clinicDto, "Clinic created successfully.");
    }

    public async Task<ApiResponse<ClinicDto>> UpdateAsync(Guid id, UpdateClinicDto dto)
    {
        var repo = _unitOfWork.GetRepository<Clinic, Guid>();
        var spec = new ClinicSpecification(id);

        var clinics = await repo.GetBySpecificationAsync(spec);
        var existingClinic = clinics.FirstOrDefault();

        if (existingClinic is null)
            throw new NotFoundException($"Clinic with ID '{id}' was not found.");

        var isNameDuplicate = await repo.AnyAsync( c => c.Id != id && c.Name.ToLower() == dto.Name.Trim().ToLower());

        if (isNameDuplicate)
            throw new ConflictException("Another clinic with the same name already exists.");

        _mapper.Map(dto, existingClinic);
        existingClinic.UpdatedAt = DateTime.UtcNow;

        repo.Update(existingClinic);
        await _unitOfWork.SaveChangesAsync();

        var clinicDto = _mapper.Map<ClinicDto>(existingClinic);
        return ApiResponse<ClinicDto>.Ok(clinicDto, "Clinic updated successfully.");
    }

    public async Task<ApiResponse<bool>> ActivateAsync(Guid id)
    {
        return await SetClinicStatusAsync(id, isActive: true, "Clinic activated successfully.");
    }

    public async Task<ApiResponse<bool>> DeactivateAsync(Guid id)
    {
        return await SetClinicStatusAsync(id, isActive: false, "Clinic deactivated successfully.");
    }

    // ==========================================
    // Private Helper Methods
    // ==========================================

    private async Task<ApiResponse<PaginatedResponse<ClinicDto>>> GetPaginatedClinicsAsync(ClinicSpecParams specParams)
    {
        var repo = _unitOfWork.GetRepository<Clinic, Guid>();
        var spec = new ClinicSpecification(  specParams.Search,  specParams.IsActive,  specParams.PageNumber,  specParams.PageSize);

        var clinics = await repo.GetBySpecificationAsync(spec);
        var totalCount = await repo.CountAsync(spec);

        var clinicDtos = _mapper.Map<IReadOnlyList<ClinicDto>>(clinics);

        var paginatedResponse = new PaginatedResponse<ClinicDto>(  clinicDtos,  specParams.PageNumber,  specParams.PageSize,  totalCount);

        return ApiResponse<PaginatedResponse<ClinicDto>>.Ok(paginatedResponse);
    }

    private async Task<ApiResponse<bool>> SetClinicStatusAsync(Guid id, bool isActive, string successMessage)
    {
        var repo = _unitOfWork.GetRepository<Clinic, Guid>();
        var spec = new ClinicSpecification(id);

        var clinics = await repo.GetBySpecificationAsync(spec);
        var clinic = clinics.FirstOrDefault();

        if (clinic is null)
            throw new NotFoundException($"Clinic with ID '{id}' was not found.");

        clinic.IsActive = isActive;
        clinic.UpdatedAt = DateTime.UtcNow;

        repo.Update(clinic);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<bool>.Ok(true, successMessage);
    }
}