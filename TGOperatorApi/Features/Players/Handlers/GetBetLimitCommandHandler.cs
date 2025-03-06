using MediatR;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TgOperator.Features.Players.Commands;
using TgOperator.Features.Players.Models;
using TgOperator.Features.Players.Repositories;
using TgOperator.Shared.Configuration;
using TgOperator.Shared.MongoDB;
using TGOperatorApi.Features.Players.Models;
using TGOperatorApi.Features.Players.Models.Enums;

namespace TgOperator.Features.Players.Handlers
{
    public class GetBetLimitCommandHandler : IRequestHandler<GetBetLimitCommand, BetLimitResponse>
    {
        private readonly IVendorRepository _vendorRepository;
        private readonly IHub _sentryHub;

        public GetBetLimitCommandHandler(
            IVendorRepository vendorRepository,
            IHub sentryHub)
        {
            _vendorRepository = vendorRepository;
            _sentryHub = sentryHub;
        }

        public async Task<BetLimitResponse> Handle(GetBetLimitCommand command, CancellationToken cancellationToken)
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

                var betLimitResult = new BetLimitResult
                {
                    Seq = request.Seq,
                    MaxBet = betLimit.MaxBet,
                    MinBet = betLimit.MinBet,
                    ErrorCode = 0
                };

                return new BetLimitResponse { 
                    TimeStamp = now,
                    Result = betLimitResult,
                };
            }
            catch (Exception ex)
            {
                _sentryHub.CaptureException(ex, scope =>
                {
                    scope.SetExtra("getBetLimitRequest", request);
                });

                return new BetLimitResponse
                {
                    TimeStamp = now,
                    Result = new BetLimitResult
                    {
                        Seq = request.Seq,
                        MinBet = 0,
                        MaxBet = 0,
                        ErrorCode = ErrorCode.Fail,
                        ErrorDescription = ex.Message
                    }
                };
            }
        }
    }
} 