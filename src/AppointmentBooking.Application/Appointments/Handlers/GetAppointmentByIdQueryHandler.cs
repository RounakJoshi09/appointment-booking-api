using AppointmentBooking.Application.Appointments.Queries;
using AppointmentBooking.Application.DTOs.Appointments;
using AppointmentBooking.Application.Interfaces.Appointments;
using MediatR;

namespace AppointmentBooking.Application.Appointments.Handlers;

public class GetAppointmentByIdQueryHandler : IRequestHandler<GetAppointmentByIdQuery, AppointmentResponse?>
{
    private readonly IAppointmentReadRepository _appointmentReadRepository;

    public GetAppointmentByIdQueryHandler(IAppointmentReadRepository appointmentReadRepository)
    {
        _appointmentReadRepository = appointmentReadRepository;
    }

    public async Task<AppointmentResponse?> Handle(GetAppointmentByIdQuery request, CancellationToken cancellationToken)
    {
        var appointment = await _appointmentReadRepository.GetAppointmentById(request.Id, cancellationToken);

        if (appointment == null)
        {
            return null;
        }

        return new AppointmentResponse(
            appointment.Id,
            appointment.PatientId,
            appointment.Patient?.Name ?? "Unknown",
            appointment.Patient?.Email ?? "Unknown",
            appointment.DoctorId,
            appointment.Doctor?.Name ?? "Unknown",
            appointment.Doctor?.Email ?? "Unknown",
            appointment.AppointmentDateTime,
            (int)appointment.Duration.TotalMinutes,
            appointment.Status,
            appointment.CreatedAt,
            appointment.UpdatedAt
        );
    }
}
