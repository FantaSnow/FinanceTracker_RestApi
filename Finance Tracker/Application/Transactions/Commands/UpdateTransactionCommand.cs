using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.Transactions.Exceptions;
using Domain.Categorys;
using Domain.Transactions;
using Domain.Users;
using MediatR;

namespace Application.Transactions.Commands;

public record UpdateTransactionCommand : IRequest<Result<Transaction, TransactionException>>
{
    public required Guid TransactionId { get; init; }
    public required decimal Sum { get; init; }
    public required Guid CategoryId { get; init; }
    public required Guid UserIdFromToken { get; init; }
}

public class UpdateTransactionCommandHandler(
    ITransactionRepository transactionRepository,
    IUserRepository userRepository,
    ICategoryRepository categoryRepository)
    : IRequestHandler<UpdateTransactionCommand, Result<Transaction, TransactionException>>
{
    public async Task<Result<Transaction, TransactionException>> Handle(UpdateTransactionCommand request,
        CancellationToken cancellationToken)
    {
        var transactionId = new TransactionId(request.TransactionId);
        var existingTransaction = await transactionRepository.GetById(transactionId, cancellationToken);

        return await existingTransaction.Match<Task<Result<Transaction, TransactionException>>>(
            async t =>
            {
                var userFromTransactionId = t.UserId;
                var existingUserFromTransaction =
                    await userRepository.GetById(userFromTransactionId, cancellationToken);

                return await existingUserFromTransaction.Match<Task<Result<Transaction, TransactionException>>>(
                    async uft =>
                    {
                        var userIdFromToken = new UserId(request.UserIdFromToken);
                        var existingUserFromToken = await userRepository.GetById(userIdFromToken, cancellationToken);

                        return await existingUserFromToken.Match<Task<Result<Transaction, TransactionException>>>(
                            async userFromToken =>
                            {
                                var categoryId = new CategoryId(request.CategoryId);
                                var existingCategory = await categoryRepository.GetById(categoryId, cancellationToken);

                                return await existingCategory.Match(
                                    async category => await UpdateEntity(t, uft, userFromToken, request.Sum, category.Id,
                                        cancellationToken),
                                    () => Task.FromResult<Result<Transaction, TransactionException>>(
                                        new CategoryNotFoundException(categoryId)));
                            },
                            () => Task.FromResult<Result<Transaction, TransactionException>>(
                                new UserNotFoundException(userIdFromToken))
                        );
                    },
                    () => Task.FromResult<Result<Transaction, TransactionException>>(
                        new UserNotFoundException(userFromTransactionId))
                );
            },
            () => Task.FromResult<Result<Transaction, TransactionException>>(
                new TransactionNotFoundException(transactionId))
        );
    }

    private async Task<Result<Transaction, TransactionException>> UpdateEntity(
        Transaction transaction,
        User userFromTransaction,
        User userFromToken,
        decimal sum,
        CategoryId categoryId,
        CancellationToken cancellationToken)
    {
        try
        {
            if (userFromToken.Id == userFromTransaction.Id || userFromToken.IsAdmin)
            {
                userFromTransaction.AddToBalance(-transaction.Sum);
                userFromTransaction.AddToBalance(sum);
                await userRepository.Update(userFromTransaction, cancellationToken);

                transaction.UpdateDatails(sum, categoryId);
                return await transactionRepository.Update(transaction, cancellationToken);
            }

            return await Task.FromResult<Result<Transaction, TransactionException>>(
                new YouDoNotHaveTheAuthorityToDo(userFromToken.Id, userFromTransaction.Id));
        }
        catch (Exception exception)
        {
            return new TransactionUnknownException(transaction.Id, exception);
        }
    }
}