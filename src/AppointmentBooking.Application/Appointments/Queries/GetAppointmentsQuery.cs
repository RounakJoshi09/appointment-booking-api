using MediatR;
using AppointmentBooking.Application.DTOs.Appointments;

namespace AppointmentBooking.Application.Appointments.Queries;

public record GetAppointmentsQuery(
    int Page = 1,
    int PageSize = 10,
    Guid? DoctorId = null,
    Guid? PatientId = null,
    DateTime? StartDate = null,
    DateTime? EndDate = null
) : IRequest<PagedAppointmentsResponse>;
