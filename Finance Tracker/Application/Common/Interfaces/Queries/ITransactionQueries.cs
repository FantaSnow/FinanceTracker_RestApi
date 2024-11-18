using Domain.Categorys;
using Domain.Transactions;
using Domain.Users;
using Optional;

namespace Application.Common.Interfaces.Queries;

public interface ITransactionQueries
{
    Task<IReadOnlyList<Transaction>> GetAll(CancellationToken cancellationToken);
    Task<Option<Transaction>> GetById(TransactionId id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Transaction>> GetAllByUser(UserId id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Transaction>> GetAllMinusByUserAndDate(UserId id, DateTime startDateTime, DateTime endDateTime, CancellationToken cancellationToken);
    Task<IReadOnlyList<Transaction>> GetAllPlusByUserAndDate(UserId id, DateTime startDateTime, DateTime endDateTime, CancellationToken cancellationToken);
    Task<IReadOnlyList<Transaction>> GetAllMinusByUserAndDate(UserId id, DateTime startDateTime, DateTime endDateTime,CategoryId categoryId,  CancellationToken cancellationToken);
    Task<IReadOnlyList<Transaction>> GetAllPlusByUserAndDate(UserId id, DateTime startDateTime, DateTime endDateTime,CategoryId categoryId, CancellationToken cancellationToken);
}