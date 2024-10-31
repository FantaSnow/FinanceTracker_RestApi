using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.Categorys;
using Microsoft.EntityFrameworkCore;
using Optional;

namespace Infrastructure.Persistence.Repositories;

public class CategoryRepository(ApplicationDbContext context) : ICategoryRepository, ICategoryQueries
{
    public async Task<Category> Add(Category category, CancellationToken cancellationToken)
    {
        await context.Categorys.AddAsync(category, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return category;
    }

    public async Task<Category> Update(Category category, CancellationToken cancellationToken)
    {
        context.Categorys.Update(category);
        await context.SaveChangesAsync(cancellationToken);

        return category;
    }

    public async Task<Category> Delete(Category category, CancellationToken cancellationToken)
    {
        context.Categorys.Remove(category);
        await context.SaveChangesAsync(cancellationToken);

        return category;
    }

    public async Task<IReadOnlyList<Category>> GetAll(CancellationToken cancellationToken)
    {
        return await context.Categorys
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Option<Category>> GetById(CategoryId id, CancellationToken cancellationToken)
    {
        var entity = await context.Categorys
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return entity == null ? Option.None<Category>() : Option.Some(entity);
    }

    public async Task<Option<Category>> GetByName(string name, CancellationToken cancellationToken)
    {
        var entity = await context.Categorys
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Name == name, cancellationToken);

        return entity == null ? Option.None<Category>() : Option.Some(entity);
    }
}