using Microsoft.AspNetCore.Mvc;
using AppointmentBooking.Application.DTOs;
using MediatR;
using AppointmentBooking.Application.Doctors.Commands;
using AppointmentBooking.Application.Doctors.Queries;
namespace AppointmentBooking.API.Controllers;

[ApiController]
[Route("api/doctors")]
public class DoctorsController : ControllerBase
{
    private readonly IMediator _mediator;
    public DoctorsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateDoctor([FromBody] CreateDoctorRequest request)
    {
        var result = await _mediator.Send(new CreateDoctorCommand(request));
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllDoctors()
    {
        var result = await _mediator.Send(new GetDoctorsQuery());
        return Ok(result);
    }

    [HttpGet]
    [Route("{doctorId}/availability")]
    public async Task<IActionResult> GetDoctorAvailability(
        [FromRoute] Guid doctorId,
        [FromQuery] string date)
    {
        if (!DateOnly.TryParse(date, out var dateOnly))
        {
            return BadRequest(new { error = "Invalid date format. Expected format: YYYY-MM-DD" });
        }

        var result = await _mediator.Send(new GetDoctorAvailabilityQuery(doctorId, dateOnly));
        return Ok(result);
    }

    [HttpPost]
    [Route("schedule")]
    public async Task<IActionResult> AddDoctorSchedule([FromBody] AddDoctorScheduleRequest request)
    {
        try
        {
            var result = await _mediator.Send(new AddDoctorScheduleCommand(request));
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet]
    [Route("{doctorId}/schedules")]
    public async Task<IActionResult> GetDoctorSchedules(
        [FromRoute] Guid doctorId,
        [FromQuery] string date)
    {
        if (!DateOnly.TryParse(date, out var dateOnly))
        {
            return BadRequest(new { error = "Invalid date format. Expected format: YYYY-MM-DD" });
        }

        var result = await _mediator.Send(new GetDoctorSchedulesQuery(doctorId, dateOnly));
        return Ok(result);
    }
}