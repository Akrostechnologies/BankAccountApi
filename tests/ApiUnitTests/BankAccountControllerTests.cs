using System.ComponentModel.DataAnnotations;
using Api.Controllers;
using Api.Dtos;
using Domain.Exceptions;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace ApiUnitTests;

[TestClass]
public class BankAccountControllerTests
{
    [TestMethod]
    public async Task GetFoundReturnsOkWithDto()
    {
        var account = NewAccount();
        var service = new Mock<IBankAccountService>();
        service.Setup(x => x.GetBankAccountAsync("1234")).ReturnsAsync(account);
        var controller = CreateController(service);

        var result = await controller.Get("1234");

        var ok = result as OkObjectResult;
        Assert.IsNotNull(ok);
        Assert.IsInstanceOfType(ok.Value, typeof(BankAccountDto));
    }

    [TestMethod]
    public async Task GetMissingReturnsNotFound()
    {
        var service = new Mock<IBankAccountService>();
        service.Setup(x => x.GetBankAccountAsync("1234")).ReturnsAsync((BankAccount?)null);
        var controller = CreateController(service);

        var result = await controller.Get("1234");

        Assert.IsInstanceOfType(result, typeof(NotFoundResult));
    }

    [TestMethod]
    public async Task GetMalformedAccountNumberReturnsBadRequest()
    {
        var service = new Mock<IBankAccountService>();
        var controller = CreateController(service);

        var result = await controller.Get("12<34");

        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        service.Verify(x => x.GetBankAccountAsync(It.IsAny<string>()), Times.Never);
    }

    [TestMethod]
    public async Task DepositSuccessReturnsOk()
    {
        var service = new Mock<IBankAccountService>();
        service.Setup(x => x.DepositAsync("1234", 25m)).ReturnsAsync(new Transaction());
        service.Setup(x => x.GetBankAccountAsync("1234")).ReturnsAsync(NewAccount());
        var controller = CreateController(service);

        var result = await controller.Deposit("1234", new DepositRequestDto { Amount = 25m });

        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
    }

    [TestMethod]
    public async Task DepositInvalidTransactionReturnsBadRequest()
    {
        var service = new Mock<IBankAccountService>();
        service.Setup(x => x.DepositAsync("1234", 25m)).ThrowsAsync(new InvalidTransactionException("Insufficient funds."));
        var controller = CreateController(service);

        var result = await controller.Deposit("1234", new DepositRequestDto { Amount = 25m });

        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
    }

    [TestMethod]
    public async Task DepositUnknownAccountReturnsNotFound()
    {
        var service = new Mock<IBankAccountService>();
        service.Setup(x => x.DepositAsync("1234", 25m)).ThrowsAsync(new AccountNotFoundException("1234"));
        var controller = CreateController(service);

        var result = await controller.Deposit("1234", new DepositRequestDto { Amount = 25m });

        Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
    }

    [TestMethod]
    public async Task WithdrawSuccessReturnsOk()
    {
        var service = new Mock<IBankAccountService>();
        service.Setup(x => x.WithdrawAsync("1234", 25m)).ReturnsAsync(new Transaction());
        service.Setup(x => x.GetBankAccountAsync("1234")).ReturnsAsync(NewAccount());
        var controller = CreateController(service);

        var result = await controller.Withdraw("1234", new WithdrawalRequestDto { Amount = 25m });

        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
    }

    [TestMethod]
    public async Task WithdrawInsufficientFundsReturnsBadRequest()
    {
        var service = new Mock<IBankAccountService>();
        service.Setup(x => x.WithdrawAsync("1234", 25m)).ThrowsAsync(new InvalidTransactionException("Insufficient funds."));
        var controller = CreateController(service);

        var result = await controller.Withdraw("1234", new WithdrawalRequestDto { Amount = 25m });

        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
    }

    [TestMethod]
    public async Task TransferSuccessReturnsOk()
    {
        var service = new Mock<IBankAccountService>();
        service.Setup(x => x.TransferAsync("1234", "1235", 25m)).Returns(Task.CompletedTask);
        service.Setup(x => x.GetBankAccountAsync("1234")).ReturnsAsync(NewAccount());
        var controller = CreateController(service);

        var result = await controller.Transfer(new TransferRequestDto
        {
            SourceAccountNumber = "1234",
            DestinationAccountNumber = "1235",
            Amount = 25m
        });

        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
    }

    [TestMethod]
    public async Task TransferUnknownAccountReturnsNotFound()
    {
        var service = new Mock<IBankAccountService>();
        service.Setup(x => x.TransferAsync("1234", "1235", 25m)).ThrowsAsync(new AccountNotFoundException("1235"));
        var controller = CreateController(service);

        var result = await controller.Transfer(new TransferRequestDto
        {
            SourceAccountNumber = "1234",
            DestinationAccountNumber = "1235",
            Amount = 25m
        });

        Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
    }

    [TestMethod]
    public void RequestDtoValidationRejectsInvalidValues()
    {
        var deposit = new DepositRequestDto { Amount = 0 };
        var transfer = new TransferRequestDto
        {
            SourceAccountNumber = "12-34",
            DestinationAccountNumber = string.Empty,
            Amount = -1
        };

        Assert.IsFalse(Validator.TryValidateObject(deposit, new ValidationContext(deposit), new List<ValidationResult>(), true));
        Assert.IsFalse(Validator.TryValidateObject(transfer, new ValidationContext(transfer), new List<ValidationResult>(), true));
    }

    [TestMethod]
    public async Task ModelStateErrorReturnsBadRequest()
    {
        var service = new Mock<IBankAccountService>();
        var controller = CreateController(service);
        controller.ModelState.AddModelError("Amount", "Invalid amount");

        var result = await controller.Deposit("1234", new DepositRequestDto { Amount = 1m });

        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
    }

    private static BankAccountController CreateController(Mock<IBankAccountService> service)
    {
        return new BankAccountController(service.Object, NullLogger<BankAccountController>.Instance);
    }

    private static BankAccount NewAccount()
    {
        return new BankAccount { Id = 1, AccountNumber = "1234", AccountHolder = "Steve", AccountBalance = 100m };
    }
}
