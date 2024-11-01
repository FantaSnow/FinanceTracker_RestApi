using Domain.Users;

namespace Api.Dtos;

public record TokenGenerationRequest(
    string Login,
    string Password)
{
    public static TokenGenerationRequest FromDomainModel(User user)
        => new(
            Login: user.Login,
            Password: user.Password
        );
}