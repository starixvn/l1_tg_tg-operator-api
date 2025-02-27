using System.ComponentModel.DataAnnotations;

namespace TgOperator.Features.Players.Models
{
    public class EndRoundRequest
    {
        public DateTime EndTime { get; set; }
        public bool FreeBet { get; set; }
        
        [Required]
        public long GameRoundId { get; set; }
        
        [Required]
        public int GameTypeId { get; set; }
        
        [Required]
        public long PlayerId { get; set; }
        
        [Required]
        public string Seq { get; set; }
        
        [Required]
        public string SiteId { get; set; }
        
        [Required]
        public decimal TotalAmount { get; set; }
        
        [Required]
        public decimal TotalWin { get; set; }
        
        [Required]
        public decimal ValidBetAmount { get; set; }
    }
} 