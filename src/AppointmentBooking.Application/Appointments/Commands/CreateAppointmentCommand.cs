using MediatR;
using AppointmentBooking.Application.DTOs.Appointments;

namespace AppointmentBooking.Application.Appointments.Commands;

public record CreateAppointmentCommand(CreateAppointmentRequest Request) : IRequest<CreateAppointmentResponse>;
