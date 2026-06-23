using AppointmentBooking.Domain.Entities;

namespace AppointmentBooking.Application.Interfaces.Appointments;

public interface IAppointmentRepository
{
    Task<Appointment> CreateAppointment(Appointment appointment, CancellationToken cancellationToken);
    Task<Appointment?> GetAppointmentById(Guid appointmentId, CancellationToken cancellationToken);
    Task<Appointment> UpdateAppointment(Appointment appointment, CancellationToken cancellationToken);

    /// <summary>
    /// Atomically re-checks overlaps and inserts the appointment inside a serializable transaction.
    /// Returns null when another scheduled appointment for the doctor overlaps the requested slot.
    /// </summary>
    Task<Appointment?> CreateAppointmentIfNoOverlap(Appointment appointment, CancellationToken cancellationToken);

    /// <summary>
    /// Atomically re-checks overlaps (excluding <paramref name="excludeAppointmentId"/>) and updates the appointment.
    /// Returns null when another scheduled appointment for the doctor overlaps the requested slot.
    /// </summary>
    Task<Appointment?> UpdateAppointmentIfNoOverlap(Appointment appointment, Guid excludeAppointmentId, CancellationToken cancellationToken);

    Task<bool> HasOverlappingAppointment(Guid doctorId, DateTime appointmentDateTime, TimeSpan duration, CancellationToken cancellationToken, Guid? excludeAppointmentId = null);
    Task<List<DoctorSchedule>> GetDoctorSchedules(Guid doctorId, DateTime date, CancellationToken cancellationToken);
    Task<bool> DoctorExists(Guid doctorId, CancellationToken cancellationToken);
    Task<bool> PatientExists(Guid patientId, CancellationToken cancellationToken);
}
