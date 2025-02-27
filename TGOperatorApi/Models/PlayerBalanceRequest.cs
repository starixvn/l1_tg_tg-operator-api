using System.ComponentModel.DataAnnotations;

public class PlayerBalanceRequest
{
    /// <summary>
    /// The unique identifier for the operator
    /// </summary>
    [Required]
    public string OperatorId { get; set; }

    /// <summary>
    /// The unique identifier for the player
    /// </summary>
    [Required]
    public string PlayerId { get; set; }

    /// <summary>
    /// The site identifier
    /// </summary>
    [Required]
    public string SiteId { get; set; }

    /// <summary>
    /// Authentication token for the operator
    /// </summary>
    [Required]
    public string OperatorToken { get; set; }
} 