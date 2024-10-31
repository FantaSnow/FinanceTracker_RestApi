using Domain.Categorys;

namespace Api.Dtos.Categorys;

public record CategoryDto(Guid? Id, string Name)
{
    public static CategoryDto FromDomainModel(Category category)
        => new(
            Id: category.Id.Value,
            Name: category.Name
        );
}