using Domain.Models;

namespace Infrastructure;

public class SeedData
{
    public static void Seed(BankDbContext context)
    {
        if (!context.BankAccount.Any())
        {
            var yesterday = DateTime.UtcNow.Date.AddDays(-1);
            var account1 = new BankAccount
            {
                Id = 1,
                AccountNumber = "1234",
                AccountHolder = "Steve",
                AccountBalance = 12345.12m,
                InterestRate = 0.05m,
                LastInterestCalculationDate = yesterday,
                Transactions = new List<Transaction>
                {
                    new() { Id = 1, Amount = 1000m, Type = TransactionType.Deposit, Timestamp = yesterday.AddDays(-2), Description = "Initial deposit" },
                    new() { Id = 2, Amount = 25.50m, Type = TransactionType.Withdrawal, Timestamp = yesterday.AddDays(-1), Description = "ATM withdrawal" }
                }
            };
            var account2 = new BankAccount
            {
                Id = 2,
                AccountNumber = "1235",
                AccountHolder = "Jim Halpert",
                AccountBalance = 123.10m,
                InterestRate = 0.025m,
                LastInterestCalculationDate = yesterday,
                Transactions = new List<Transaction>
                {
                    new() { Id = 3, Amount = 200m, Type = TransactionType.Deposit, Timestamp = yesterday.AddDays(-3), Description = "Initial deposit" },
                    new() { Id = 4, Amount = 76.90m, Type = TransactionType.Withdrawal, Timestamp = yesterday.AddDays(-2), Description = "Office supplies" }
                }
            };

            context.BankAccount.AddRange(account1, account2);
            context.SaveChanges();
        }
    }
}