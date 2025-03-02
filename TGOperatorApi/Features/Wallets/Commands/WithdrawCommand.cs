using MediatR;
using TgOperator.Features.Wallets.Models;

namespace TgOperator.Features.Wallets.Commands
{
    public record WithdrawCommand(WithdrawRequest Request) : IRequest<WalletResponse>;
} 