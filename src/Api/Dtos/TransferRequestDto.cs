using System.ComponentModel.DataAnnotations;

namespace Api.Dtos;

public class TransferRequestDto
{
    [Required]
    [StringLength(34, MinimumLength = 1)]
    [RegularExpression("^[A-Za-z0-9]+$")]
    public string SourceAccountNumber { get; set; } = string.Empty;

    [Required]
    [StringLength(34, MinimumLength = 1)]
    [RegularExpression("^[A-Za-z0-9]+$")]
    public string DestinationAccountNumber { get; set; } = string.Empty;

    [Range(0.01, 1000000000, ErrorMessage = "Amount must be between 0.01 and 1000000000.")]
    public decimal Amount { get; set; }
}
