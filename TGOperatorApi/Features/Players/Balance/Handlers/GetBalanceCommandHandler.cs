using MediatR;
using TgOperator.Core.Features.Wallet;
using TgOperator.Features.Players.Balance.Commands;

namespace TgOperator.Features.Players.Balance.Handlers
{
    public class GetBalanceCommandHandler : IRequestHandler<GetBalanceCommand, GetBalanceCommand.Response>
    {
        private readonly IWalletService _walletService;

        public GetBalanceCommandHandler(IWalletService walletService)
        {
            _walletService = walletService;
        }

        public async Task<GetBalanceCommand.Response> Handle(GetBalanceCommand request, CancellationToken cancellationToken)
        {
            // TODO: Validate operator token
            // TODO: Validate site ID

            var walletResponse = await _walletService.GetBalanceAsync(request.PlayerId, cancellationToken);

            return new GetBalanceCommand.Response
            {
                TimeStamp = request.TimeStamp,
                Result = new GetBalanceCommand.Result
                {
                    Seq = request.Seq,
                    ErrorCode = string.IsNullOrEmpty(walletResponse.StatusCode) ? 0 : int.Parse(walletResponse.StatusCode),
                    ErrorDescription = string.IsNullOrEmpty(walletResponse.Message) ? "Success" : walletResponse.Message,
                    Balance = walletResponse.Balance
                }
            };
        }
    }
} 