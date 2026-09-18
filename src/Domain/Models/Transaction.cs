using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models;

public enum TransactionType
{
    Deposit,
    Withdrawal,
    TransferIn,
    TransferOut,
    Interest
}

public class Transaction
{
    public int Id { get; set; }
    public int BankAccountId { get; set; }
    public BankAccount? BankAccount { get; set; }
    [Column(TypeName = "decimal(16,4)")]
    public decimal Amount { get; set; }
    public TransactionType Type { get; set; }
    public DateTime Timestamp { get; set; }
    public string? Description { get; set; }
}
