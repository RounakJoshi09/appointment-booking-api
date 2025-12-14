namespace AppointmentBooking.Application.DTOs.Appointments;

public class CreateAppointmentRequest
{
    public required Guid PatientId { get; set; }
    public required Guid DoctorId { get; set; }
    public required DateTime AppointmentDateTime { get; set; }
    public int? DurationInMinutes { get; set; }
}
