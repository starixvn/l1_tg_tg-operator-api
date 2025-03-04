namespace TgOperator.Features.Players.Models
{
    public class SignInRequest
    {
        public DateTime TimeStamp { get; set; }
        public string Seq { get; set; }
        public string PlayerId { get; set; }
        public string LangId { get; set; }
        public string CurrencyId { get; set; }
        public Dictionary<string, string> Inputs { set; get; }
    }
} 