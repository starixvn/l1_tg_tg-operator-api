namespace TgOperator.Features.Players.Models
{
    public class EnterPortalRequest
    {
        public string PlayerId { get; set; }
        public string OperatorId { get; set; }
        public string SiteId { get; set; }
        public string CharacterName { get; set; }

        public string Seq { get; set; }

        public bool FirstLogin { get; set; }

        public string OperatorToken { get; set; }
        
        public DateTime TimeStamp { get; set; }
    }
} 