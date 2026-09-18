using Domain.Models;

namespace Domain.Interfaces;

public interface IBankAccountService
{
    Task<BankAccount?> GetBankAccountAsync(string accountNumber);
    Task<Transaction> DepositAsync(string accountNumber, decimal amount);
    Task<Transaction> WithdrawAsync(string accountNumber, decimal amount);
    Task TransferAsync(string sourceAccountNumber, string destinationAccountNumber, decimal amount);
    Task CalculateDailyInterestAsync(CancellationToken cancellationToken = default);
}