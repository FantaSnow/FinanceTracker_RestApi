using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.Transactions;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Optional;

namespace Infrastructure.Persistence.Repositories;

public class TransactionRepository(ApplicationDbContext context) : ITransactionRepository, ITransactionQueries
{
    public async Task<Transaction> Add(Transaction transaction, CancellationToken cancellationToken)
    {
        await context.Transactions.AddAsync(transaction, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return transaction;
    }

    public async Task<Transaction> Update(Transaction transaction, CancellationToken cancellationToken)
    {
        context.Transactions.Update(transaction);
        await context.SaveChangesAsync(cancellationToken);

        return transaction;
    }

    public async Task<Transaction> Delete(Transaction transaction, CancellationToken cancellationToken)
    {
        context.Transactions.Remove(transaction);
        await context.SaveChangesAsync(cancellationToken);

        return transaction;
    }

    public async Task<IReadOnlyList<Transaction>> GetAll(CancellationToken cancellationToken)
    {
        return await context.Transactions
            .Include(t => t.Category)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }


    public async Task<Option<Transaction>> GetById(TransactionId id, CancellationToken cancellationToken)
    {
        var entity = await context.Transactions
            .Include(t => t.Category)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return entity == null ? Option.None<Transaction>() : Option.Some(entity);
    }

    public async Task<IReadOnlyList<Transaction>> GetAllByUser(UserId id, CancellationToken cancellationToken)
    {
        var entity = await context.Transactions
            .Include(t => t.Category)
            .AsNoTracking()
            .Where(x => x.UserId == id)
            .ToListAsync(cancellationToken);
        return entity;
    }
}