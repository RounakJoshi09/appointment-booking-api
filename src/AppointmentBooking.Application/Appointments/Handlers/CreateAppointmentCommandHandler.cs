using System.ComponentModel.DataAnnotations;
using MediatR;
using AppointmentBooking.Application.Appointments.Commands;
using AppointmentBooking.Application.DTOs.Appointments;
using AppointmentBooking.Application.Interfaces.Appointments;
using AppointmentBooking.Domain.Entities;
using AppointmentBooking.Domain.Enums;
using AppointmentBooking.Application.Constants;

namespace AppointmentBooking.Application.Appointments.Handlers;

public class CreateAppointmentCommandHandler : IRequestHandler<CreateAppointmentCommand, CreateAppointmentResponse>
{
    private readonly IAppointmentRepository _appointmentRepository;

    public CreateAppointmentCommandHandler(IAppointmentRepository appointmentRepository)
    {
        _appointmentRepository = appointmentRepository;
    }

    public async Task<CreateAppointmentResponse> Handle(CreateAppointmentCommand request, CancellationToken cancellationToken)
    {
        var patientExists = await _appointmentRepository.PatientExists(request.Request.PatientId, cancellationToken);
        if (!patientExists)
        {
            throw new ValidationException($"Patient with ID {request.Request.PatientId} does not exist");
        }

        var doctorExists = await _appointmentRepository.DoctorExists(request.Request.DoctorId, cancellationToken);
        if (!doctorExists)
        {
            throw new ValidationException($"Doctor with ID {request.Request.DoctorId} does not exist");
        }

        // Request appointment is local IST; schedules are stored as IST clock times.
        // Use local date/time for schedule lookup and working-hours checks only.
        var localAppointmentDateTime = request.Request.AppointmentDateTime;
        var utcAppointmentDateTime = Utils.ConvertTimeZoneToUtc(localAppointmentDateTime, TimeZoneConstants.IndiaStandardTime);

        var duration = TimeSpan.FromMinutes(request.Request.DurationInMinutes ?? 30);

        var doctorSchedules = await _appointmentRepository.GetDoctorSchedules(
            request.Request.DoctorId,
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

        // Overlap and persistence use UTC instants (appointments stored in UTC).
        var hasOverlap = await _appointmentRepository.HasOverlappingAppointment(
            request.Request.DoctorId,
            utcAppointmentDateTime,
            duration,
            cancellationToken);

        if (hasOverlap)
        {
            throw new ValidationException("Doctor already has an appointment scheduled during this time. Please choose a different time slot.");
        }

        var appointment = new Appointment
        {
            PatientId = request.Request.PatientId,
            DoctorId = request.Request.DoctorId,
            AppointmentDateTime = utcAppointmentDateTime,
            Duration = duration,
            Status = AppointmentStatus.Scheduled
        };

        var createdAppointment = await _appointmentRepository.CreateAppointment(appointment, cancellationToken);

        return new CreateAppointmentResponse
        {
            Id = createdAppointment.Id,
            PatientId = createdAppointment.PatientId,
            DoctorId = createdAppointment.DoctorId,
            AppointmentDateTime = createdAppointment.AppointmentDateTime,
            Duration = createdAppointment.Duration,
            Status = createdAppointment.Status,
            CreatedAt = createdAppointment.CreatedAt
        };
    }
}
