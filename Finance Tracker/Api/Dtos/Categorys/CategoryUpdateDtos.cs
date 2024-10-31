using Domain.Categorys;

namespace Api.Dtos.Categorys;

public record CategoryUpdateDto(string Name)
{
    public static CategoryUpdateDto FromDomainModel(Category category)
        => new(
            Name: category.Name
        );
}