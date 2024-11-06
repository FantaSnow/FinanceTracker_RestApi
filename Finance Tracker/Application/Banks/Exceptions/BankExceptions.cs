using Domain.Banks;
using Domain.Users;

namespace Application.Banks.Exceptions;

public abstract class BankException : Exception
{
    public BankId? BankId { get; }
    public UserId? UserId { get; }

    protected BankException(BankId? bankId, string message, Exception? innerException = null)
        : base(message, innerException)
    {
        BankId = bankId;
    }

    protected BankException(UserId? userId, string message, Exception? innerException = null)
        : base(message, innerException)
    {
        UserId = userId;
    }
}

public class BankNotFoundException(BankId id) : BankException(id, $"Bank under id: {id} not found");

public class UserNotFoundException(UserId id) : BankException(id, $"User under id: {id} not found");

public class BankAlreadyExistsException(BankId id) : BankException(id, $"Bank already exists: {id}");

public class YouDoNotHaveTheAuthorityToDo(UserId idFromToken, UserId idFromUser) : BankException(idFromToken, $"You do not have the authority to do. Your id:{idFromToken} | Id what u want to change: {idFromUser}");

public class BankUnknownException(BankId id, Exception innerException)
    : BankException(id, $"Unknown exception for the Bank under id: {id}", innerException);