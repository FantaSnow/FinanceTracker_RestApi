using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.Banks;
using Domain.BankTransactions;
using Domain.Categorys;
using Domain.Transactions;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Optional;

namespace Infrastructure.Persistence.Repositories;

public class BankTransactionRepository(ApplicationDbContext context) : IBankTransactionRepository, IBankTransactionQueries
{
    public async Task<BankTransaction> Add(BankTransaction bankTransaction, CancellationToken cancellationToken)
    {
        await context.BankTransactions.AddAsync(bankTransaction, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return bankTransaction;
    }

    public async Task<BankTransaction> Update(BankTransaction bankTransaction, CancellationToken cancellationToken)
    {
        context.BankTransactions.Update(bankTransaction);
        await context.SaveChangesAsync(cancellationToken);

        return bankTransaction;
    }

    public async Task<BankTransaction> Delete(BankTransaction bankTransaction, CancellationToken cancellationToken)
    {
        context.BankTransactions.Remove(bankTransaction);
        await context.SaveChangesAsync(cancellationToken);

        return bankTransaction;
    }

    public async Task<IReadOnlyList<BankTransaction>> GetAll(CancellationToken cancellationToken)
    {
        return await context.BankTransactions
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }


    public async Task<Option<BankTransaction>> GetById(BankTransactionId id, CancellationToken cancellationToken)
    {
        var entity = await context.BankTransactions
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return entity == null ? Option.None<BankTransaction>() : Option.Some(entity);
    }
    
    public async Task<IReadOnlyList<BankTransaction>> GetAllByBank(BankId id, CancellationToken cancellationToken)
    {
        var entity = await context.BankTransactions
            .AsNoTracking()
            .Where(x => x.BankId == id)
            .ToListAsync(cancellationToken);
        return entity;
    }
    public async Task<IReadOnlyList<BankTransaction>> GetAllByUser(UserId id, CancellationToken cancellationToken)
    {
        var entity = await context.BankTransactions
            .AsNoTracking()
            .Where(x => x.UserId == id)
            .ToListAsync(cancellationToken);
        return entity;
    }
}