using SmartCare.Core.Entities.Appointments;
using SmartCare.Core.Entities.DoctorAvailabilitys;
using SmartCare.Services.Abstraction.DTOs.DoctorAvailability;

namespace SmartCare.Services.Helpers;

public static class TimeSlotHelper
{
    public static List<TimeSlotDto> GenerateAvailableSlots(
        DoctorAvailability baseAvailability,
        IEnumerable<DoctorAvailabilityException> exceptions,
        IEnumerable<Appointment> existingAppointments,
        DateOnly targetDate)
    {
        var slots = new List<TimeSlotDto>();

        // 1. التحقق مما إذا كان هناك استثناء للسيوم بأكمله (IsUnavailable = true وليس له وقت محدد)
        var fullDayException = exceptions.FirstOrDefault(e => e.Date == targetDate && e.IsUnavailable && e.StartTime == null);
        if (fullDayException != null)
        {
            return slots; // اليوم مغلق بالكامل بواسطة استثناء طارئ/إجازة
        }

        var slotDuration = TimeSpan.FromMinutes(baseAvailability.SlotDurationInMinutes);
        var currentStart = baseAvailability.StartTime;

        // تجميع استثناءات الساعات الجزئية لهذا اليوم
        var partialExceptions = exceptions
            .Where(e => e.Date == targetDate && e.StartTime.HasValue && e.EndTime.HasValue)
            .ToList();

        // تجميع الحجوزات النشطة التي تحجز وقتاً (نستبعد الـ Cancelled)
        var activeAppointments = existingAppointments
            .Where(a => a.AppointmentDate == targetDate && a.Status != AppointmentStatus.Cancelled)
            .ToList();

        while (currentStart.Add(slotDuration) <= baseAvailability.EndTime)
        {
            var currentEnd = currentStart.Add(slotDuration);
            bool isAvailable = true;
            string? reason = null;

            // 2. فحص التقاطع مع فترات الراحة (Breaks)
            foreach (var b in baseAvailability.Breaks)
            {
                if (IsOverlapping(currentStart, currentEnd, b.StartTime, b.EndTime))
                {
                    isAvailable = false;
                    reason = "Break Period";    
                    break;
                }
            }

            // 3. فحص التقاطع مع الاستثناءات الجزئية
            if (isAvailable)
            {
                foreach (var ex in partialExceptions)
                {
                    if (IsOverlapping(currentStart, currentEnd, ex.StartTime!.Value, ex.EndTime!.Value))
                    {
                        isAvailable = false;
                        reason = ex.Reason ?? "Doctor Unavailable";
                        break;
                    }
                }
            }

            // 4. فحص التقاطع مع الحجوزات القائمة (Appointments)
            if (isAvailable)
            {
                foreach (var app in activeAppointments)
                {
                    if (IsOverlapping(currentStart, currentEnd, app.StartTime, app.EndTime))
                    {
                        isAvailable = false;
                        reason = "Already Booked";
                        break;
                    }
                }
            }

            slots.Add(new TimeSlotDto
            {
                StartTime = currentStart,
                EndTime = currentEnd,
                IsAvailable = isAvailable,
                UnavailabilityReason = reason
            });

            currentStart = currentEnd; // الانتقال للـ Slot التالي
        }

        return slots;
    }

    private static bool IsOverlapping(TimeOnly start1, TimeOnly end1, TimeOnly start2, TimeOnly end2)
    {
        return start1 < end2 && start2 < end1;
    }
}