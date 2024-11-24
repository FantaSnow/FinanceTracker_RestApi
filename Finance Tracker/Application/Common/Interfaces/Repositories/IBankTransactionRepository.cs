using Domain.BankTransactions;
using Domain.Transactions;
using Domain.Users;
using Optional;

namespace Application.Common.Interfaces.Repositories;

public interface IBankTransactionRepository
{
    Task<BankTransaction> Add(BankTransaction bankTransaction, CancellationToken cancellationToken);
    Task<BankTransaction> Update(BankTransaction bankTransaction, CancellationToken cancellationToken);
    Task<BankTransaction> Delete(BankTransaction bankTransaction, CancellationToken cancellationToken);
    Task<Option<BankTransaction>> GetById(BankTransactionId id, CancellationToken cancellationToken);
    Task<IReadOnlyList<BankTransaction>> GetAllByUser(UserId id, CancellationToken cancellationToken);
}