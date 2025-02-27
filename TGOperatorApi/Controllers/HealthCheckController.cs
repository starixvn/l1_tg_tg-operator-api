using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace TgOperator.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
    [ApiVersion("1.0")]
    public class HealthCheckController : ControllerBase
    {
        /// <summary>
        /// Checks the health status of the API
        /// </summary>
        /// <returns>Health status information</returns>
        /// <response code="200">Returns the health status</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Get()
        {
            return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
        }
    }
} 