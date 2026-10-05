using SmartCare.Core.Specifications.Appointments;
using SmartCare.Services.Abstraction.DTOs.Appointments;
using SmartCare.Shared.Responses;

namespace SmartCare.Services.Abstraction.Services;

public interface IAppointmentService
{
    Task<ApiResponse<AppointmentDto>> CreateAsync(CreateAppointmentDto dto, Guid patientId);
    Task<ApiResponse<AppointmentDetailsDto>> GetByIdAsync(Guid appointmentId, Guid currentUserId, string currentUserRole);
    Task<ApiResponse<PaginatedResponse<AppointmentDetailsDto>>> GetMyAppointmentsAsync(Guid patientId, AppointmentSpecParams specParams);
    Task<ApiResponse<PaginatedResponse<AppointmentDetailsDto>>> GetAllAsync(AppointmentSpecParams specParams, Guid currentUserId, string currentUserRole);
    Task<ApiResponse<AppointmentDetailsDto>> CancelAsync(Guid appointmentId, Guid currentUserId, string? cancelReason);
    Task<ApiResponse<AppointmentDetailsDto>> RescheduleAsync(Guid appointmentId, Guid patientId, RescheduleAppointmentDto dto);
    Task<ApiResponse<AppointmentDetailsDto>> ConfirmAsync(Guid appointmentId, Guid doctorUserId);
    Task<ApiResponse<AppointmentDetailsDto>> CompleteAsync(Guid appointmentId, Guid doctorUserId);
}