using Application.Categorys.Exceptions;
using Application.Common;
using Application.Common.Interfaces.Repositories;
using Domain.Categorys;
using MediatR;

namespace Application.Categorys.Commands;

public record UpdateCategoryCommand : IRequest<Result<Category, CategoryException>>
{
    public required Guid CategoryId { get; init; }
    public required string Name { get; init; }
}

public class UpdateCategoryCommandHandler(ICategoryRepository categoryRepository)
    : IRequestHandler<UpdateCategoryCommand, Result<Category, CategoryException>>
{
    public async Task<Result<Category, CategoryException>> Handle(UpdateCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var categoryId = new CategoryId(request.CategoryId);

        var existingCategory = await categoryRepository.GetById(categoryId, cancellationToken);

        return await existingCategory.Match(
            async u => await UpdateEntity(u, request.Name, cancellationToken),
            () => Task.FromResult<Result<Category, CategoryException>>(new CategoryNotFoundException(categoryId)));
    }

    private async Task<Result<Category, CategoryException>> UpdateEntity(
        Category entity,
        string name,
        CancellationToken cancellationToken)
    {
        try
        {
            entity.UpdateName(name);

            return await categoryRepository.Update(entity, cancellationToken);
        }
        catch (Exception exception)
        {
            return new CategoryUnknownException(entity.Id, exception);
        }
    }
}