using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models;

public class BankAccount
{
    public int Id { get; set; }
    public string AccountNumber { get; set; } = string.Empty;
    public string AccountHolder { get; set; } = string.Empty;
    [Column(TypeName = "decimal(16,4)")]
    public decimal AccountBalance { get; set; }
    [Column(TypeName = "decimal(9,6)")]
    public decimal InterestRate { get; set; }
    public DateTime LastInterestCalculationDate { get; set; }
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}