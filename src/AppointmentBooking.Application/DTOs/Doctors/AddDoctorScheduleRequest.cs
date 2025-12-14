namespace AppointmentBooking.Application.DTOs;

public record AddDoctorScheduleRequest(
    Guid DoctorId,
    DayOfWeek? DayOfWeek,
    DateTime? Date,
    TimeSpan? StartTime,
    TimeSpan? EndTime,
    bool IsOffDay
);
