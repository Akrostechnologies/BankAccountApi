using Domain.Models;

namespace Api.Dtos;

public class TransactionDto
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public string Type { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string? Description { get; set; }

    public static TransactionDto FromTransaction(Transaction transaction)
    {
        return new TransactionDto
        {
            Id = transaction.Id,
            Amount = transaction.Amount,
            Type = transaction.Type.ToString(),
            Timestamp = transaction.Timestamp,
            Description = transaction.Description
        };
    }
}
