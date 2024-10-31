using Domain.Categorys;
using Optional;

namespace Application.Common.Interfaces.Repositories;

public interface ICategoryRepository
{
    Task<Category> Add(Category category, CancellationToken cancellationToken);
    Task<Category> Update(Category category, CancellationToken cancellationToken);
    Task<Category> Delete(Category category, CancellationToken cancellationToken);
    Task<Option<Category>> GetByName(string name, CancellationToken cancellationToken);
    Task<Option<Category>> GetById(CategoryId id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Category>> GetAll(CancellationToken cancellationToken);
}