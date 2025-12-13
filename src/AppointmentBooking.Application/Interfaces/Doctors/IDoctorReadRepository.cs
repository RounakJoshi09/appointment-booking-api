

using AppointmentBooking.Domain.Entities;

namespace AppointmentBooking.Application.Interfaces.Doctors;

public interface IDoctorReadRepository
{
    Task<List<Doctor>> GetAllDoctors();
}