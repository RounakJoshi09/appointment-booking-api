using AppointmentBooking.Application.Appointments.Queries;
using AppointmentBooking.Application.DTOs.Appointments;
using AppointmentBooking.Application.Interfaces.Appointments;
using MediatR;

namespace AppointmentBooking.Application.Appointments.Handlers;

public class GetAppointmentsQueryHandler : IRequestHandler<GetAppointmentsQuery, PagedAppointmentsResponse>
{
    private readonly IAppointmentReadRepository _appointmentReadRepository;

    public GetAppointmentsQueryHandler(IAppointmentReadRepository appointmentReadRepository)
    {
        _appointmentReadRepository = appointmentReadRepository;
    }

    public async Task<PagedAppointmentsResponse> Handle(GetAppointmentsQuery request, CancellationToken cancellationToken)
    {
        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 10 : request.PageSize;

        var (appointments, totalCount) = await _appointmentReadRepository.GetAppointments(
            page,
            pageSize,
            request.DoctorId,
            request.PatientId,
            request.StartDate,
            request.EndDate,
            cancellationToken);

        var appointmentResponses = appointments.Select(a => new AppointmentResponse(
            a.Id,
            a.PatientId,
            a.Patient?.Name ?? "Unknown",
            a.Patient?.Email ?? "Unknown",
            a.DoctorId,
            a.Doctor?.Name ?? "Unknown",
            a.Doctor?.Email ?? "Unknown",
            a.AppointmentDateTime,
            (int)a.Duration.TotalMinutes,
            a.Status,
            a.CreatedAt,
            a.UpdatedAt
        )).ToList();

        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        return new PagedAppointmentsResponse(
            appointmentResponses,
            page,
            pageSize,
            totalCount,
            totalPages
        );
    }
}
