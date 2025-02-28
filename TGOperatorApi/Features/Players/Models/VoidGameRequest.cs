using System.ComponentModel.DataAnnotations;

namespace TgOperator.Features.Players.Models
{
    public class VoidGameRequest
    {
        public string OperatorId { get; set; }

        public long PlayerId { get; set; }

        public string SiteId { get; set; }

        public int GameRoundId { get; set; }

        public byte GameTypeId { get; set; }

        public string Reason { get; set; }

        public bool FreeBet { get; set; }
    }
} 