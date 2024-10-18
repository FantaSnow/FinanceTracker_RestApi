
using Domain.Categorys;

namespace Application.Common.Interfaces.Repositories;

public interface ICategoryRepository
{
    Task<Category> Add(Category category, CancellationToken cancellationToken);
    Task<Category> Update(Category category, CancellationToken cancellationToken);
    Task<Category> Delete(Category category, CancellationToken cancellationToken);
}