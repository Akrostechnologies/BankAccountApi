using Domain.Exceptions;
using Domain.Interfaces;
using Domain.Models;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class BankAccountService : IBankAccountService
{
    private readonly BankDbContext _dbContext;

    public BankAccountService(BankDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<BankAccount?> GetBankAccountAsync(string accountNumber)
    {
        return _dbContext.BankAccount
            .Include(a => a.Transactions)
            .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);
    }

    public async Task<Transaction> DepositAsync(string accountNumber, decimal amount)
    {
        ValidateAmount(amount);
        var account = await GetRequiredAccountAsync(accountNumber);
        var roundedAmount = RoundAmount(amount);
        account.AccountBalance += roundedAmount;
        var transaction = new Transaction
        {
            BankAccount = account,
            Amount = roundedAmount,
            Type = TransactionType.Deposit,
            Timestamp = DateTime.UtcNow,
            Description = "Deposit"
        };
        _dbContext.Transaction.Add(transaction);
        await _dbContext.SaveChangesAsync();
        return transaction;
    }

    public async Task<Transaction> WithdrawAsync(string accountNumber, decimal amount)
    {
        ValidateAmount(amount);
        var account = await GetRequiredAccountAsync(accountNumber);
        var roundedAmount = RoundAmount(amount);
        if (account.AccountBalance < roundedAmount)
        {
            throw new InvalidTransactionException("Insufficient funds.");
        }

        account.AccountBalance -= roundedAmount;
        var transaction = new Transaction
        {
            BankAccount = account,
            Amount = roundedAmount,
            Type = TransactionType.Withdrawal,
            Timestamp = DateTime.UtcNow,
            Description = "Withdrawal"
        };
        _dbContext.Transaction.Add(transaction);
        await _dbContext.SaveChangesAsync();
        return transaction;
    }

    public async Task TransferAsync(string sourceAccountNumber, string destinationAccountNumber, decimal amount)
    {
        ValidateAmount(amount);
        if (sourceAccountNumber == destinationAccountNumber)
        {
            throw new InvalidTransactionException("Source and destination accounts must be different.");
        }

        var source = await GetRequiredAccountAsync(sourceAccountNumber);
        var destination = await GetRequiredAccountAsync(destinationAccountNumber);
        var roundedAmount = RoundAmount(amount);
        if (source.AccountBalance < roundedAmount)
        {
            throw new InvalidTransactionException("Insufficient funds.");
        }

        source.AccountBalance -= roundedAmount;
        destination.AccountBalance += roundedAmount;
        _dbContext.Transaction.AddRange(
            new Transaction
            {
                BankAccount = source,
                Amount = roundedAmount,
                Type = TransactionType.TransferOut,
                Timestamp = DateTime.UtcNow,
                Description = $"Transfer to {destinationAccountNumber}"
            },
            new Transaction
            {
                BankAccount = destination,
                Amount = roundedAmount,
                Type = TransactionType.TransferIn,
                Timestamp = DateTime.UtcNow,
                Description = $"Transfer from {sourceAccountNumber}"
            });
        await _dbContext.SaveChangesAsync();
    }

    public async Task CalculateDailyInterestAsync(CancellationToken cancellationToken = default)
    {
        var accounts = await _dbContext.BankAccount.ToListAsync(cancellationToken);
        var today = DateTime.UtcNow.Date;
        foreach (var account in accounts)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var days = (int)(today - account.LastInterestCalculationDate.Date).TotalDays;
            if (days <= 0)
            {
                continue;
            }

            if (account.AccountBalance > 0)
            {
                var interest = Math.Round(
                    account.AccountBalance * account.InterestRate / 365m * days,
                    2,
                    MidpointRounding.AwayFromZero);
                if (interest > 0)
                {
                    account.AccountBalance += interest;
                    _dbContext.Transaction.Add(new Transaction
                    {
                        BankAccount = account,
                        Amount = interest,
                        Type = TransactionType.Interest,
                        Timestamp = DateTime.UtcNow,
                        Description = $"Interest for {days} day(s)"
                    });
                }
            }

            account.LastInterestCalculationDate = today;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task<BankAccount> GetRequiredAccountAsync(string accountNumber)
    {
        var account = await _dbContext.BankAccount
            .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);
        return account ?? throw new AccountNotFoundException(accountNumber);
    }

    private static void ValidateAmount(decimal amount)
    {
        if (amount <= 0)
        {
            throw new InvalidTransactionException("Amount must be greater than zero.");
        }
    }

    private static decimal RoundAmount(decimal amount)
    {
        return Math.Round(amount, 2, MidpointRounding.AwayFromZero);
    }
}