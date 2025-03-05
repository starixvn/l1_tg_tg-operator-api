using TGOperatorApi.Features.Players.Models.Enums;

namespace TGOperatorApi.Features.Players.Models
{
    public class BaseResult
    {
        public string Seq { get; set; }

        public bool Success { get; set; }

        public ErrorCode ErrorCode { get; set; }

        public string ErrorMessage { get; set; }

        public string Description { get; set; }
    }
}
