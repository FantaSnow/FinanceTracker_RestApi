using Api.Dtos.Categorys;
using Api.Dtos.Users;
using Domain.Transactions;

namespace Api.Dtos.Transactions;

public record TransactionDto(Guid Id, decimal Sum ,DateTime CreatedAt,Guid UserId, UserDto? User,Guid CategoryId, CategoryDto? Category)
{
    public static TransactionDto FromDomainModel(Transaction transaction)
        => new(
            Id: transaction.Id.Value,
            Sum: transaction.Sum,
            CreatedAt: transaction.CreatedAt,
            UserId: transaction.UserId.Value,
            User: transaction.User == null ? null : UserDto.FromDomainModel(transaction.User),
            CategoryId: transaction.Category!.Id.Value,
            Category: transaction.Category == null ? null : CategoryDto.FromDomainModel(transaction.Category)
            );
            
}