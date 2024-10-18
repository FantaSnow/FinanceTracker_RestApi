using Domain.Users;
using Optional;

namespace Application.Common.Interfaces.Queries;

public interface IUserQueries
{
    Task<Option<IReadOnlyList<User>>> GetAll(CancellationToken cancellationToken);
    Task<Option<User>> GetById(UserId id, CancellationToken cancellationToken);
    Task<Option<User>> GetByLogin(string login, CancellationToken cancellationToken);
    Task<decimal> GetBalanceById(UserId id, CancellationToken cancellationToken);


}