using AppointmentBooking.Domain.Entities;

namespace AppointmentBooking.Application.Interfaces.Patients;

public interface IPatientReadRepository
{
    Task<List<Patient>> GetAllPatients();
}
