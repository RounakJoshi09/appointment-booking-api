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
}