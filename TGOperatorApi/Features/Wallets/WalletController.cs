using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using TgOperator.Features.Wallets.Models;
using TgOperator.Features.Wallets.Commands;
using MediatR;
using Sentry;

namespace TgOperator.Features.Wallets
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
    [ApiVersion("1.0")]
    public class WalletController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IHub _sentryHub;

        public WalletController(IMediator mediator, IHub sentryHub)
        {
            _mediator = mediator;
            _sentryHub = sentryHub;
        }

        [HttpPost("deposit")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<WalletResponse>> Deposit([FromBody] DepositRequest request)
        {
            try
            {
                var result = await _mediator.Send(new DepositCommand(request));
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _sentryHub.CaptureException(ex);
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("withdraw")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<WalletResponse>> Withdraw([FromBody] WithdrawRequest request)
        {
            try
            {
                var result = await _mediator.Send(new WithdrawCommand(request));
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _sentryHub.CaptureException(ex);
                return BadRequest(new { message = ex.Message });
            }
        }
    }
} 