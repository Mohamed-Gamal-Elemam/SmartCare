using SmartCare.Services.Abstraction.DTOs.Doctors;
using SmartCare.Shared.Responses;
namespace SmartCare.Services.Abstraction.Services;

public interface IDoctorService
{
    Task<PaginatedResponse<DoctorDto>> GetDoctorsAsync(DoctorSpecParams specParams);
    Task<PaginatedResponse<DoctorDto>> GetAdminDoctorsAsync(DoctorSpecParams specParams);
    Task<DoctorDto> GetDoctorByIdAsync(Guid id);
    Task<DoctorDto> GetDoctorByUserIdAsync(Guid userId);
    Task<DoctorDto> CreateDoctorAsync(CreateDoctorDto createDoctorDto);
    Task<DoctorDto> UpdateDoctorAsync(Guid id, UpdateDoctorDto updateDoctorDto);
    Task<bool> ActivateDoctorAsync(Guid id);
    Task<bool> DeactivateDoctorAsync(Guid id);
}