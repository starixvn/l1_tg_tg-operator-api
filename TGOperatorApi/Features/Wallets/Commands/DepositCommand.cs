using MediatR;
using TgOperator.Features.Wallets.Models;

namespace TgOperator.Features.Wallets.Commands
{
    public record DepositCommand(DepositRequest Request) : IRequest<WalletResponse>;
} 