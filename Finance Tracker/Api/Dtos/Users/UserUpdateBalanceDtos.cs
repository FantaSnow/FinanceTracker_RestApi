using Domain.Users;

namespace Api.Dtos.Users;

public record UserUpdateBalanceDto(
    decimal Balance)
{
    public static UserUpdateBalanceDto FromDomainModel(User user)
        => new(
            Balance: user.Balance
        );
}