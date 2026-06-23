using AppointmentBooking.Domain.Enums;

namespace AppointmentBooking.Application.DTOs.Appointments;

public class RescheduleAppointmentResponse
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }
    public DateTime AppointmentDateTime { get; set; }
    public TimeSpan Duration { get; set; }
    public AppointmentStatus Status { get; set; }
    public string Message { get; set; } = string.Empty;
}
