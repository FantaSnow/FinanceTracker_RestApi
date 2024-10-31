using Domain.Users;

namespace Api.Dtos.Users;

public record UserCreateDto(
    string Login,
    string Password)
{
    public static UserCreateDto FromDomainModel(User user)
        => new(
            Login: user.Login,
            Password: user.Password);
}