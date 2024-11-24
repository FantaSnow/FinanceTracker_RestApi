using Application.BankTransactions.Exceptions;
using Application.Common;
using Application.Common.Interfaces.Repositories;
using Domain.Banks;
using Domain.BankTransactions;
using Domain.Users;
using MediatR;


namespace Application.BankTransactions.Commands;

public record DeleteBankTransactionCommand : IRequest<Result<BankTransaction, BankTransactionException>>
{
    public required Guid BankTransactionId { get; init; }
    public required Guid UserIdFromToken { get; init; }
}

public class DeleteBankTransactionCommandHandler(
    IBankTransactionRepository bankTransactionRepository,
    IUserRepository userRepository,
    IBankRepository bankRepository)
    : IRequestHandler<DeleteBankTransactionCommand, Result<BankTransaction, BankTransactionException>>
{
    public async Task<Result<BankTransaction, BankTransactionException>> Handle(DeleteBankTransactionCommand request,
        CancellationToken cancellationToken)
    {
        var bankTransactionId = new BankTransactionId(request.BankTransactionId);
        var existingBankTransaction = await bankTransactionRepository.GetById(bankTransactionId, cancellationToken);

        return await existingBankTransaction.Match<Task<Result<BankTransaction, BankTransactionException>>>(
            async t =>
            {
                var userFromTransactionId = t.UserId;
                var existingUserFromTransaction = await userRepository.GetById(userFromTransactionId, cancellationToken);

                return await existingUserFromTransaction.Match<Task<Result<BankTransaction, BankTransactionException>>>(
                    async userFromTransaction =>
                    {
                        var userIdFromToken = new UserId(request.UserIdFromToken);
                        var existingUserFromToken = await userRepository.GetById(userIdFromToken, cancellationToken);

                        return await existingUserFromToken.Match<Task<Result<BankTransaction, BankTransactionException>>>(
                            async userFromToken =>
                            {
                                var bankIdFromTransaction = t.BankId;
                                var existingBankFromTransaction = await bankRepository.GetById(bankIdFromTransaction, cancellationToken);

                                return await existingBankFromTransaction.Match(
                                    async b => await DeleteEntity(userFromTransaction, userFromToken, t, b, cancellationToken),
                                    () => Task.FromResult<Result<BankTransaction, BankTransactionException>>(new BankNotFoundException(bankIdFromTransaction)));
                            },
                            () => Task.FromResult<Result<BankTransaction, BankTransactionException>>(new UserNotFoundException(userIdFromToken))
                        );
                    },
                    () => Task.FromResult<Result<BankTransaction, BankTransactionException>>(new UserNotFoundException(userFromTransactionId))
                );
            },
            () => Task.FromResult<Result<BankTransaction, BankTransactionException>>(new BankTransactionNotFoundException(bankTransactionId))
        );
    }

    private async Task<Result<BankTransaction, BankTransactionException>> DeleteEntity(
        User userFromTransaction,
        User userFromToken,
        BankTransaction transaction,
        Bank bank,
        CancellationToken cancellationToken)
    {
        try
        {
            if (userFromToken.Id == userFromTransaction.Id || userFromToken.IsAdmin)
            {
                userFromTransaction.AddToBalance(transaction.Amount);
                await userRepository.Update(userFromTransaction, cancellationToken);
                bank.AddToBalance(-transaction.Amount);
                await bankRepository.Update(bank, cancellationToken);

                return await bankTransactionRepository.Delete(transaction, cancellationToken);
            }

            return await Task.FromResult<Result<BankTransaction, BankTransactionException>>(
                new YouDoNotHaveTheAuthorityToDo(userFromToken.Id, userFromTransaction.Id)
            );
        }
        catch (Exception exception)
        {
            return new BankTransactionUnknownException(transaction.Id, exception);
        }
    }
}
