using Application.Banks.Commands;
using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.Transactions.Exceptions;
using Domain.Banks;
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

}

public class CreateTransactionCommandHandler(ITransactionRepository transactionRepository, IUserRepository userRepository, ICategoryRepository categoryRepository)
    : IRequestHandler<CreateTransactionCommand, Result<Transaction, TransactionException>>
{
    public async Task<Result<Transaction, TransactionException>> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
    {
        var userId = new UserId(request.UserId);
        var existingUser = await userRepository.GetById(userId,cancellationToken);
        
        return await existingUser.Match(
            async u =>
            {
                var categoryId = new CategoryId(request.CategoryId);
                var existingCategory = await categoryRepository.GetById(categoryId, cancellationToken);
                return await existingCategory.Match(
                    async c => await CreateEntity(request.Sum, categoryId, u, cancellationToken),
                    () => Task.FromResult<Result<Transaction, TransactionException>>(new CategoryNotFoundException(categoryId)));
            },
            () => Task.FromResult<Result<Transaction, TransactionException>>(new UserNotFoundException(userId)));
    }

    private async Task<Result<Transaction, TransactionException>> CreateEntity(
        decimal sum,
        CategoryId categoryId,
        User user,
        CancellationToken cancellationToken)
    {
        try
        {
            
            var entity = Transaction.Create(TransactionId.New(), sum, user.Id, categoryId);
            user.AddToBalance(sum);
            await userRepository.Update(user, cancellationToken);
            return await transactionRepository.Add(entity, cancellationToken);
        }
        catch (Exception exception)
        {
            return new TransactionUnknownException(TransactionId.Empty(), exception);
        }
    }
}