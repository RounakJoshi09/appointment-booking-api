namespace AppointmentBooking.Infrastructure.Database.Doctors;
using AppointmentBooking.Application.Interfaces.Doctors;
using AppointmentBooking.Domain.Entities;
using AppointmentBooking.Domain.Enums;
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

    public async Task<List<AvailabilitySlot>> GetDoctorsAvailabilityByDate(Guid doctorId, DateOnly date)
    {
        var schedules = await GetDoctorSchedulesByDate(doctorId, date);

        if (schedules.Count == 0)
        {
            return new List<AvailabilitySlot>();
        }

        var dateTime = date.ToDateTime(TimeOnly.MinValue);
        var appointments = await _context.Appointments
            .Where(a =>
                a.DoctorId == doctorId &&
                a.Status == AppointmentStatus.Scheduled &&
                a.AppointmentDateTime.Date == dateTime.Date)
            .OrderBy(a => a.AppointmentDateTime)
            .ToListAsync();

        var availableSlots = new List<AvailabilitySlot>();

        foreach (var schedule in schedules)
        {
            var scheduleStart = schedule.StartTime!.Value;
            var scheduleEnd = schedule.EndTime!.Value;

            var relevantAppointments = appointments
                .Where(a =>
                {
                    var appointmentTime = a.AppointmentDateTime.TimeOfDay;
                    var appointmentEnd = appointmentTime + a.Duration;
                    return appointmentTime < scheduleEnd && appointmentEnd > scheduleStart;
                })
                .OrderBy(a => a.AppointmentDateTime.TimeOfDay)
                .ToList();

            if (relevantAppointments.Count == 0)
            {
                availableSlots.Add(new AvailabilitySlot(scheduleStart, scheduleEnd));
                continue;
            }

            var currentTime = scheduleStart;

            foreach (var appointment in relevantAppointments)
            {
                var appointmentStart = appointment.AppointmentDateTime.TimeOfDay;
                var appointmentEnd = appointmentStart + appointment.Duration;

                if (currentTime < appointmentStart)
                {
                    availableSlots.Add(new AvailabilitySlot(currentTime, appointmentStart));
                }

                currentTime = appointmentEnd > currentTime ? appointmentEnd : currentTime;
            }

            if (currentTime < scheduleEnd)
            {
                availableSlots.Add(new AvailabilitySlot(currentTime, scheduleEnd));
            }
        }

        return availableSlots.OrderBy(slot => slot.StartTime).ToList();
    }

    public async Task<List<DoctorSchedule>> GetDoctorSchedulesByDateIncludingOffDays(Guid doctorId, DateOnly date)
    {
        var dateTime = date.ToDateTime(TimeOnly.MinValue);
        var dayOfWeek = date.DayOfWeek;

        var dateSpecificSchedules = await _context.DoctorSchedules
            .Where(schedule =>
                schedule.DoctorId == doctorId &&
                schedule.Date.HasValue &&
                schedule.Date.Value.Date == dateTime.Date)
            .ToListAsync();

        if (dateSpecificSchedules.Count > 0)
        {
            return dateSpecificSchedules;
        }

        // If no date-specific schedules, get recurring schedules for this day of week
        return await _context.DoctorSchedules
            .Where(schedule =>
                schedule.DoctorId == doctorId &&
                schedule.DayOfWeek == dayOfWeek &&
                !schedule.Date.HasValue)
            .ToListAsync();
    }
}