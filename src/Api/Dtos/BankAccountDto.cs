using Domain.Models;

namespace Api.Dtos;

public class BankAccountDto
{
    public int Id { get; set; }
    public string AccountNumber { get; set; } = string.Empty;
    public string AccountHolder { get; set; } = string.Empty;
    public decimal AccountBalance { get; set; }
    public decimal InterestRate { get; set; }
    public DateTime LastInterestCalculationDate { get; set; }
    public decimal InterestEarned { get; set; }
    public IReadOnlyList<TransactionDto> Transactions { get; set; } = Array.Empty<TransactionDto>();

    public static BankAccountDto FromBankAccount(BankAccount bankAccount)
    {
        return new BankAccountDto
        {
            Id = bankAccount.Id,
            AccountNumber = bankAccount.AccountNumber,
            AccountHolder = bankAccount.AccountHolder,
            AccountBalance = bankAccount.AccountBalance,
            InterestRate = bankAccount.InterestRate,
            LastInterestCalculationDate = bankAccount.LastInterestCalculationDate,
            InterestEarned = bankAccount.Transactions
                .Where(transaction => transaction.Type == TransactionType.Interest)
                .Sum(transaction => transaction.Amount),
            Transactions = bankAccount.Transactions
                .OrderByDescending(transaction => transaction.Timestamp)
                .Select(TransactionDto.FromTransaction)
                .ToList()
        };
    }
}