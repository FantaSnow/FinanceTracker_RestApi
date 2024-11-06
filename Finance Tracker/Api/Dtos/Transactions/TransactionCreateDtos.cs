using Api.Dtos.Categorys;
using Domain.Transactions;

namespace Api.Dtos.Transactions;

public record TransactionCreateDto(decimal Sum, Guid CategoryId)
{
    public static TransactionCreateDto FromDomainModel(Transaction transaction)
        => new(
            Sum: transaction.Sum,
            CategoryId: transaction.CategoryId!.Value
        );
}