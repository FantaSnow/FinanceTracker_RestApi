

using Domain.Transactions;

namespace Application.Common.Interfaces.Repositories;

public interface ITransactionRepository
{
    Task<Transaction> Add(Transaction transaction, CancellationToken cancellationToken);
    Task<Transaction> Update(Transaction transaction, CancellationToken cancellationToken);
    Task<Transaction> Delete(Transaction transaction, CancellationToken cancellationToken);
}