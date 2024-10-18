
using Domain.Transactions;
using Domain.Users;
using Optional;

namespace Application.Common.Interfaces.Queries;

public interface ITransactionQueries
{
    Task<Option<IReadOnlyList<Transaction>>> GetAll(CancellationToken cancellationToken);
    Task<Option<IReadOnlyList<Transaction>>> GetAllByUser(UserId userId, CancellationToken cancellationToken);
    Task<Option<Transaction>> GetById(TransactionId id, CancellationToken cancellationToken);
}