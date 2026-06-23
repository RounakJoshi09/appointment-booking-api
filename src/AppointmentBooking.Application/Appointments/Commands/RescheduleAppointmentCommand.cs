using MediatR;
using AppointmentBooking.Application.DTOs.Appointments;

namespace AppointmentBooking.Application.Appointments.Commands;

public record RescheduleAppointmentCommand(Guid AppointmentId, RescheduleAppointmentRequest Request) : IRequest<RescheduleAppointmentResponse>;
