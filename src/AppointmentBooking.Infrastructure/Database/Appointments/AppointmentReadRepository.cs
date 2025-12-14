using AppointmentBooking.Application.Interfaces.Appointments;
using AppointmentBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AppointmentBooking.Infrastructure.Database.Appointments;

public class AppointmentReadRepository : IAppointmentReadRepository
{
    private readonly ReadEntityContext _context;

    public AppointmentReadRepository(ReadEntityContext context)
    {
        _context = context;
    }

    public async Task<(List<Appointment> Appointments, int TotalCount)> GetAppointments(
        int page,
        int pageSize,
        Guid? doctorId,
        Guid? patientId,
        DateTime? startDate,
        DateTime? endDate,
        CancellationToken cancellationToken)
    {
        var query = _context.Appointments
            .Include(a => a.Doctor)
            .Include(a => a.Patient)
            .AsQueryable();

        if (doctorId.HasValue)
        {
            query = query.Where(a => a.DoctorId == doctorId.Value);
        }

        if (patientId.HasValue)
        {
            query = query.Where(a => a.PatientId == patientId.Value);
        }

        if (startDate.HasValue)
        {
            query = query.Where(a => a.AppointmentDateTime >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(a => a.AppointmentDateTime <= endDate.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var appointments = await query
            .OrderByDescending(a => a.AppointmentDateTime)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (appointments, totalCount);
    }

    public async Task<Appointment?> GetAppointmentById(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Appointments
            .Include(a => a.Doctor)
            .Include(a => a.Patient)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }
}
