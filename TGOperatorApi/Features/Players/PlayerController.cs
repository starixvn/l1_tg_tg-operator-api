using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using TgOperator.Features.Players.Models;

namespace TgOperator.Features.Players
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