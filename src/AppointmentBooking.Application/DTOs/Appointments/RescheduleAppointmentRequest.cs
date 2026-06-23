namespace AppointmentBooking.Application.DTOs.Appointments;

public class RescheduleAppointmentRequest
{
    public required DateTime AppointmentDateTime { get; set; }
    public int? DurationInMinutes { get; set; }
}
