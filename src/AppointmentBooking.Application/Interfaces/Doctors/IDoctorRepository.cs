using AppointmentBooking.Domain.Entities;

namespace AppointmentBooking.Application.Interfaces.Doctors;

public interface IDoctorRepository
{
    Task<bool> CreateDoctor(Doctor doctor, CancellationToken cancellationToken);
    Task<DoctorSchedule?> AddDoctorSchedule(DoctorSchedule schedule, CancellationToken cancellationToken);
    Task<bool> DoctorExists(Guid doctorId, CancellationToken cancellationToken);
}