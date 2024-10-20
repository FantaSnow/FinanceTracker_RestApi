using Domain.Transactions;

namespace Api.Dtos;

public record TransactionDto(Guid? Id, decimal Sum ,DateTime CreatedAt,Guid CategoryId, CategoryDto? Category)
{
    public static TransactionDto FromDomainModel(Transaction transaction)
        => new(
            Id: transaction.Id.Value,
            Sum: transaction.Sum,
            CreatedAt: transaction.CreatedAt,
            CategoryId: transaction.Category!.Id.Value,
            Category: transaction.Category == null ? null : CategoryDto.FromDomainModel(transaction.Category)
            );
            
}