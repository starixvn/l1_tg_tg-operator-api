namespace TGOperatorApi.Features.Players.Models
{
    public class LoginResp
    {
        public string Seq { get; set; }
        public DateTime TimeStamp { get; set; }
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public Payload_Login Payload { get; set; }
    }

    public class Payload_Login
    {
        public string ReturnUrl { get; set; }
    }

    public class VendorLoginSettings
    {
        public string ApiUrl { get; set; } = "https://l2-api.lumigame.com/SignIn/Index";
        public string OperatorId { get; set; }
        public string OperatorSecret { get; set; }
        public string OperatorToken { get; set; }
        public string SiteId { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
}
