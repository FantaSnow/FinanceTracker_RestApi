using Application.Common;
using Application.Users.Exceptions;
using Domain.Banks;
using Domain.Users;

namespace Application.Banks.Exceptions;

public abstract class BankException : Exception
{
    public BankId? BankId { get; }
    protected BankException(BankId? bankId, string message, Exception? innerException = null)
        : base(message, innerException)
    {
        BankId = bankId;
    }
}

public class BankNotFoundException(BankId id) : BankException(id, $"Bank under id: {id} not found");

public class BankAlreadyExistsException(BankId id) : BankException(id, $"Bank already exists: {id}");

public class BankUnknownException(BankId id, Exception innerException)
    : BankException(id, $"Unknown exception for the Bank under id: {id}", innerException);

public class BankUpdateFailedException : BankException
{
    public UserException UserException { get; }
    public BankUpdateFailedException(BankId? bankId, UserException userException)
        : base(bankId, $"Failed to update the balance for the bank with id: {bankId} due to a user-related error.", userException)
    {
        UserException = userException;
    }
}