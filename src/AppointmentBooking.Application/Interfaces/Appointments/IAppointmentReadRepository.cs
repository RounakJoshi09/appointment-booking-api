using AppointmentBooking.Domain.Entities;

namespace AppointmentBooking.Application.Interfaces.Appointments;

public interface IAppointmentReadRepository
{
    Task<(List<Appointment> Appointments, int TotalCount)> GetAppointments(
        int page,
        int pageSize,
        Guid? doctorId,
        Guid? patientId,
        DateTime? startDate,
        DateTime? endDate,
        CancellationToken cancellationToken);

    Task<Appointment?> GetAppointmentById(Guid id, CancellationToken cancellationToken);
}
