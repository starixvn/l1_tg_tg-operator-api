using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using TgOperator.Features.Players.Models;
using TgOperator.Features.Players.Commands;
using Sentry;
using MediatR;

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
        private readonly IMediator _mediator;

        public PlayerController(IPlayerService playerService, IHub sentryHub, IMediator mediator)
        {
            _playerService = playerService;
            _sentryHub = sentryHub;
            _mediator = mediator;
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
        
        [HttpPost("bet")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BetResult>> PlaceBet([FromBody] BetRequest request)
        {
            try
            {
                var result = await _mediator.Send(new PlaceBetCommand(request));
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _sentryHub.CaptureException(ex);
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("endround")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EndRoundResult>> EndRound([FromBody] EndRoundRequest request)
        {
            try
            {
                var result = await _mediator.Send(new EndRoundCommand(request));
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _sentryHub.CaptureException(ex, scope =>
                {
                    scope.SetExtra("totalWin", request.TotalWin);
                    scope.SetExtra("playerId", request.PlayerId);
                    scope.SetExtra("gameRoundId", request.GameRoundId);
                });
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _sentryHub.CaptureException(ex, scope =>
                {
                    scope.SetExtra("totalWin", request.TotalWin);
                    scope.SetExtra("playerId", request.PlayerId);
                    scope.SetExtra("gameRoundId", request.GameRoundId);
                });
                throw;
            }
        }
    }
} 