using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.Banks;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Optional;

namespace Infrastructure.Persistence.Repositories;

public class BankRepository(ApplicationDbContext context) : IBankRepository, IBankQueries
{
    public async Task<Bank> Add(Bank bank, CancellationToken cancellationToken)
    {
        await context.Banks.AddAsync(bank, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return bank;    
    }

    public async Task<Bank> Update(Bank bank, CancellationToken cancellationToken)
    {
        context.Banks.Update(bank);
        await context.SaveChangesAsync(cancellationToken);

        return bank;    
    }

    public async Task<Bank> Delete(Bank bank, CancellationToken cancellationToken)
    {
        context.Banks.Remove(bank);
        await context.SaveChangesAsync(cancellationToken);

        return bank;    
    }

    public async Task<Option<IReadOnlyList<Bank>>> GetAll(CancellationToken cancellationToken)
    {
        var entity = await context.Banks
            .AsNoTracking()
            .ToListAsync(cancellationToken);
        
        return entity.Any() ? Option.Some<IReadOnlyList<Bank>>(entity.AsReadOnly()) : Option.None<IReadOnlyList<Bank>>();
    }

    public async Task<Option<Bank>> GetById(BankId id, CancellationToken cancellationToken)
    {
        var entity = await context.Banks
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return entity == null ? Option.None<Bank>() : Option.Some(entity);    
    }

    public async Task<Option<Bank>> GetByNameAndUser(string name, UserId userId, CancellationToken cancellationToken)
    {
        var entity = await context.Banks
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Name == name && x.UserId == userId, cancellationToken);

        return entity == null ? Option.None<Bank>() : Option.Some(entity);        
    }
}
