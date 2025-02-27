using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using System.Threading.Tasks;

namespace TgOperator.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
    [ApiVersion("1.0")]
    public class PlayerController : ControllerBase
    {
        private readonly IPlayerService _playerService;

        public PlayerController(IPlayerService playerService)
        {
            _playerService = playerService;
        }

        /// <summary>
        /// Retrieves the player's balance
        /// </summary>
        /// <param name="request">Player balance request details</param>
        /// <returns>Player balance information</returns>
        /// <response code="200">Returns the player's balance</response>
        /// <response code="400">If the request is invalid</response>
        /// <response code="401">If the authentication fails</response>
        /// <response code="404">If the player is not found</response>
        [HttpPost("balance")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetBalance([FromBody] PlayerBalanceRequest request)
        {
            var player = await _playerService.GetPlayerBalanceAsync(
                request.OperatorId,
                request.PlayerId,
                request.SiteId);

            if (player == null)
            {
                return NotFound(new { message = "Player not found" });
            }

            return Ok(new 
            { 
                balance = player.Balance,
                currency = player.Currency,
                playerId = player.PlayerId,
                status = "success"
            });
        }
    }
} 