using System.ComponentModel.DataAnnotations;

namespace TgOperator.Features.Players.Models
{
    public class PlayerBalanceRequest
    {
        [Required]
        public string OperatorId { get; set; }

        [Required]
        public string PlayerId { get; set; }

        [Required]
        public string SiteId { get; set; }

        [Required]
        public string OperatorToken { get; set; }
    }
} 