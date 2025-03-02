namespace TgOperator.Features.Wallets.Models
{
    using System.ComponentModel.DataAnnotations;

    public class DepositRequest
    {
        [Required]
        public string PlayerId { get; set; }
        [Required]
        public string VendorId { get; set; }
        [Required]
        public string SiteId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentReference { get; set; }
    }

    public class WithdrawRequest
    {
        [Required]
        public string PlayerId { get; set; }
        [Required]
        public string VendorId { get; set; }
        [Required]
        public string SiteId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentReference { get; set; }
    }
} 