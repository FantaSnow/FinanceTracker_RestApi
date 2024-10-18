using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Optional;

namespace Infrastructure.Persistence.Repositories;

public class UserRepository(ApplicationDbContext context) : IUserRepository, IUserQueries
{
    public async Task<User> Add(User user, CancellationToken cancellationToken)
    {
        await context.Users.AddAsync(user, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return user;    
    }

    public async Task<User> Update(User user, CancellationToken cancellationToken)
    {
        context.Users.Update(user);
        await context.SaveChangesAsync(cancellationToken);

        return user;    
    }

    public async Task<User> Delete(User user, CancellationToken cancellationToken)
    {
        context.Users.Remove(user);
        await context.SaveChangesAsync(cancellationToken);

        return user;    
    }

    public async Task<Option<IReadOnlyList<User>>> GetAll(CancellationToken cancellationToken)
    {
        var entity = await context.Users
            .AsNoTracking()
            .ToListAsync(cancellationToken);
        
        return entity.Any() ? Option.Some<IReadOnlyList<User>>(entity.AsReadOnly()) : Option.None<IReadOnlyList<User>>();
    }

    public async Task<Option<User>> GetById(UserId id, CancellationToken cancellationToken)
    {
        var entity = await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return entity == null ? Option.None<User>() : Option.Some(entity);    
    }

    public async Task<Option<User>> GetByLogin(string login, CancellationToken cancellationToken)
    {
        var entity = await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Login == login, cancellationToken);
        
        return entity == null ? Option.None<User>() : Option.Some(entity);    
    }

    public async Task<decimal> GetBalanceById(UserId id, CancellationToken cancellationToken)
    {
        var entity = await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        
        return entity!.Balance;    
    }
}