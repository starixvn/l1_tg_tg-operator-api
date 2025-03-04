namespace TgOperator.Features.Players.Models
{
    public class VGSignInRequest
    {
        public Auth Auth { get; set; }
        public string SiteId { get; set; }
        public string PlayerId { get; set; }
        public string PlayerName { get; set; }
        public string Currency { get; set; }
        public int GameTypeId { get; set; }
        public string IP { get; set; }
        public string BrowserUserAgent { get; set; }
        public string Domain { get; set; }
        public string Location { get; set; }
        public string SiteName { get; set; }
        public bool IsCash { get; set; }
        public string DisplayCurrency { get; set; }
        public int AppType { get; set; }
        public bool DisableJackpotGames { get; set; }
        public bool DisableAward { get; set; }
        public string OperatorToken { get; set; }
        public string Ref { get; set; }
        public string ReturnUrl { get; set; }
        public string GameId { get; set; }
    }

    public class Auth
    {
        public string OperatorId { get; set; }
    }
} 