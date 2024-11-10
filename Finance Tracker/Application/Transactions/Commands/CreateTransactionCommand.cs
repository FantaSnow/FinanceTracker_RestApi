using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.Transactions.Exceptions;
using Domain.Categorys;
using Domain.Transactions;
using Domain.Users;
using MediatR;

namespace Application.Transactions.Commands;

public record CreateTransactionCommand : IRequest<Result<Transaction, TransactionException>>
{
    public required decimal Sum { get; init; }
    public required Guid CategoryId { get; init; }
    public required Guid UserId { get; init; }
    public required Guid UserIdFromToken { get; init; }
}

public class CreateTransactionCommandHandler(
    ITransactionRepository transactionRepository,
    IUserRepository userRepository,
    ICategoryRepository categoryRepository)
    : IRequestHandler<CreateTransactionCommand, Result<Transaction, TransactionException>>
{
    public async Task<Result<Transaction, TransactionException>> Handle(CreateTransactionCommand request,
        CancellationToken cancellationToken)
    {
        var userId = new UserId(request.UserId);
        var existingUserForTransaction = await userRepository.GetById(userId, cancellationToken);

        return await existingUserForTransaction.Match<Task<Result<Transaction, TransactionException>>>(
            async userForTransaction =>
            {
                var userIdFromToken = new UserId(request.UserIdFromToken);
                var existingUserFromToken = await userRepository.GetById(userIdFromToken, cancellationToken);

                return await existingUserFromToken.Match<Task<Result<Transaction, TransactionException>>>(
                    async userFromToken =>
                    {
                        var categoryId = new CategoryId(request.CategoryId);
                        var existingCategory = await categoryRepository.GetById(categoryId, cancellationToken);

                        return await existingCategory.Match<Task<Result<Transaction, TransactionException>>>(
                            async category => await CreateEntity(request.Sum, categoryId, userForTransaction, userFromToken, cancellationToken),
                            () => Task.FromResult<Result<Transaction, TransactionException>>(
                                new CategoryNotFoundException(categoryId)));
                    },
                    () => Task.FromResult<Result<Transaction, TransactionException>>(
                        new UserNotFoundException(userIdFromToken)));
            },
            () => Task.FromResult<Result<Transaction, TransactionException>>(new UserNotFoundException(userId)));
    }

    private async Task<Result<Transaction, TransactionException>> CreateEntity(
        decimal sum,
        CategoryId categoryId,
        User userForTransaction,
        User userFromToken,
        CancellationToken cancellationToken)
    {
        try
        {
            if (userFromToken.Id == userForTransaction.Id || userFromToken.IsAdmin)
            {
                var entity = Transaction.Create(TransactionId.New(), sum, userForTransaction.Id, categoryId);
                userForTransaction.AddToBalance(sum);
                await userRepository.Update(userForTransaction, cancellationToken);
                return await transactionRepository.Add(entity, cancellationToken);
            }

            return await Task.FromResult<Result<Transaction, TransactionException>>(
                new YouDoNotHaveTheAuthorityToDo(userFromToken.Id, userForTransaction.Id));
        }
        catch (Exception exception)
        {
            return new TransactionUnknownException(TransactionId.Empty(), exception);
        }
    }
}
