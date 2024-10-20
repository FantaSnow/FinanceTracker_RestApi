using Domain.Users;

namespace Api.Dtos;

public record UserDto(
    Guid? Id,
    string Login,
    string Password,
    decimal Balance,
    DateTime? CreatedAt)
{
    public static UserDto FromDomainModel(User user)
        => new(
            Id: user.Id.Value,
            Login: user.Login,
            Password: user.Password,
            Balance: user.Balance,
            CreatedAt: user.CreatedAt
            );
}
