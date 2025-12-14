namespace AppointmentBooking.Application.DTOs;

public record DoctorScheduleResponse(
    Guid Id,
    Guid DoctorId,
    DayOfWeek? DayOfWeek,
    DateTime? Date,
    TimeSpan? StartTime,
    TimeSpan? EndTime,
    bool IsOffDay
);
