using Microsoft.AspNetCore.Mvc;

namespace AppointmentBooking.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthCheckController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(HealthCheckResponse), StatusCodes.Status200OK)]
    public IActionResult Get()
    {
        var response = new HealthCheckResponse
        {
            Status = "Healthy",
            Timestamp = DateTime.UtcNow,
            Version = "1.0.0"
        };

        return Ok(response);
    }
}

public class HealthCheckResponse
{
    public string Status { get; set; } = string.Empty;

    public DateTime Timestamp { get; set; }
    public string Version { get; set; } = string.Empty;
}
