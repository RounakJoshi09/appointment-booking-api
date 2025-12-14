using AppointmentBooking.Domain.Entities;

namespace AppointmentBooking.Application.Interfaces.Appointments;

public interface IAppointmentRepository
{
    Task<Appointment> CreateAppointment(Appointment appointment, CancellationToken cancellationToken);
    Task<bool> HasOverlappingAppointment(Guid doctorId, DateTime appointmentDateTime, TimeSpan duration, CancellationToken cancellationToken);
    Task<List<DoctorSchedule>> GetDoctorSchedules(Guid doctorId, DateTime date, CancellationToken cancellationToken);
    Task<bool> DoctorExists(Guid doctorId, CancellationToken cancellationToken);
    Task<bool> PatientExists(Guid patientId, CancellationToken cancellationToken);
}
