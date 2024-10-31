using Domain.Categorys;

namespace Api.Dtos.Categorys;

public record CategoryCreateDto(string Name)
{
    public static CategoryCreateDto FromDomainModel(Category category)
        => new(
            Name: category.Name
        );
}