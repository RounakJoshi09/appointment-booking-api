using Microsoft.AspNetCore.Mvc;
using MediatR;
using AppointmentBooking.Application.Appointments.Commands;
using AppointmentBooking.Application.Appointments.Queries;
using AppointmentBooking.Application.DTOs.Appointments;

namespace AppointmentBooking.API.Controllers;

[ApiController]
[Route("api/appointments")]
public class AppointmentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AppointmentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAppointment([FromBody] CreateAppointmentRequest request)
    {
        var result = await _mediator.Send(new CreateAppointmentCommand(request));
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAppointments(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] Guid? doctorId = null,
        [FromQuery] Guid? patientId = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        var result = await _mediator.Send(new GetAppointmentsQuery(
            page,
            pageSize,
            doctorId,
            patientId,
            startDate,
            endDate
        ));
        return Ok(result);
    }

    [HttpGet("{appointmentId}")]
    public async Task<IActionResult> GetAppointmentById(Guid appointmentId)
    {
        var result = await _mediator.Send(new GetAppointmentByIdQuery(appointmentId));

        if (result == null)
        {
            return NotFound(new { message = "Appointment not found" });
        }

        return Ok(result);
    }

    [HttpPut("{appointmentId}/cancel")]
    public async Task<IActionResult> CancelAppointment(Guid appointmentId)
    {
        var result = await _mediator.Send(new CancelAppointmentCommand(appointmentId));
        return Ok(result);
    }
}
