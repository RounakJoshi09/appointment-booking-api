using AppointmentBooking.Application.Interfaces.Doctors;
using AppointmentBooking.Domain.Entities;

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
}