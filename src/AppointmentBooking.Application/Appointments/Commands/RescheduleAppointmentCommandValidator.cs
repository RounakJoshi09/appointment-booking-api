using System.ComponentModel.DataAnnotations;
using AppointmentBooking.Application.Behavior;
using AppointmentBooking.Application.Constants;

namespace AppointmentBooking.Application.Appointments.Commands;

public class RescheduleAppointmentCommandValidator : IValidator<RescheduleAppointmentCommand>
{
    public void Validate(RescheduleAppointmentCommand request)
    {
        if (request.AppointmentId == Guid.Empty)
        {
            throw new ValidationException("AppointmentId is required");
        }

        if (request.Request == null)
        {
            throw new ValidationException("Request cannot be null");
        }

        if (request.Request.DurationInMinutes.HasValue && request.Request.DurationInMinutes.Value <= 0)
        {
            throw new ValidationException("Duration must be greater than 0 minutes");
        }

        var utcAppointmentDateTime = Utils.ConvertTimeZoneToUtc(request.Request.AppointmentDateTime, TimeZoneConstants.IndiaStandardTime);

        if (utcAppointmentDateTime <= DateTime.UtcNow)
        {
            throw new ValidationException("Appointment must be rescheduled for a future date and time");
        }
    }
}
