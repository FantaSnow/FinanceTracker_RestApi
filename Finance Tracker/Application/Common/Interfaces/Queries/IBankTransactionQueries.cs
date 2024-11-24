using Domain.Banks;
using Domain.BankTransactions;
using Domain.Users;
using Optional;

namespace Application.Common.Interfaces.Queries;

public interface IBankTransactionQueries
{
    Task<IReadOnlyList<BankTransaction>> GetAll(CancellationToken cancellationToken);
    Task<Option<BankTransaction>> GetById(BankTransactionId id, CancellationToken cancellationToken);
    Task<IReadOnlyList<BankTransaction>> GetAllByBank(BankId id, CancellationToken cancellationToken);
    Task<IReadOnlyList<BankTransaction>> GetAllByUser(UserId id, CancellationToken cancellationToken);
}