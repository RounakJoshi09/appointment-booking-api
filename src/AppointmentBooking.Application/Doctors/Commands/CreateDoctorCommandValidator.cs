using System.ComponentModel.DataAnnotations;
using AppointmentBooking.Application.Behavior;
namespace AppointmentBooking.Application.Doctors.Commands;

public class CreateDoctorCommandValidator : IValidator<CreateDoctorCommand>
{
    public void Validate(CreateDoctorCommand request)
    {
        if (request.Request == null)
        {
            throw new ValidationException("Request cannot be null");
        }

        if (string.IsNullOrWhiteSpace(request.Request.Name) || request.Request.Name.Length < 3)
        {
            throw new ValidationException("Name must be at least 3 characters long");
        }
        if (string.IsNullOrWhiteSpace(request.Request.Email) || !Utils.IsValidEmail(request.Request.Email))
        {
            throw new ValidationException("Email is not valid");
        }
    }
}