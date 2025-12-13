namespace AppointmentBooking.Application.Behavior;

public interface IValidator<in TRequest>
{
    void Validate(TRequest request);
}
