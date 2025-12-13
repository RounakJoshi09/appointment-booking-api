

using AppointmentBooking.Domain.Entities;

namespace AppointmentBooking.Application.Interfaces.Doctors;

public interface IDoctorReadRepository
{
    Task<List<Doctor>> GetAllDoctors();
    Task<List<DoctorSchedule>> GetDoctorSchedulesByDate(Guid doctorId, DateOnly date);
}