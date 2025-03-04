using TGOperatorApi.Features.Players.Models;

namespace TgOperator.Features.Players.Models
{
    public class SignInResult : BaseResult
    {
        public string RedirectUrl { get; set; }
        public string Token { get; set; }
    }
} 