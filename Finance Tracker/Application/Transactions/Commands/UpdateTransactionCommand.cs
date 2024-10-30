using Application.Banks.Commands;
using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.Transactions.Exceptions;
using Domain.Banks;
using Domain.Categorys;
using Domain.Transactions;
using MediatR;

namespace Application.Transactions.Commands;

public record UpdateTransactionCommand : IRequest<Result<Transaction, TransactionException>>
{
    public required Guid TransactionId { get; init; }
    public required decimal Sum { get; init; }
    public required Guid CategoryId { get; init; }
}

public class UpdateTransactionCommandHandler(ITransactionRepository transactionRepository)
    : IRequestHandler<UpdateTransactionCommand, Result<Transaction, TransactionException>>
{
    public async Task<Result<Transaction, TransactionException>> Handle(UpdateTransactionCommand request,
        CancellationToken cancellationToken)
    {
        var transactionId = new TransactionId(request.TransactionId);
        var existingTransaction = await transactionRepository.GetById(transactionId, cancellationToken);

        return await existingTransaction.Match(
            async t => await UpdateEntity(t, request.Sum, new CategoryId(request.CategoryId), cancellationToken),
            () => Task.FromResult<Result<Transaction, TransactionException>>(new TransactionNotFoundException(transactionId)));
    }

    private async Task<Result<Transaction, TransactionException>> UpdateEntity(
        Transaction entity,
        decimal sum,
        CategoryId categoryId,
        CancellationToken cancellationToken)
    {
        try
        {
            entity.UpdateDatails(sum, categoryId);

            return await transactionRepository.Update(entity, cancellationToken);
        }
        catch (Exception exception)
        {
            return new TransactionUnknownException(entity.Id, exception);
        }
    }
}