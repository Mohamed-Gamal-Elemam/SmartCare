using System.Linq;
using SmartCare.Core.Entities.Appointments;
using SmartCare.Core.Entities.DoctorAvailabilitys;
using SmartCare.Core.Interfaces;
using SmartCare.Core.Specifications.Appointments;
using SmartCare.Core.Specifications.DoctorAvailabilitySpecifications;
using SmartCare.Services.Abstraction.DTOs.DoctorAvailability;
using SmartCare.Services.Abstraction.Services;
using SmartCare.Services.Helpers;

namespace SmartCare.Services;

public class TimeSlotService : ITimeSlotService
{
    private readonly IUnitOfWork _unitOfWork;

    public TimeSlotService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<TimeSlotDto>> GetAvailableSlotsAsync(Guid doctorId, DateOnly date)
    {
        var availabilityRepository = _unitOfWork.GetRepository<DoctorAvailability, Guid>();
        var exceptionRepository = _unitOfWork.GetRepository<DoctorAvailabilityException, Guid>();
        var appointmentRepository = _unitOfWork.GetRepository<Appointment, Guid>();

        // 1. جلب فترات دوام الدكتور في اليوم ده (ممكن تكون أكتر من فترة)
        var availabilitySpec = new DoctorAvailabilityWithBreaksSpec(doctorId, date.DayOfWeek);
        var availabilityList = await availabilityRepository.GetBySpecificationAsync(availabilitySpec);

        if (availabilityList is null || !availabilityList.Any())
        {
            return Enumerable.Empty<TimeSlotDto>();
        }

        // 2. جلب استثناءات الدكتور في اليوم ده
        var exceptionSpec = new DoctorAvailabilityExceptionSpec(doctorId, date);
        var exceptions = await exceptionRepository.GetBySpecificationAsync(exceptionSpec)
                         ?? Enumerable.Empty<DoctorAvailabilityException>();

        // 3. جلب كل مواعيد الدكتور في اليوم ده (بدون Pagination)
        var appointmentSpec = new AppointmentByDoctorAndDateSpec(doctorId, date);
        var appointments = await appointmentRepository.GetBySpecificationAsync(appointmentSpec)
                           ?? Enumerable.Empty<Appointment>();

        // 4. توليد الـ Slots
        return TimeSlotHelper.GenerateAvailableSlots(availabilityList,exceptions,appointments,date,TimeOnly.FromDateTime(DateTime.Now));
    }
}