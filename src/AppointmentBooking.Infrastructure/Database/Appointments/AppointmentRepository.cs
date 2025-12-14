using AppointmentBooking.Application.Interfaces.Appointments;
using AppointmentBooking.Application.Interfaces;
using AppointmentBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using AppointmentBooking.Domain.Enums;

namespace AppointmentBooking.Infrastructure.Database.Appointments;

public class AppointmentRepository : IAppointmentRepository
{
    private readonly EntityContext _context;

    public AppointmentRepository(EntityContext context)
    {
        _context = context;
    }

    public async Task<Appointment> CreateAppointment(Appointment appointment, CancellationToken cancellationToken)
    {
        await _context.Appointments.AddAsync(appointment, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return appointment;
    }

    public async Task<Appointment?> GetAppointmentById(Guid appointmentId, CancellationToken cancellationToken)
    {
        return await _context.Appointments
            .FirstOrDefaultAsync(a => a.Id == appointmentId, cancellationToken);
    }

    public async Task<Appointment> UpdateAppointment(Appointment appointment, CancellationToken cancellationToken)
    {
        _context.Appointments.Update(appointment);
        await _context.SaveChangesAsync(cancellationToken);
        return appointment;
    }

    public async Task<bool> HasOverlappingAppointment(Guid doctorId, DateTime appointmentDateTime, TimeSpan duration, CancellationToken cancellationToken)
    {
        var appointmentEndTime = appointmentDateTime.Add(duration);

        var appointmentsOnDate = await _context.Appointments
            .Where(a => a.DoctorId == doctorId
                && a.Status == AppointmentStatus.Scheduled
                && a.AppointmentDateTime.Date == appointmentDateTime.Date)
            .Select(a => new { a.AppointmentDateTime, a.Duration })
            .ToListAsync(cancellationToken);

        var hasOverlap = appointmentsOnDate.Any(a =>
        {
            var existingEndTime = a.AppointmentDateTime.Add(a.Duration);
            return appointmentDateTime < existingEndTime && appointmentEndTime > a.AppointmentDateTime;
        });

        return hasOverlap;
    }

    public async Task<List<DoctorSchedule>> GetDoctorSchedules(Guid doctorId, DateTime date, CancellationToken cancellationToken)
    {
        var dayOfWeek = date.DayOfWeek;
        var dateOnly = date.Date;

        var specificSchedules = await _context.DoctorSchedules
            .Where(s => s.DoctorId == doctorId && s.Date == dateOnly)
            .ToListAsync(cancellationToken);

        if (specificSchedules.Any())
        {
            return specificSchedules;
        }

        var recurringSchedules = await _context.DoctorSchedules
            .Where(s => s.DoctorId == doctorId && s.DayOfWeek == dayOfWeek && s.Date == null)
            .ToListAsync(cancellationToken);

        return recurringSchedules;
    }

    public async Task<bool> DoctorExists(Guid doctorId, CancellationToken cancellationToken)
    {
        return await _context.Doctors.AnyAsync(d => d.Id == doctorId, cancellationToken);
    }

    public async Task<bool> PatientExists(Guid patientId, CancellationToken cancellationToken)
    {
        return await _context.Patients.AnyAsync(p => p.Id == patientId, cancellationToken);
    }
}
