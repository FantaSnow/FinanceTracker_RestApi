using Domain.Users;

namespace Api.Dtos.Users;

public record UserUpdateDto(
    string Login,
    string Password,
    decimal Balance
    )
{
    public static UserUpdateDto FromDomainModel(User user)
        => new(
            Login: user.Login,
            Password: user.Password,
            Balance: user.Balance);
}
