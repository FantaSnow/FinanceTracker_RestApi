using Api.Dtos.Categorys;

namespace Api.Dtos.Transactions;

public record TransactionCreateDto(decimal Sum ,Guid CategoryId, CategoryDto? Category)
{
    public static TransactionCreateDto FromDomainModel(Domain.Transactions.Transaction transaction)
        => new(
            Sum: transaction.Sum,
            CategoryId: transaction.Category!.Id.Value,
            Category: transaction.Category == null ? null : CategoryDto.FromDomainModel(transaction.Category)
        );
            
}