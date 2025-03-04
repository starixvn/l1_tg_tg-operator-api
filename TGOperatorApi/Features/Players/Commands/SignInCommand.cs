using MediatR;
using TgOperator.Features.Players.Models;

namespace TgOperator.Features.Players.Commands
{
    public class SignInCommand : IRequest<SignInResult>
    {
        public SignInRequest SignInRequest { get; }

        public SignInCommand(SignInRequest signInRequest)
        {
            SignInRequest = signInRequest;
        }
    }
} 