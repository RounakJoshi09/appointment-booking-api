using AppointmentBooking.Domain.Enums;

namespace AppointmentBooking.Application.DTOs.Appointments;

public record AppointmentResponse(
    Guid Id,
    Guid PatientId,
    string PatientName,
    string PatientEmail,
    Guid DoctorId,
    string DoctorName,
    string DoctorEmail,
    DateTime AppointmentDateTime,
    int DurationInMinutes,
    AppointmentStatus Status,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);
