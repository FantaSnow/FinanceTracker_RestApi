using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.Transactions.Exceptions;
using Domain.Transactions;
using Domain.Users;
using MediatR;

namespace Application.Transactions.Commands;

public record DeleteTransactionCommand : IRequest<Result<Transaction, TransactionException>>
{
    public required Guid TransactionId { get; init; }
    public required Guid UserIdFromToken { get; init; }
}

public class DeleteTransactionCommandHandler(
    ITransactionRepository transactionRepository,
    IUserRepository userRepository)
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
                var userFromTransactionId = t.UserId;
                var existingUserFromTransaction = await userRepository.GetById(userFromTransactionId, cancellationToken);

                return await existingUserFromTransaction.Match<Task<Result<Transaction, TransactionException>>>(
                    async userFromTransaction =>
                    {
                        var userIdFromToken = new UserId(request.UserIdFromToken);
                        var existingUserFromToken = await userRepository.GetById(userIdFromToken, cancellationToken);

                        return await existingUserFromToken.Match<Task<Result<Transaction, TransactionException>>>(
                            async userFromToken => await DeleteEntity(userFromTransaction, userFromToken, t, cancellationToken),
                            () => Task.FromResult<Result<Transaction, TransactionException>>(new UserNotFoundException(userIdFromToken))
                        );
                    },
                    () => Task.FromResult<Result<Transaction, TransactionException>>(new UserNotFoundException(userFromTransactionId))
                );
            },
            () => Task.FromResult<Result<Transaction, TransactionException>>(new TransactionNotFoundException(transactionId))
        );
    }

    private async Task<Result<Transaction, TransactionException>> DeleteEntity(
        User userFromTransaction,
        User userFromToken,
        Transaction transaction,
        CancellationToken cancellationToken)
    {
        try
        {
            if (userFromToken.Id == userFromTransaction.Id || userFromToken.IsAdmin)
            {
                userFromTransaction.AddToBalance(-transaction.Sum);
                await userRepository.Update(userFromTransaction, cancellationToken);
                return await transactionRepository.Delete(transaction, cancellationToken);
            }

            return await Task.FromResult<Result<Transaction, TransactionException>>(
                new YouDoNotHaveTheAuthorityToDo(userFromToken.Id, userFromTransaction.Id)
            );
        }
        catch (Exception exception)
        {
            return new TransactionUnknownException(transaction.Id, exception);
        }
    }
}
