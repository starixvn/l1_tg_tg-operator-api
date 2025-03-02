using System.ComponentModel.DataAnnotations;

namespace TgOperator.Features.Players.Models
{
    public class BetRequest
    {
        [Required]
        public decimal Amount { get; set; }
        
        [Required]
        public int ChoiceId { get; set; }
        
        public decimal ContributionAmount { get; set; }
        
        public decimal ContributionRate { get; set; }
        
        [Required]
        public string Currency { get; set; }
        
        public bool FreeBet { get; set; }
        
        public bool FreeSpin { get; set; }
        
        [Required]
        public long GameRoundId { get; set; }
        
        [Required]
        public int GameTypeId { get; set; }
        
        [Required]
        public string Ip { get; set; }
        
        [Required]
        public required string OperatorId { get; set; }
        
        [Required]
        public required string OperatorToken { get; set; }
        
        [Required]
        public long PlayerId { get; set; }
        
        public DateTime? RoundEndTime { get; set; }
        
        [Required]
        public string Seq { get; set; }
        
        [Required]
        public string SiteId { get; set; }
        
        public string? UserId { get; set; }
    }
} 