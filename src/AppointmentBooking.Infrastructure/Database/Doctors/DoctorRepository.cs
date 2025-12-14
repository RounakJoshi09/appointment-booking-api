using AppointmentBooking.Application.Interfaces.Doctors;
using AppointmentBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AppointmentBooking.Infrastructure.Database.Doctors;

public class DoctorRepository : IDoctorRepository
{
    private readonly EntityContext _context;
    public DoctorRepository(EntityContext context)
    {
        _context = context;
    }
    public async Task<bool> CreateDoctor(Doctor doctor, CancellationToken cancellationToken)
    {
        await _context.Doctors.AddAsync(doctor);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<DoctorSchedule?> AddDoctorSchedule(DoctorSchedule schedule, CancellationToken cancellationToken)
    {
        await _context.DoctorSchedules.AddAsync(schedule, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return schedule;
    }

    public async Task<bool> DoctorExists(Guid doctorId, CancellationToken cancellationToken)
    {
        return await _context.Doctors.AnyAsync(d => d.Id == doctorId, cancellationToken);
    }
}