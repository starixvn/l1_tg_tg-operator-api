using MediatR;
using MongoDB.Driver;
using TgOperator.Features.Players.Commands;
using TgOperator.Features.Players.Models;
using TgOperator.Shared.MongoDB;
using Microsoft.Extensions.Options;
using TgOperator.Shared.Configuration;
using Sentry;
using TGOperatorApi.Features.Players.Models.Enums;
using TgOperator.Features.Players.Repositories;

namespace TgOperator.Features.Players.Handlers
{
    public class EnterPortalCommandHandler : IRequestHandler<EnterPortalCommand, EnterPortalResponse>
    {
        private readonly IVendorRepository _vendorRepository;
        private readonly IHub _sentryHub;

        public EnterPortalCommandHandler(
            IVendorRepository vendorRepository,
            IHub sentryHub)
        {
            _vendorRepository = vendorRepository;
            _sentryHub = sentryHub;
        }

        public async Task<EnterPortalResponse> Handle(EnterPortalCommand command, CancellationToken cancellationToken)
        {
            var request = command.Request;
            var now = DateTime.UtcNow;

            try
            {
                var betLimit = await _vendorRepository.GetByVendorIdAsync(request.VendorId, cancellationToken);

                if (betLimit == null)
                {
                    throw new InvalidOperationException("Bet limit configuration not found");
                }

                return new EnterPortalResponse { 
                    TimeStamp = now,
                    Result = new EnterPortalResult
                    {
                        Seq = request.Seq,
                        ErrorCode = 0,
                        ErrorDescription = "Success",
                        MinBet = betLimit?.MinBet ?? 0,
                        MaxBet = betLimit?.MaxBet ?? decimal.MaxValue
                    }
                };
            }
            catch (Exception ex)
            {
                _sentryHub.CaptureException(ex, scope =>
                {
                    scope.SetExtra("enterPortalRequest", request);
                });

                return new EnterPortalResponse
                {
                    TimeStamp = now,
                    Result = new EnterPortalResult
                    {
                        Seq = request.Seq,
                        MinBet = 0,
                        MaxBet = 0,
                        ErrorCode = ErrorCode.Internal_Server_Error,
                        ErrorDescription = ex.Message,
                    }

                };
            }
        }
    }
} 