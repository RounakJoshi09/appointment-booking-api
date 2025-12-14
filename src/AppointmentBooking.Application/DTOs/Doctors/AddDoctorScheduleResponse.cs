namespace AppointmentBooking.Application.DTOs;

public record AddDoctorScheduleResponse(
    Guid Id,
    Guid DoctorId,
    DayOfWeek? DayOfWeek,
    DateTime? Date,
    TimeSpan? StartTime,
    TimeSpan? EndTime,
    bool IsOffDay,
    string Message
);
