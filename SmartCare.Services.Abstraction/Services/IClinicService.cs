using SmartCare.Services.Abstraction.DTOs.Clinic;
using SmartCare.Services.Abstraction.DTOs.Clinics;
using SmartCare.Shared.Responses;

namespace SmartCare.Services.Abstraction.Services;

public interface IClinicService
{
    Task<ApiResponse<ClinicDto>> GetByIdAsync(Guid id);

    Task<ApiResponse<PaginatedResponse<ClinicDto>>> GetAllActiveAsync(ClinicSpecParams specParams);

    Task<ApiResponse<PaginatedResponse<ClinicDto>>> GetAllForAdminAsync(ClinicSpecParams specParams);

    Task<ApiResponse<ClinicDto>> CreateAsync(CreateClinicDto dto);

    Task<ApiResponse<ClinicDto>> UpdateAsync( Guid id, UpdateClinicDto dto);

    Task<ApiResponse<bool>> ActivateAsync(Guid id);

    Task<ApiResponse<bool>> DeactivateAsync(Guid id);
}