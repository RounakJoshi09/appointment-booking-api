namespace AppointmentBooking.Infrastructure.Database.Patients;

using AppointmentBooking.Application.Interfaces.Patients;
using AppointmentBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;

public class PatientReadRepository : IPatientReadRepository
{
    private readonly ReadEntityContext _context;

    public PatientReadRepository(ReadEntityContext context)
    {
        _context = context;
    }

    public async Task<List<Patient>> GetAllPatients()
    {
        return await _context.Patients.ToListAsync();
    }
}
