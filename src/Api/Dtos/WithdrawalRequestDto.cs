using System.ComponentModel.DataAnnotations;

namespace Api.Dtos;

public class WithdrawalRequestDto
{
    [Range(0.01, 1000000000, ErrorMessage = "Amount must be between 0.01 and 1000000000.")]
    public decimal Amount { get; set; }
}
