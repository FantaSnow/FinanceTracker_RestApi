using Domain.Banks;
using Domain.Users;
using Optional;

namespace Application.Common.Interfaces.Queries;

public interface IBankQueries
{
    Task<IReadOnlyList<Bank>> GetAll(CancellationToken cancellationToken);
    Task<Option<Bank>> GetById(BankId id, CancellationToken cancellationToken);
    Task<Option<Bank>> GetByNameAndUser(string name, UserId userId, CancellationToken cancellationToken);

    
}