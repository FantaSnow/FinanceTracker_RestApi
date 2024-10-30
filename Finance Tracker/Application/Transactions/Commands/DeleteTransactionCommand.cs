using Application.Banks.Commands;
using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.Transactions.Exceptions;
using Domain.Banks;
using Domain.Transactions;
using Domain.Users;
using MediatR;

namespace Application.Transactions.Commands;

public record DeleteTransactionCommand : IRequest<Result<Transaction, TransactionException>>
{
    public required Guid TransactionId { get; init; }
}

public class DeleteTransactionCommandHandler(ITransactionRepository transactionRepository, IUserRepository userRepository)
    : IRequestHandler<DeleteTransactionCommand, Result<Transaction, TransactionException>>
{
    public async Task<Result<Transaction, TransactionException>> Handle(DeleteTransactionCommand request,
        CancellationToken cancellationToken)
    {
        var transactionId = new TransactionId(request.TransactionId);
        var existingTransaction = await transactionRepository.GetById(transactionId, cancellationToken);
        return await existingTransaction.Match<Task<Result<Transaction, TransactionException>>>(
            async t =>
            {
                var existingUser = await userRepository.GetById(t.UserId, cancellationToken);

                return await existingUser.Match<Task<Result<Transaction, TransactionException>>>(
                    async u => await DeleteEntity(t, u, cancellationToken),
                    () => Task.FromResult<Result<Transaction, TransactionException>>(new UserNotFoundException(t.UserId)));
            },
            () => Task.FromResult<Result<Transaction, TransactionException>>(new TransactionNotFoundException(transactionId)));
    }

    private async Task<Result<Transaction, TransactionException>> DeleteEntity(Transaction transaction, User user,
        CancellationToken cancellationToken)
    {
        try
        {
            user.AddToBalance(transaction.Sum);
            await userRepository.Update(user, cancellationToken);
            var afterDeleateTransaction = await transactionRepository.Delete(transaction, cancellationToken);
            return afterDeleateTransaction;
        }
        catch (Exception exception)
        {
            return new TransactionUnknownException(transaction.Id, exception);
        }
    }
    
}