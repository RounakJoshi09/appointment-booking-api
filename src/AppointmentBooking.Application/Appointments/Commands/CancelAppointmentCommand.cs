using MediatR;
using AppointmentBooking.Application.DTOs.Appointments;

namespace AppointmentBooking.Application.Appointments.Commands;

public record CancelAppointmentCommand(Guid AppointmentId) : IRequest<CancelAppointmentResponse>;
