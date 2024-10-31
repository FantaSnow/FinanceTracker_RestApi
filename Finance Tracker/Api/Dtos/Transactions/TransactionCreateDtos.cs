using Api.Dtos.Categorys;
using Domain.Transactions;

namespace Api.Dtos.Transactions;

public record TransactionCreateDto(decimal Sum ,Guid CategoryId, CategoryDto? Category)
{
    public static TransactionCreateDto FromDomainModel(Transaction transaction)
        => new(
            Sum: transaction.Sum,
            CategoryId: transaction.CategoryId!.Value,
            Category: transaction.Category == null ? null : CategoryDto.FromDomainModel(transaction.Category)
        );
            
}