using Application.BankTransactions.Exceptions;
using Application.Common;
using Application.Common.Interfaces.Repositories;
using Domain.Banks;
using Domain.BankTransactions;
using Domain.Categorys;
using Domain.Transactions;
using Domain.Users;
using MediatR;

namespace Application.BankTransactions.Commands;

public record CreateBankTransactionCommand : IRequest<Result<BankTransaction, BankTransactionException>>
{
    public required decimal Sum { get; init; }
    public required Guid BankId { get; init; }
    public required Guid UserId { get; init; }
    public required Guid UserIdFromToken { get; init; }
}

public class CreateBankTransactionCommandHandler(
    IBankTransactionRepository bankTransactionRepository,
    IUserRepository userRepository,
    IBankRepository bankRepository)
    : IRequestHandler<CreateBankTransactionCommand, Result<BankTransaction, BankTransactionException>>
{
    public async Task<Result<BankTransaction, BankTransactionException>> Handle(CreateBankTransactionCommand request,
        CancellationToken cancellationToken)
    {
        var userId = new UserId(request.UserId);
        var existingUserForTransaction = await userRepository.GetById(userId, cancellationToken);

        return await existingUserForTransaction.Match<Task<Result<BankTransaction, BankTransactionException>>>(
            async userForTransaction =>
            {
                var userIdFromToken = new UserId(request.UserIdFromToken);
                var existingUserFromToken = await userRepository.GetById(userIdFromToken, cancellationToken);

                return await existingUserFromToken.Match<Task<Result<BankTransaction, BankTransactionException>>>(
                    async userFromToken =>
                    {
                        var bankId = new BankId(request.BankId);
                        var existingBank = await bankRepository.GetById(bankId, cancellationToken);

                        return await existingBank.Match<Task<Result<BankTransaction, BankTransactionException>>>(
                            async bank => await CreateEntity(request.Sum, bank, userForTransaction, userFromToken, cancellationToken),
                            () => Task.FromResult<Result<BankTransaction, BankTransactionException>>(
                                new BankNotFoundException(bankId)));
                    },
                    () => Task.FromResult<Result<BankTransaction, BankTransactionException>>(
                        new UserNotFoundException(userIdFromToken)));
            },
            () => Task.FromResult<Result<BankTransaction, BankTransactionException>>(new UserNotFoundException(userId)));
    }

    private async Task<Result<BankTransaction, BankTransactionException>> CreateEntity(
        decimal sum,
        Bank bank,
        User userForTransaction,
        User userFromToken,
        CancellationToken cancellationToken)
    {
        try
        {
            if (userFromToken.Id == userForTransaction.Id || userFromToken.IsAdmin)
            {
                userForTransaction.AddToBalance(-sum);
                await userRepository.Update(userForTransaction, cancellationToken);
                bank.AddToBalance(sum);
                await bankRepository.Update(bank, cancellationToken);
                
                var entity = BankTransaction.Create(BankTransactionId.New(), sum, userForTransaction.Id, bank.Id);
                
                return await bankTransactionRepository.Add(entity, cancellationToken);            }

            return await Task.FromResult<Result<BankTransaction, BankTransactionException>>(
                new YouDoNotHaveTheAuthorityToDo(userFromToken.Id, userForTransaction.Id));
        }
        catch (Exception exception)
        {
            return new BankTransactionUnknownException(BankTransactionId.Empty(), exception);
        }
    }
}
