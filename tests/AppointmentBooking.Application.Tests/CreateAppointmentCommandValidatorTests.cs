using System.ComponentModel.DataAnnotations;
using AppointmentBooking.Application.Appointments.Commands;
using AppointmentBooking.Application.DTOs.Appointments;
using AppointmentBooking.Application.Interfaces.Appointments;
using AppointmentBooking.Application.Constants;
using FluentAssertions;
using Moq;
using Xunit;

namespace AppointmentBooking.Application.Tests;

public class CreateAppointmentCommandValidatorTests
{
    private readonly Mock<IAppointmentRepository> _mockRepository;
    private readonly CreateAppointmentCommandValidator _validator;

    public CreateAppointmentCommandValidatorTests()
    {
        _mockRepository = new Mock<IAppointmentRepository>();
        _validator = new CreateAppointmentCommandValidator(_mockRepository.Object);
    }

    [Fact]
    public void Validate_Should_Throw_When_AppointmentDateTime_Is_In_Past()
    {
        var request = new CreateAppointmentRequest
        {
            PatientId = Guid.NewGuid(),
            DoctorId = Guid.NewGuid(),
            AppointmentDateTime = DateTime.Now.AddMinutes(-10),
            DurationInMinutes = 30
        };
        var command = new CreateAppointmentCommand(request);

        var exception = Assert.Throws<ValidationException>(() => _validator.Validate(command));
        exception.Message.Should().Contain("must be scheduled for a future date and time");
    }

    [Fact]
    public void Validate_Should_Convert_IST_To_UTC_For_Future_Check()
    {
        var utcNow = DateTime.UtcNow;
        var istTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, TimeZoneInfo.FindSystemTimeZoneById(TimeZoneConstants.IndiaStandardTime));
        var pastIstTime = istTime.AddMinutes(-10);

        var request = new CreateAppointmentRequest
        {
            PatientId = Guid.NewGuid(),
            DoctorId = Guid.NewGuid(),
            AppointmentDateTime = pastIstTime,
            DurationInMinutes = 30
        };
        var command = new CreateAppointmentCommand(request);

        var exception = Assert.Throws<ValidationException>(() => _validator.Validate(command));
        exception.Message.Should().Contain("must be scheduled for a future date and time");
    }
}
