using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace TgOperator.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
    [ApiVersion("1.0")]
    public class PlayerController : ControllerBase
    {
        /// <summary>
        /// Retrieves the player's balance
        /// </summary>
        /// <param name="request">Player balance request details</param>
        /// <returns>Player balance information</returns>
        /// <response code="200">Returns the player's balance</response>
        /// <response code="400">If the request is invalid</response>
        /// <response code="401">If the authentication fails</response>
        [HttpPost("GetBalance")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult GetBalance([FromBody] PlayerBalanceRequest request)
        {
            // TODO: Implement your business logic here
            // This is just a sample response
            return Ok(new 
            { 
                balance = 1000.00,
                currency = "USD",
                playerId = request.PlayerId,
                status = "success"
            });
        }
    }
} 