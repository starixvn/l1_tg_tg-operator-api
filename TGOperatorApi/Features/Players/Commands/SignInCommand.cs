using MediatR;
using TgOperator.Features.Players.Models;
using TGOperatorApi.Features.Players.Models;

namespace TgOperator.Features.Players.Commands
{
    public record SignInCommand(SignInRequest Request) : IRequest<SignInResponse>;

    public class SignInRequest
    {
        public required DateTime TimeStamp { get; set; }
        public required string Seq { get; set; }
        public required string PlayerId { get; set; }
        public required string LangId { get; set; }
        public required string CurrencyId { get; set; }
        public required Dictionary<string, string> Inputs { set; get; }
    }

    public class SignInResponse
    {
        public required DateTime TimeStamp { get; set; }
        public required SignInResult Result { get; set; }
    }

    public class SignInResult : BaseResult
    {
        public string RedirectUrl { get; set; }
        public string Token { get; set; }
    }
} 