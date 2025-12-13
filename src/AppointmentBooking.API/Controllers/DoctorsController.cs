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
}