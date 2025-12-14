using MediatR;
using AppointmentBooking.Application.DTOs.Appointments;

namespace AppointmentBooking.Application.Appointments.Queries;

public record GetAppointmentByIdQuery(Guid Id) : IRequest<AppointmentResponse?>;
