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
        context.Entry(bank).State = EntityState.Modified;

        await context.SaveChangesAsync(cancellationToken);

        return bank;
    }

    public async void SaveChangesAsync(CancellationToken cancellationToken)
    {
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<Bank> Delete(Bank bank, CancellationToken cancellationToken)
    {
        context.Entry(bank).State = EntityState.Deleted;

        await context.SaveChangesAsync(cancellationToken);

        return bank;
    }


    public async Task<IReadOnlyList<Bank>> GetAll(CancellationToken cancellationToken)
    {
        return await context.Banks
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Bank>> GetAllByUser(UserId id,CancellationToken cancellationToken)
    {
        var entity = await context.Banks
            .AsNoTracking()
            .Where(x => x.UserId == id)
            .ToListAsync(cancellationToken);
        return entity;
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
    public async Task<Option<Bank>> GetByNameAndUser(string name, UserId userId,BankId bankId, CancellationToken cancellationToken)
    {
        var entity = await context.Banks
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Name == name && x.UserId == userId && x.Id != bankId, cancellationToken);

        return entity == null ? Option.None<Bank>() : Option.Some(entity);
    }
}