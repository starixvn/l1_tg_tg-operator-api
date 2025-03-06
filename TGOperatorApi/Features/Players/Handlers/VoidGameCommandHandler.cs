using MediatR;
using MongoDB.Driver;
using TgOperator.Features.Players.Commands;
using TgOperator.Features.Players.Models;
using TgOperator.Shared.MongoDB;
using Microsoft.Extensions.Options;
using TgOperator.Shared.Configuration;
using Sentry;
using TgOperator.Features.Players.Repositories;
using System.Transactions;
using TGOperatorApi.Features.Players.Models.Enums;

namespace TgOperator.Features.Players.Handlers
{
    public class VoidGameCommandHandler : IRequestHandler<VoidGameCommand, VoidGameResponse>
    {
        private readonly IBetTransactionRepository _betTransactionRepository;
        private readonly IHub _sentryHub;

        public VoidGameCommandHandler(
            IBetTransactionRepository betTransactionRepository,
            IHub sentryHub)
        {
            _betTransactionRepository = betTransactionRepository;
            _sentryHub = sentryHub;
        }

        public async Task<VoidGameResponse> Handle(VoidGameCommand command, CancellationToken cancellationToken)
        {
            var request = command.Request;
            var now = DateTime.UtcNow;

            try
            {
                var betTransaction = await _betTransactionRepository.
                    GetByGameRoundAsync(request.GameRoundId, request.PlayerId, request.GameTypeId, cancellationToken);

                if (betTransaction == null)
                {
                    throw new InvalidOperationException("Bet transaction not found");
                }

                //TODO update balance player
                
                var transactionId = Guid.NewGuid().ToString();
              
                //TODO update transaction 

                return new VoidGameResponse
                {
                    TimeStamp = now,
                    Result = new VoidGameResult
                    {
                        Seq = request.Seq,
                        PlayerId = request.PlayerId,
                        ErrorCode = 0,
                        ErrorDescription = "Success"
                    }
                };
            }
            catch (Exception ex)
            {
                _sentryHub.CaptureException(ex, scope =>
                {
                    scope.SetExtra("voidGameRequest", request);
                });

                return new VoidGameResponse
                {
                    TimeStamp = now,
                    Result = new VoidGameResult
                    {
                        Seq = request.Seq,
                        ErrorCode = ErrorCode.Fail,
                        ErrorDescription = ex.Message
                    }
                };
            }
        }
    }
} 