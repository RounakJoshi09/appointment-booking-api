namespace AppointmentBooking.Application.DTOs.Doctors;

public record DoctorAvailabilityResponse(
    Guid DoctorId,
    DateOnly Date,
    List<TimeSlot> TimeSlots
);

public record TimeSlot(
    string StartTime,
    string EndTime
);
