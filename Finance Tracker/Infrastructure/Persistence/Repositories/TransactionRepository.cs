using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.Categorys;
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
    
    public async Task<IReadOnlyList<Transaction>> GetAllMinusByUserAndDate(UserId id, DateTime startDateTime, DateTime endDateTime, CancellationToken cancellationToken)
    {
        var entity = await context.Transactions
            .Include(t => t.Category)
            .AsNoTracking()
            .Where(x => 
                x.UserId == id && 
                x.CreatedAt > startDateTime && 
                x.CreatedAt < endDateTime && 
                x.Sum < 0)
            .ToListAsync(cancellationToken);
        return entity;

    }
    
    public async Task<IReadOnlyList<Transaction>> GetAllPlusByUserAndDate(UserId id, DateTime startDateTime, DateTime endDateTime, CancellationToken cancellationToken)
    {
        var entity = await context.Transactions
            .Include(t => t.Category)
            .AsNoTracking()
            .Where(x => 
                x.UserId == id && 
                x.CreatedAt > startDateTime && 
                x.CreatedAt < endDateTime && 
                x.Sum > 0)
            .ToListAsync(cancellationToken);
        return entity;

    }
    
    public async Task<IReadOnlyList<Transaction>> GetAllMinusByUserAndDate(UserId id, DateTime startDateTime, DateTime endDateTime,CategoryId categoryId, CancellationToken cancellationToken)
    {
        var entity = await context.Transactions
            .Include(t => t.Category)
            .AsNoTracking()
            .Where(x => 
                x.UserId == id && 
                x.CreatedAt > startDateTime && 
                x.CreatedAt < endDateTime && 
                x.Sum < 0 &&
                x.CategoryId == categoryId)
            .ToListAsync(cancellationToken);
        return entity;

    }
    
    public async Task<IReadOnlyList<Transaction>> GetAllPlusByUserAndDate(UserId id, DateTime startDateTime, DateTime endDateTime, CategoryId categoryId,CancellationToken cancellationToken)
    {
        var entity = await context.Transactions
            .Include(t => t.Category)
            .AsNoTracking()
            .Where(x => 
                x.UserId == id && 
                x.CreatedAt > startDateTime && 
                x.CreatedAt < endDateTime && 
                x.Sum > 0 &&
                x.CategoryId == categoryId)
            .ToListAsync(cancellationToken);
        return entity;

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