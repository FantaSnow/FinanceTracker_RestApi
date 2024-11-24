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

public record UpdateBankTransactionCommand : IRequest<Result<BankTransaction, BankTransactionException>>
{
    public required Guid BankTransactionId { get; init; }
    public required decimal Sum { get; init; }
    public required Guid UserIdFromToken { get; init; }
}

public class UpdateBankTransactionCommandHandler(
    IBankTransactionRepository bankTransactionRepository,
    IUserRepository userRepository,
    IBankRepository bankRepository)
    : IRequestHandler<UpdateBankTransactionCommand, Result<BankTransaction, BankTransactionException>>
{
    public async Task<Result<BankTransaction, BankTransactionException>> Handle(UpdateBankTransactionCommand request,
        CancellationToken cancellationToken)
    {
        var bankTransactionId = new BankTransactionId(request.BankTransactionId);
        var existingTransaction = await bankTransactionRepository.GetById(bankTransactionId, cancellationToken);

        return await existingTransaction.Match<Task<Result<BankTransaction, BankTransactionException>>>(
            async t =>
            {
                var userFromTransactionId = t.UserId;
                var existingUserFromTransaction =
                    await userRepository.GetById(userFromTransactionId, cancellationToken);

                return await existingUserFromTransaction.Match<Task<Result<BankTransaction, BankTransactionException>>>(
                    async uft =>
                    {
                        var userIdFromToken = new UserId(request.UserIdFromToken);
                        var existingUserFromToken = await userRepository.GetById(userIdFromToken, cancellationToken);

                        return await existingUserFromToken
                            .Match<Task<Result<BankTransaction, BankTransactionException>>>(
                                async userFromToken =>
                                {
                                    var bankId = t.BankId;
                                    var existingBank = await bankRepository.GetById(bankId, cancellationToken);

                                    return await existingBank.Match(
                                        async bank => await UpdateEntity(t, uft, userFromToken, request.Sum, bank,
                                            cancellationToken),
                                        () => Task.FromResult<Result<BankTransaction, BankTransactionException>>(
                                            new BankNotFoundException(bankId))
                                    );
                                },
                                () => Task.FromResult<Result<BankTransaction, BankTransactionException>>(
                                    new UserNotFoundException(userIdFromToken))
                            );
                    },
                    () => Task.FromResult<Result<BankTransaction, BankTransactionException>>(
                        new UserNotFoundException(userFromTransactionId))
                );
            },
            () => Task.FromResult<Result<BankTransaction, BankTransactionException>>(
                new BankTransactionNotFoundException(bankTransactionId))
        );
    }

    private async Task<Result<BankTransaction, BankTransactionException>> UpdateEntity(
        BankTransaction transaction,
        User userFromTransaction,
        User userFromToken,
        decimal sum,
        Bank bank,
        CancellationToken cancellationToken)
    {
        try
        {
            if (userFromToken.Id == userFromTransaction.Id || userFromToken.IsAdmin)
            {
                userFromTransaction.AddToBalance(transaction.Amount);
                userFromTransaction.AddToBalance(-sum);
                await userRepository.Update(userFromTransaction, cancellationToken);
                
                bank.AddToBalance(-transaction.Amount);
                bank.AddToBalance(sum);
                await bankRepository.Update(bank, cancellationToken);

                transaction.UpdateBalance(sum);
                return await bankTransactionRepository.Update(transaction, cancellationToken);
            }

            return await Task.FromResult<Result<BankTransaction, BankTransactionException>>(
                new YouDoNotHaveTheAuthorityToDo(userFromToken.Id, userFromTransaction.Id));
        }
        catch (Exception exception)
        {
            return new BankTransactionUnknownException(transaction.Id, exception);
        }
    }
}