using System.Text.RegularExpressions;
using Api.Dtos;
using Domain.Exceptions;
using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class BankAccountController : ControllerBase
{
    private static readonly Regex AccountNumberPattern = new("^[A-Za-z0-9]{1,34}$");
    private readonly IBankAccountService _bankAccountService;
    private readonly ILogger<BankAccountController> _logger;

    public BankAccountController(
        IBankAccountService bankAccountService,
        ILogger<BankAccountController> logger)
    {
        _bankAccountService = bankAccountService;
        _logger = logger;
    }

    [HttpGet("{accountNumber}")]
    public async Task<IActionResult> Get(string accountNumber)
    {
        if (!IsValidAccountNumber(accountNumber))
        {
            return BadRequest(new { error = "Account number is malformed." });
        }

        var account = await _bankAccountService.GetBankAccountAsync(accountNumber);
        return account is null
            ? NotFound()
            : Ok(BankAccountDto.FromBankAccount(account));
    }

    [HttpPost("{accountNumber}/deposit")]
    public async Task<IActionResult> Deposit(string accountNumber, [FromBody] DepositRequestDto request)
    {
        if (!IsValidAccountNumber(accountNumber))
        {
            return BadRequest(new { error = "Account number is malformed." });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            await _bankAccountService.DepositAsync(accountNumber, request.Amount);
            return await GetAccountResponse(accountNumber);
        }
        catch (InvalidTransactionException exception)
        {
            _logger.LogWarning(exception, "Invalid deposit attempted.");
            return BadRequest(new { error = exception.Message });
        }
        catch (AccountNotFoundException exception)
        {
            return NotFound(new { error = exception.Message });
        }
    }

    [HttpPost("{accountNumber}/withdraw")]
    public async Task<IActionResult> Withdraw(string accountNumber, [FromBody] WithdrawalRequestDto request)
    {
        if (!IsValidAccountNumber(accountNumber))
        {
            return BadRequest(new { error = "Account number is malformed." });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            await _bankAccountService.WithdrawAsync(accountNumber, request.Amount);
            return await GetAccountResponse(accountNumber);
        }
        catch (InvalidTransactionException exception)
        {
            _logger.LogWarning(exception, "Invalid withdrawal attempted.");
            return BadRequest(new { error = exception.Message });
        }
        catch (AccountNotFoundException exception)
        {
            return NotFound(new { error = exception.Message });
        }
    }

    [HttpPost("transfer")]
    public async Task<IActionResult> Transfer([FromBody] TransferRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            await _bankAccountService.TransferAsync(
                request.SourceAccountNumber,
                request.DestinationAccountNumber,
                request.Amount);
            return await GetAccountResponse(request.SourceAccountNumber);
        }
        catch (InvalidTransactionException exception)
        {
            _logger.LogWarning(exception, "Invalid transfer attempted.");
            return BadRequest(new { error = exception.Message });
        }
        catch (AccountNotFoundException exception)
        {
            return NotFound(new { error = exception.Message });
        }
    }

    private async Task<IActionResult> GetAccountResponse(string accountNumber)
    {
        var account = await _bankAccountService.GetBankAccountAsync(accountNumber);
        return account is null
            ? NotFound()
            : Ok(BankAccountDto.FromBankAccount(account));
    }

    private static bool IsValidAccountNumber(string accountNumber)
    {
        return !string.IsNullOrEmpty(accountNumber) && AccountNumberPattern.IsMatch(accountNumber);
    }
}