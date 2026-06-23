using System.ComponentModel.DataAnnotations;
using MediatR;
using AppointmentBooking.Application.Appointments.Commands;
using AppointmentBooking.Application.Constants;
using AppointmentBooking.Application.DTOs.Appointments;
using AppointmentBooking.Application.Interfaces.Appointments;
using AppointmentBooking.Domain.Enums;

namespace AppointmentBooking.Application.Appointments.Handlers;

public class RescheduleAppointmentCommandHandler : IRequestHandler<RescheduleAppointmentCommand, RescheduleAppointmentResponse>
{
    private readonly IAppointmentRepository _appointmentRepository;

    public RescheduleAppointmentCommandHandler(IAppointmentRepository appointmentRepository)
    {
        _appointmentRepository = appointmentRepository;
    }

    public async Task<RescheduleAppointmentResponse> Handle(RescheduleAppointmentCommand request, CancellationToken cancellationToken)
    {
        var appointment = await _appointmentRepository.GetAppointmentById(request.AppointmentId, cancellationToken);

        if (appointment == null)
        {
            throw new ValidationException($"Appointment with ID {request.AppointmentId} does not exist");
        }

        if (appointment.Status == AppointmentStatus.Cancelled)
        {
            throw new ValidationException("Cannot reschedule a cancelled appointment");
        }

        if (appointment.Status == AppointmentStatus.Completed)
        {
            throw new ValidationException("Cannot reschedule a completed appointment");
        }

        if (appointment.Status != AppointmentStatus.Scheduled)
        {
            throw new ValidationException($"Can only reschedule appointments with Scheduled status. Current status: {appointment.Status}");
        }

        // Request appointment is local IST; schedules are stored as IST clock times.
        // Use local date/time for schedule lookup and working-hours checks only.
        var localAppointmentDateTime = request.Request.AppointmentDateTime;
        var utcAppointmentDateTime = Utils.ConvertTimeZoneToUtc(localAppointmentDateTime, TimeZoneConstants.IndiaStandardTime);
        var duration = TimeSpan.FromMinutes(request.Request.DurationInMinutes ?? (int)appointment.Duration.TotalMinutes);

        var doctorSchedules = await _appointmentRepository.GetDoctorSchedules(
            appointment.DoctorId,
            localAppointmentDateTime.Date,
            cancellationToken);

        if (doctorSchedules == null || !doctorSchedules.Any())
        {
            throw new ValidationException($"Doctor does not have a schedule configured for {localAppointmentDateTime}");
        }

        if (doctorSchedules.All(s => s.IsOffDay))
        {
            throw new ValidationException($"Doctor is not available on {localAppointmentDateTime} (off day)");
        }

        var appointmentTime = localAppointmentDateTime.TimeOfDay;
        var appointmentEndTime = appointmentTime.Add(duration);

        var isWithinWorkingHours = false;
        foreach (var schedule in doctorSchedules.Where(s => !s.IsOffDay))
        {
            if (schedule.StartTime == null || schedule.EndTime == null)
            {
                continue;
            }

            if (appointmentTime >= schedule.StartTime.Value && appointmentEndTime <= schedule.EndTime.Value)
            {
                isWithinWorkingHours = true;
                break;
            }
        }

        if (!isWithinWorkingHours)
        {
            throw new ValidationException("Appointment time must be within doctor's working hours");
        }

        // Overlap check + update run atomically (serializable transaction) so concurrent bookings cannot both succeed.
        appointment.AppointmentDateTime = utcAppointmentDateTime;
        appointment.Duration = duration;

        var updatedAppointment = await _appointmentRepository.UpdateAppointmentIfNoOverlap(
            appointment,
            excludeAppointmentId: appointment.Id,
            cancellationToken);

        if (updatedAppointment is null)
        {
            throw new ValidationException("Doctor already has an appointment scheduled during this time. Please choose a different time slot.");
        }

        return new RescheduleAppointmentResponse
        {
            Id = updatedAppointment.Id,
            PatientId = updatedAppointment.PatientId,
            DoctorId = updatedAppointment.DoctorId,
            AppointmentDateTime = updatedAppointment.AppointmentDateTime,
            Duration = updatedAppointment.Duration,
            Status = updatedAppointment.Status,
            Message = "Appointment rescheduled successfully"
        };
    }
}
