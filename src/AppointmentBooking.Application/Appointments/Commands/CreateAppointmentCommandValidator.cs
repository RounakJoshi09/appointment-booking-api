using System.ComponentModel.DataAnnotations;
using AppointmentBooking.Application.Behavior;
using AppointmentBooking.Application.Constants;
using AppointmentBooking.Application.Interfaces.Appointments;

namespace AppointmentBooking.Application.Appointments.Commands;

public class CreateAppointmentCommandValidator : IValidator<CreateAppointmentCommand>
{
    private readonly IAppointmentRepository _appointmentRepository;

    public CreateAppointmentCommandValidator(IAppointmentRepository appointmentRepository)
    {
        _appointmentRepository = appointmentRepository;
    }

    public void Validate(CreateAppointmentCommand request)
    {
        if (request.Request == null)
        {
            throw new ValidationException("Request cannot be null");
        }

        if (request.Request.PatientId == Guid.Empty)
        {
            throw new ValidationException("PatientId is required");
        }

        if (request.Request.DoctorId == Guid.Empty)
        {
            throw new ValidationException("DoctorId is required");
        }

        if (request.Request.DurationInMinutes.HasValue && request.Request.DurationInMinutes.Value <= 0)
        {
            throw new ValidationException("Duration must be greater than 0 minutes");
        }

        // Convert appointment datetime from client timezone(considering IST Currently) to UTC
        var utcAppointmentDateTime = Utils.ConvertTimeZoneToUtc(request.Request.AppointmentDateTime, TimeZoneConstants.IndiaStandardTime);

        if (utcAppointmentDateTime <= DateTime.UtcNow)
        {
            throw new ValidationException("Appointment must be scheduled for a future date and time");
        }
    }
}
