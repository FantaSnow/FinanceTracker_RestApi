using Domain.Banks;
using Domain.Users;
using Optional;

namespace Application.Common.Interfaces.Repositories;

public interface IBankRepository
{
    Task<Bank> Add(Bank bank, CancellationToken cancellationToken);
    Task<Bank> Update(Bank bank, CancellationToken cancellationToken);
    Task<Bank> Delete(Bank bank, CancellationToken cancellationToken);
    Task<Option<Bank>> GetById(BankId id, CancellationToken cancellationToken);
    Task<Option<Bank>> GetByNameAndUser(string name, UserId userId, CancellationToken cancellationToken);
    Task<Option<Bank>> GetByNameAndUser(string name, UserId userId,BankId bankId, CancellationToken cancellationToken);

}