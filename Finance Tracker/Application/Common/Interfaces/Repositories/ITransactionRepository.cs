

using Domain.Transactions;
using Domain.Users;
using Optional;

namespace Application.Common.Interfaces.Repositories;

public interface ITransactionRepository
{
    Task<Transaction> Add(Transaction transaction, CancellationToken cancellationToken);
    Task<Transaction> Update(Transaction transaction, CancellationToken cancellationToken);
    Task<Transaction> Delete(Transaction transaction, CancellationToken cancellationToken);
    Task<Option<IReadOnlyList<Transaction>>> GetAllByUser(UserId userId, CancellationToken cancellationToken);
    Task<Option<Transaction>> GetById(TransactionId id, CancellationToken cancellationToken);

}