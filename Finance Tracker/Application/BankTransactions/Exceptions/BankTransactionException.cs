using Application.Transactions.Exceptions;
using Domain.Banks;
using Domain.BankTransactions;
using Domain.Categorys;
using Domain.Transactions;
using Domain.Users;

namespace Application.BankTransactions.Exceptions;

public abstract class BankTransactionException : Exception
{
    public BankTransactionId? BankTransactionId { get; }
    public UserId? UserId { get; }
    public BankId? BankId { get; }

    protected BankTransactionException(BankTransactionId? bankTransactionId, string message, Exception? innerException = null)
        : base(message, innerException)
    {
        BankTransactionId = bankTransactionId;
    }

    protected BankTransactionException(UserId? userId, string message, Exception? innerException = null)
        : base(message, innerException)
    {
        UserId = userId;
    }

    protected BankTransactionException(BankId? bankId, string message, Exception? innerException = null)
        : base(message, innerException)
    {
        BankId = bankId;
    }
}
public class YouDoNotHaveTheAuthorityToDo(UserId idFromToken, UserId idFromUser) : BankTransactionException(idFromToken, $"You do not have the authority to do. Your id:{idFromToken} | Id what u want to change: {idFromUser}");
public class BankTransactionNotFoundException(BankTransactionId id)
    : BankTransactionException(id, $"BankTransaction under id: {id} not found");

public class UserNotFoundException(UserId id) : BankTransactionException(id, $"User under id: {id} not found");

public class BankTransactionAlreadyExistsException(BankTransactionId id)
    : BankTransactionException(id, $"Transaction already exists: {id}");

public class BankNotFoundException(BankId id) : BankTransactionException(id, $"Bank under id: {id} not found");

public class BankTransactionUnknownException(BankTransactionId id, Exception innerException)
    : BankTransactionException(id, $"Unknown exception for the Transaction under id: {id}", innerException);