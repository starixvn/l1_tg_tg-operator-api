using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using TgOperator.Features.Players.Models;
using Sentry;

namespace TgOperator.Features.Players
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
    [ApiVersion("1.0")]
    public class PlayerController : ControllerBase
    {
        private readonly IPlayerService _playerService;
        private readonly IHub _sentryHub;

        public PlayerController(IPlayerService playerService, IHub sentryHub)
        {
            _playerService = playerService;
            _sentryHub = sentryHub;
        }

        [HttpPost("balance")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetBalance([FromBody] PlayerBalanceRequest request)
        {
            try
            {
                var player = await _playerService.GetPlayerBalanceAsync(
                    request.OperatorId,
                    request.PlayerId,
                    request.SiteId);

                if (player == null)
                {
                    _sentryHub.CaptureMessage($"Player not found: {request.PlayerId}", SentryLevel.Warning);
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
            catch (Exception ex)
            {
                _sentryHub.CaptureException(ex);
                throw;
            }
        }
    }
} 