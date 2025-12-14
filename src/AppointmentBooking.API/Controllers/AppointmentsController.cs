using Microsoft.AspNetCore.Mvc;
using MediatR;
using AppointmentBooking.Application.Appointments.Commands;
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
}
