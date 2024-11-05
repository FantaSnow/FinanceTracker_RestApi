using Application.Users.Exceptions;
using Domain.Users;

namespace Application.Tickets.Exceptions;

public abstract class TokenException : Exception
{
    public string? Login { get; }

    protected TokenException(string login, string message, Exception? innerException = null)
        : base(message, innerException)
    {
        Login = login;
    }

    protected TokenException(string message, Exception? innerException = null)
        : base(message, innerException)
    {
        
    }
}


public class LoginException() : TokenException($"Invalid login or password");

public class TicketUnknownException(string login,Exception innerException)
    : TokenException(login, $"Unknown exception for the ticket under login: {login}", innerException);