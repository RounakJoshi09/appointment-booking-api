namespace AppointmentBooking.Infrastructure.Database.Doctors;
using AppointmentBooking.Application.Interfaces.Doctors;
using AppointmentBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;

public class DoctorReadRepository : IDoctorReadRepository
{
    private readonly ReadEntityContext _context;
    public DoctorReadRepository(ReadEntityContext context)
    {
        _context = context;
    }
    public async Task<List<Doctor>> GetAllDoctors()
    {
        return await _context.Doctors.ToListAsync();
    }

    public async Task<List<DoctorSchedule>> GetDoctorSchedulesByDate(Guid doctorId, DateOnly date)
    {
        var dateTime = date.ToDateTime(TimeOnly.MinValue);
        var dayOfWeek = (int)date.DayOfWeek;

        var dateSpecificSchedules = await _context.DoctorSchedules
            .Where(schedule =>
                schedule.DoctorId == doctorId &&
                schedule.Date.HasValue &&
                schedule.Date.Value.Date == dateTime.Date &&
                !schedule.IsOffDay &&
                schedule.StartTime.HasValue &&
                schedule.EndTime.HasValue)
            .ToListAsync();

        if (dateSpecificSchedules.Count > 0)
        {
            return dateSpecificSchedules;
        }

        return await _context.DoctorSchedules
            .Where(schedule =>
                schedule.DoctorId == doctorId &&
                schedule.DayOfWeek == (DayOfWeek)dayOfWeek &&
                !schedule.IsOffDay &&
                schedule.StartTime.HasValue &&
                schedule.EndTime.HasValue)
            .ToListAsync();
    }
}