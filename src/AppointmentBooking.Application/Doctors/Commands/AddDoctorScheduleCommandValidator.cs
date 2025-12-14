using System.ComponentModel.DataAnnotations;
using AppointmentBooking.Application.Behavior;

namespace AppointmentBooking.Application.Doctors.Commands;

public class AddDoctorScheduleCommandValidator : IValidator<AddDoctorScheduleCommand>
{
    public void Validate(AddDoctorScheduleCommand command)
    {
        if (command.Request == null)
        {
            throw new ValidationException("Request cannot be null");
        }

        if (command.Request.DoctorId == Guid.Empty)
        {
            throw new ValidationException("DoctorId is required.");
        }

        if (command.Request.DayOfWeek.HasValue && command.Request.Date.HasValue)
        {
            throw new ValidationException("Cannot specify both DayOfWeek and Date. Use DayOfWeek for recurring schedules or Date for specific date schedules.");
        }

        if (!command.Request.DayOfWeek.HasValue && !command.Request.Date.HasValue)
        {
            throw new ValidationException("Either DayOfWeek or Date must be specified.");
        }

        if (!command.Request.IsOffDay)
        {
            if (!command.Request.StartTime.HasValue)
            {
                throw new ValidationException("StartTime is required when IsOffDay is false.");
            }

            if (!command.Request.EndTime.HasValue)
            {
                throw new ValidationException("EndTime is required when IsOffDay is false.");
            }

            if (command.Request.StartTime.HasValue && command.Request.EndTime.HasValue)
            {
                if (command.Request.StartTime >= command.Request.EndTime)
                {
                    throw new ValidationException("StartTime must be before EndTime.");
                }
            }
        }
    }
}
