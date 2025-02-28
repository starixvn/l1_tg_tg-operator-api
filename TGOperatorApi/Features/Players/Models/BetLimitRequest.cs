namespace TgOperator.Features.Players.Models
{
    public class BetLimitRequest
    {
        public string PlayerId { get; set; }
        public string OperatorId { get; set; }
        public string SiteId { get; set; }
        public string GameTypeId { get; set; }
        public string Currency { get; set; }
    }
} 