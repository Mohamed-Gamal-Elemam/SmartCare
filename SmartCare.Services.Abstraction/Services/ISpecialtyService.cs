using SmartCare.Services.Abstraction.DTOs.Specialty;
using SmartCare.Shared.Responses;

namespace SmartCare.Services.Abstraction.Services;

public interface ISpecialtyService
{
    Task<ApiResponse<IReadOnlyList<SpecialtyResponseDto>>> GetAllAsync();

    Task<ApiResponse<SpecialtyResponseDto>> GetByIdAsync(Guid id);

    Task<ApiResponse<SpecialtyResponseDto>> CreateAsync(CreateSpecialtyDto dto);

    Task<ApiResponse<SpecialtyResponseDto>> UpdateAsync(Guid id,UpdateSpecialtyDto dto);

    Task<ApiResponse<bool>> DeactivateAsync(Guid id);
}