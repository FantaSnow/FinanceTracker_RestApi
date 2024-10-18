using Domain.Banks;

namespace Application.Common.Interfaces.Repositories;

public interface IBankRepository
{
    Task<Bank> Add(Bank bank, CancellationToken cancellationToken);
    Task<Bank> Update(Bank bank, CancellationToken cancellationToken);
    Task<Bank> Delete(Bank bank, CancellationToken cancellationToken);
}