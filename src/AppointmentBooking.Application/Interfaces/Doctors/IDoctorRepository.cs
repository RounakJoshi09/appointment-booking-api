using AppointmentBooking.Domain.Entities;

namespace AppointmentBooking.Application.Interfaces.Doctors;

public interface IDoctorRepository
{
    Task<bool> CreateDoctor(Doctor doctor, CancellationToken cancellationToken);
}