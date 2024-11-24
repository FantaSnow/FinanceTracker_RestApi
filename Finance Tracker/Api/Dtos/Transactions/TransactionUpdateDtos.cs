using Api.Dtos.Categorys;
using Domain.Transactions;

namespace Api.Dtos.Transactions;

public record TransactionUpdateDto(decimal Sum, Guid CategoryId)
{
    public static TransactionUpdateDto FromDomainModel(Transaction transaction)
        => new(
            Sum: transaction.Sum,
            CategoryId: transaction.Category!.Id.Value
        );
}