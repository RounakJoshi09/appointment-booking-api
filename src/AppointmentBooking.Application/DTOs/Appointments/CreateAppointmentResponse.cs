using AppointmentBooking.Domain.Enums;

namespace AppointmentBooking.Application.DTOs.Appointments;

public class CreateAppointmentResponse
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }
    public DateTime AppointmentDateTime { get; set; } // Stored in UTC
    public TimeSpan Duration { get; set; }
    public AppointmentStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}
