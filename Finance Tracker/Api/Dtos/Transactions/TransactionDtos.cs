using Api.Dtos.Categorys;
using Api.Dtos.Users;
using Domain.Categorys;
using Domain.Transactions;

namespace Api.Dtos.Transactions;

public record TransactionDto(
    Guid? Id,
    decimal Sum,
    DateTime CreatedAt,
    Guid UserId,
    UserDto? User,
    string? CategoryName)
{
    public static TransactionDto FromDomainModel(Transaction transaction)
        => new(
            Id: transaction.Id.Value,
            CreatedAt: transaction.CreatedAt,
            Sum: transaction.Sum,
            UserId: transaction.UserId.Value,
            User: transaction.User == null ? null : UserDto.FromDomainModel(transaction.User),
            CategoryName: transaction.Category!.Name
        );
}