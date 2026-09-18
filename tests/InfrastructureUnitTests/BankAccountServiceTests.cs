using Application.Services;
using Domain.Exceptions;
using Domain.Models;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureUnitTests;

[TestClass]
public class BankAccountServiceTests
{
    [TestMethod]
    public async Task DepositIncreasesBalanceAndRecordsTransaction()
    {
        await using var context = CreateContext();
        context.BankAccount.Add(new BankAccount { AccountNumber = "1234", AccountHolder = "Steve", AccountBalance = 100m });
        await context.SaveChangesAsync();
        var service = new BankAccountService(context);

        var transaction = await service.DepositAsync("1234", 25.126m);
        var account = await context.BankAccount.Include(a => a.Transactions).SingleAsync();

        Assert.AreEqual(125.13m, account.AccountBalance);
        Assert.AreEqual(TransactionType.Deposit, transaction.Type);
        Assert.AreEqual(25.13m, transaction.Amount);
        Assert.AreEqual(1, account.Transactions.Count);
    }

    [TestMethod]
    public async Task DepositRejectsZeroAmount()
    {
        await AssertDepositRejectsAmount(0m);
    }

    [TestMethod]
    public async Task DepositRejectsNegativeAmount()
    {
        await AssertDepositRejectsAmount(-1m);
    }

    private static async Task AssertDepositRejectsAmount(decimal amount)
    {
        await using var context = CreateContext();
        context.BankAccount.Add(new BankAccount { AccountNumber = "1234", AccountBalance = 100m });
        await context.SaveChangesAsync();
        var service = new BankAccountService(context);

        await Assert.ThrowsExceptionAsync<InvalidTransactionException>(() => service.DepositAsync("1234", amount));
    }

    [TestMethod]
    public async Task WithdrawalSucceeds()
    {
        await using var context = CreateContext();
        context.BankAccount.Add(new BankAccount { AccountNumber = "1234", AccountBalance = 100m });
        await context.SaveChangesAsync();
        var service = new BankAccountService(context);

        await service.WithdrawAsync("1234", 25m);

        Assert.AreEqual(75m, (await context.BankAccount.SingleAsync()).AccountBalance);
    }

    [TestMethod]
    public async Task WithdrawalExceedingBalanceLeavesBalanceUnchanged()
    {
        await using var context = CreateContext();
        context.BankAccount.Add(new BankAccount { AccountNumber = "1234", AccountBalance = 100m });
        await context.SaveChangesAsync();
        var service = new BankAccountService(context);

        await Assert.ThrowsExceptionAsync<InvalidTransactionException>(() => service.WithdrawAsync("1234", 101m));

        Assert.AreEqual(100m, (await context.BankAccount.SingleAsync()).AccountBalance);
        Assert.AreEqual(0, await context.Transaction.CountAsync());
    }

    [TestMethod]
    public async Task WithdrawalOnUnknownAccountThrows()
    {
        await using var context = CreateContext();
        var service = new BankAccountService(context);

        await Assert.ThrowsExceptionAsync<AccountNotFoundException>(() => service.WithdrawAsync("unknown", 1m));
    }

    [TestMethod]
    public async Task TransferMovesFundsAndRecordsBothTransactions()
    {
        await using var context = CreateContext();
        context.BankAccount.AddRange(
            new BankAccount { AccountNumber = "1234", AccountBalance = 100m },
            new BankAccount { AccountNumber = "1235", AccountBalance = 10m });
        await context.SaveChangesAsync();
        var service = new BankAccountService(context);

        await service.TransferAsync("1234", "1235", 25m);

        Assert.AreEqual(75m, (await context.BankAccount.SingleAsync(a => a.AccountNumber == "1234")).AccountBalance);
        Assert.AreEqual(35m, (await context.BankAccount.SingleAsync(a => a.AccountNumber == "1235")).AccountBalance);
        Assert.AreEqual(1, await context.Transaction.CountAsync(t => t.Type == TransactionType.TransferOut));
        Assert.AreEqual(1, await context.Transaction.CountAsync(t => t.Type == TransactionType.TransferIn));
    }

    [TestMethod]
    public async Task TransferToUnknownDestinationLeavesSourceUnchanged()
    {
        await using var context = CreateContext();
        context.BankAccount.Add(new BankAccount { AccountNumber = "1234", AccountBalance = 100m });
        await context.SaveChangesAsync();
        var service = new BankAccountService(context);

        await Assert.ThrowsExceptionAsync<AccountNotFoundException>(() => service.TransferAsync("1234", "unknown", 25m));

        Assert.AreEqual(100m, (await context.BankAccount.SingleAsync()).AccountBalance);
        Assert.AreEqual(0, await context.Transaction.CountAsync());
    }

    [TestMethod]
    public async Task TransferToSelfThrows()
    {
        await using var context = CreateContext();
        context.BankAccount.Add(new BankAccount { AccountNumber = "1234", AccountBalance = 100m });
        await context.SaveChangesAsync();
        var service = new BankAccountService(context);

        await Assert.ThrowsExceptionAsync<InvalidTransactionException>(() => service.TransferAsync("1234", "1234", 25m));
    }

    [TestMethod]
    public async Task DailyInterestCreditsInterestAndUpdatesDate()
    {
        await using var context = CreateContext();
        context.BankAccount.Add(new BankAccount
        {
            AccountNumber = "1234",
            AccountBalance = 1000m,
            InterestRate = 0.05m,
            LastInterestCalculationDate = DateTime.UtcNow.Date.AddDays(-10)
        });
        await context.SaveChangesAsync();
        var service = new BankAccountService(context);
        var expectedInterest = Math.Round(1000m * 0.05m / 365m * 10, 2, MidpointRounding.AwayFromZero);

        await service.CalculateDailyInterestAsync();

        var result = await context.BankAccount.Include(a => a.Transactions).SingleAsync();
        Assert.AreEqual(1000m + expectedInterest, result.AccountBalance);
        Assert.AreEqual(expectedInterest, result.Transactions.Single().Amount);
        Assert.AreEqual(TransactionType.Interest, result.Transactions.Single().Type);
        Assert.AreEqual(DateTime.UtcNow.Date, result.LastInterestCalculationDate);
    }

    [TestMethod]
    public async Task DailyInterestSecondImmediateCallIsNoOp()
    {
        await using var context = CreateContext();
        context.BankAccount.Add(new BankAccount
        {
            AccountNumber = "1234",
            AccountBalance = 1000m,
            InterestRate = 0.05m,
            LastInterestCalculationDate = DateTime.UtcNow.Date.AddDays(-10)
        });
        await context.SaveChangesAsync();
        var service = new BankAccountService(context);

        await service.CalculateDailyInterestAsync();
        var balanceAfterFirstCall = (await context.BankAccount.SingleAsync()).AccountBalance;
        await service.CalculateDailyInterestAsync();

        Assert.AreEqual(balanceAfterFirstCall, (await context.BankAccount.SingleAsync()).AccountBalance);
        Assert.AreEqual(1, await context.Transaction.CountAsync(t => t.Type == TransactionType.Interest));
    }

    private static BankDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<BankDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new BankDbContext(options);
    }
}
