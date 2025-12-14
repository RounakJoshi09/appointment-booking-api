using System.ComponentModel.DataAnnotations;
using MediatR;
using AppointmentBooking.Application.Appointments.Commands;
using AppointmentBooking.Application.DTOs.Appointments;
using AppointmentBooking.Application.Interfaces.Appointments;
using AppointmentBooking.Domain.Enums;

namespace AppointmentBooking.Application.Appointments.Handlers;

public class CancelAppointmentCommandHandler : IRequestHandler<CancelAppointmentCommand, CancelAppointmentResponse>
{
    private readonly IAppointmentRepository _appointmentRepository;

    public CancelAppointmentCommandHandler(IAppointmentRepository appointmentRepository)
    {
        _appointmentRepository = appointmentRepository;
    }

    public async Task<CancelAppointmentResponse> Handle(CancelAppointmentCommand request, CancellationToken cancellationToken)
    {
        var appointment = await _appointmentRepository.GetAppointmentById(request.AppointmentId, cancellationToken);

        if (appointment == null)
        {
            throw new ValidationException($"Appointment with ID {request.AppointmentId} does not exist");
        }

        if (appointment.Status == AppointmentStatus.Cancelled)
        {
            throw new ValidationException($"Appointment is already cancelled");
        }

        if (appointment.Status == AppointmentStatus.Completed)
        {
            throw new ValidationException($"Cannot cancel a completed appointment");
        }

        if (appointment.Status != AppointmentStatus.Scheduled)
        {
            throw new ValidationException($"Can only cancel appointments with Scheduled status. Current status: {appointment.Status}");
        }

        appointment.Status = AppointmentStatus.Cancelled;
        await _appointmentRepository.UpdateAppointment(appointment, cancellationToken);

        return new CancelAppointmentResponse
        {
            Message = "Appointment cancelled successfully"
        };
    }
}
