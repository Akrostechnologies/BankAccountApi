namespace Domain.Exceptions;

public class AccountNotFoundException : Exception
{
    public AccountNotFoundException(string accountNumber)
        : base($"Account '{accountNumber}' was not found.")
    {
    }
}
