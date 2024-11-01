using Domain.Users;

namespace Api.Dtos.Users;

public record UserBalanceDto(
    decimal Balance)
{
    public static UserBalanceDto FromDomainModel(User user)
        => new(
            Balance: user.Balance
        );
}