namespace TGOperatorApi.Features.Players.Models
{
    public class BaseResult
    {
        public string Seq { get; set; }

        public bool Success { get; set; }

        public int ErrorCode { get; set; }

        public string ErrorMessage { get; set; }

        public string ErrorDescription { get; set; }
    }
}
