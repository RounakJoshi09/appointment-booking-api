using System.ComponentModel.DataAnnotations;
using AppointmentBooking.Application.Appointments.Commands;
using AppointmentBooking.Application.DTOs.Appointments;
using FluentAssertions;
using Xunit;

namespace AppointmentBooking.Application.Tests;

public class RescheduleAppointmentCommandValidatorTests
{
    private readonly RescheduleAppointmentCommandValidator _validator = new();

    [Fact]
    public void Validate_Should_Throw_When_AppointmentId_Is_Empty()
    {
        var command = new RescheduleAppointmentCommand(Guid.Empty, new RescheduleAppointmentRequest
        {
            AppointmentDateTime = DateTime.UtcNow.AddDays(2)
        });

        var exception = Assert.Throws<ValidationException>(() => _validator.Validate(command));
        exception.Message.Should().Contain("AppointmentId is required");
    }

    [Fact]
    public void Validate_Should_Throw_When_Request_Is_Null()
    {
        var command = new RescheduleAppointmentCommand(Guid.NewGuid(), null!);

        var exception = Assert.Throws<ValidationException>(() => _validator.Validate(command));
        exception.Message.Should().Contain("Request cannot be null");
    }

    [Fact]
    public void Validate_Should_Throw_When_Duration_Is_Not_Positive()
    {
        var command = new RescheduleAppointmentCommand(Guid.NewGuid(), new RescheduleAppointmentRequest
        {
            AppointmentDateTime = DateTime.UtcNow.AddDays(2),
            DurationInMinutes = 0
        });

        var exception = Assert.Throws<ValidationException>(() => _validator.Validate(command));
        exception.Message.Should().Contain("Duration must be greater than 0");
    }

    [Fact]
    public void Validate_Should_Throw_When_Appointment_Is_In_The_Past()
    {
        var command = new RescheduleAppointmentCommand(Guid.NewGuid(), new RescheduleAppointmentRequest
        {
            AppointmentDateTime = DateTime.UtcNow.AddDays(-1)
        });

        var exception = Assert.Throws<ValidationException>(() => _validator.Validate(command));
        exception.Message.Should().Contain("future date and time");
    }

    [Fact]
    public void Validate_Should_Pass_For_Valid_Future_Appointment()
    {
        var command = new RescheduleAppointmentCommand(Guid.NewGuid(), new RescheduleAppointmentRequest
        {
            AppointmentDateTime = DateTime.UtcNow.AddDays(3),
            DurationInMinutes = 30
        });

        var act = () => _validator.Validate(command);
        act.Should().NotThrow();
    }
}
