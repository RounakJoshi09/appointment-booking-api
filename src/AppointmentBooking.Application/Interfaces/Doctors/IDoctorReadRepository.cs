

using AppointmentBooking.Domain.Entities;

namespace AppointmentBooking.Application.Interfaces.Doctors;

public interface IDoctorReadRepository
{
    Task<List<Doctor>> GetAllDoctors();
    Task<List<DoctorSchedule>> GetDoctorSchedulesByDate(Guid doctorId, DateOnly date);
    Task<List<AvailabilitySlot>> GetDoctorsAvailabilityByDate(Guid doctorId, DateOnly date);
}

public record AvailabilitySlot(TimeSpan StartTime, TimeSpan EndTime);