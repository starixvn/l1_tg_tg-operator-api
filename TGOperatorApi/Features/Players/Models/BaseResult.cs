namespace TGOperatorApi.Features.Players.Models
{
    public class BaseResult
    {
        public string Seq { get; set; }

        public bool Success { get; set; }

        public int ErrorCode { get; set; }

        public string Message { get; set; }

        public string Description { get; set; }
    }
}
