using TGOperatorApi.Features.Players.Models.Enums;

namespace TGOperatorApi.Features.Players.Models
{
    public class BaseResult
    {
        public required string Seq { get; set; }

        public ErrorCode ErrorCode { get; set; }

        public string ErrorMessage { get; set; }

        public string ErrorDescription { get; set; }
    }
}
