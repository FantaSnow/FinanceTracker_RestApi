using Domain.Categorys;
using Domain.Transactions;
using Domain.Users;

namespace Application.Transactions.Exceptions;

public abstract class TransactionException : Exception
{
    public TransactionId? TransactionId { get; }
    public UserId? UserId { get; }
    public CategoryId? CategoryId { get; }

    protected TransactionException(TransactionId? transactionId, string message, Exception? innerException = null)
        : base(message, innerException)
    {
        TransactionId = transactionId;
    }

    protected TransactionException(UserId? userId, string message, Exception? innerException = null)
        : base(message, innerException)
    {
        UserId = userId;
    }

    protected TransactionException(CategoryId? categoryId, string message, Exception? innerException = null)
        : base(message, innerException)
    {
        CategoryId = categoryId;
    }
}
public class YouDoNotHaveTheAuthorityToDo(UserId idFromToken, UserId idFromUser) : TransactionException(idFromToken, $"You do not have the authority to do. Your id:{idFromToken} | Id what u want to change: {idFromUser}");
public class TransactionNotFoundException(TransactionId id)
    : TransactionException(id, $"Transaction under id: {id} not found");

public class UserNotFoundException(UserId id) : TransactionException(id, $"User under id: {id} not found");

public class TransactionAlreadyExistsException(TransactionId id)
    : TransactionException(id, $"Transaction already exists: {id}");

public class CategoryNotFoundException(CategoryId id) : TransactionException(id, $"Category under id: {id} not found");

public class TransactionUnknownException(TransactionId id, Exception innerException)
    : TransactionException(id, $"Unknown exception for the Transaction under id: {id}", innerException);