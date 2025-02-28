using MediatR;
using TgOperator.Features.Wallets.Models;

namespace TgOperator.Features.Wallets.Commands
{
    public class DepositCommand : IRequest<WalletResponse>
    {
        public DepositRequest Request { get; }

        public DepositCommand(DepositRequest request)
        {
            Request = request;
        }
    }

    public class WithdrawCommand : IRequest<WalletResponse>
    {
        public WithdrawRequest Request { get; }

        public WithdrawCommand(WithdrawRequest request)
        {
            Request = request;
        }
    }
} 