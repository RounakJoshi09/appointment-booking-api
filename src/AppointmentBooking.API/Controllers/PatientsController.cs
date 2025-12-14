using Microsoft.AspNetCore.Mvc;
using MediatR;
using AppointmentBooking.Application.Patients.Queries;

namespace AppointmentBooking.API.Controllers;

[ApiController]
[Route("api/patients")]
public class PatientsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PatientsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllPatients()
    {
        var result = await _mediator.Send(new GetPatientsQuery());
        return Ok(result);
    }
}
