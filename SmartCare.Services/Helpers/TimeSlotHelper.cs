using System;
using System.Collections.Generic;
using System.Linq;
using SmartCare.Core.Entities.Appointments;
using SmartCare.Core.Entities.DoctorAvailabilitys;
using SmartCare.Services.Abstraction.DTOs.DoctorAvailability;

namespace SmartCare.Services.Helpers;

public static class TimeSlotHelper
{
    public static List<TimeSlotDto> GenerateAvailableSlots(
        IEnumerable<DoctorAvailability> baseAvailabilities,
        IEnumerable<DoctorAvailabilityException> exceptions,
        IEnumerable<Appointment> existingAppointments,
        DateOnly targetDate,
        TimeOnly? nowTime = null)
    {
        var slots = new List<TimeSlotDto>();

        var availabilities = baseAvailabilities?
            .Where(a => a.IsActive && a.DayOfWeek == targetDate.DayOfWeek)
            .ToList() ?? new List<DoctorAvailability>();

        if (availabilities.Count == 0)
            return slots;

        var exceptionList = exceptions?.ToList() ?? new List<DoctorAvailabilityException>();

        // 1. استثناءات اليوم
        var dayExceptions = exceptionList
            .Where(e => e.Date == targetDate && e.IsUnavailable)
            .ToList();

        // استثناء يوم كامل (عندما يكون StartTime غير محدد)
        if (dayExceptions.Any(e => !e.StartTime.HasValue))
            return slots;

        // استثناءات جزئية
        var partialExceptions = dayExceptions
            .Where(e => e.StartTime.HasValue && e.EndTime.HasValue)
            .ToList();

        // 2. المواعيد النشطة
        var activeAppointments = existingAppointments?
            .Where(a => a.AppointmentDate == targetDate
                     && a.Status != AppointmentStatus.Cancelled)
            .ToList() ?? new List<Appointment>();

        bool isToday = targetDate == DateOnly.FromDateTime(DateTime.Now);

        // 3. التكرار على فترات الدوام
        foreach (var availability in availabilities)
        {
            if (availability.SlotDurationInMinutes <= 0)
                continue;

            // تجاهل الفترة لو تغطيها استثناءات كاملة
            var fullyCovered = partialExceptions.Any(e =>
                e.StartTime!.Value <= availability.StartTime &&
                e.EndTime!.Value >= availability.EndTime);

            if (fullyCovered)
                continue;

            var slotDuration = TimeSpan.FromMinutes(availability.SlotDurationInMinutes);
            var currentStart = availability.StartTime;

            while (currentStart < availability.EndTime)
            {
                var currentEnd = currentStart.Add(slotDuration);

                // تجنب الـ Infinite Loop في حالة الخروج عن نطاق اليوم (Midnight Wrap)
                if (currentEnd <= currentStart && currentEnd != TimeOnly.MinValue)
                    break;

                if (currentEnd > availability.EndTime && currentEnd != TimeOnly.MinValue)
                    break;

                bool isAvailable = true;
                string? reason = null;

                // 3.1 فحص الوقت الحالي لو تاريخ اليوم
                if (nowTime.HasValue && isToday && currentStart <= nowTime.Value)
                {
                    isAvailable = false;
                    reason = "Past Time";
                }

                // 3.2 فحص فترات الراحة
                if (isAvailable && availability.Breaks != null)
                {
                    foreach (var b in availability.Breaks)
                    {
                        if (IsOverlapping(currentStart, currentEnd, b.StartTime, b.EndTime))
                        {
                            isAvailable = false;
                            reason = "Break Period";
                            break;
                        }
                    }
                }

                // 3.3 فحص الاستثناءات الجزئية
                if (isAvailable)
                {
                    foreach (var ex in partialExceptions)
                    {
                        if (IsOverlapping(currentStart, currentEnd, ex.StartTime!.Value, ex.EndTime!.Value))
                        {
                            isAvailable = false;
                            reason = string.IsNullOrWhiteSpace(ex.Reason) ? "Doctor Unavailable" : ex.Reason;
                            break;
                        }
                    }
                }

                // 3.4 فحص التقاطع مع المواعيد القائمة
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

                currentStart = currentEnd;
            }
        }

        // إزالة التكرار مع الحفاظ على الأولوية للـ Slot المتاح إن وجد
        return slots
            .GroupBy(s => s.StartTime)
            .Select(g => g.OrderByDescending(s => s.IsAvailable).First())
            .OrderBy(s => s.StartTime)
            .ToList();
    }

    private static bool IsOverlapping(TimeOnly start1, TimeOnly end1, TimeOnly start2, TimeOnly end2)
    {
        return start1 < end2 && start2 < end1;
    }
}