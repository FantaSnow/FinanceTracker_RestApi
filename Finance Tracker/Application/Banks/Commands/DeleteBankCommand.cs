using System.Transactions;
using Application.Banks.Exceptions;
using Application.Common;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Users.Exceptions;
using Domain.Banks;
using Domain.Users;
using MediatR;

namespace Application.Banks.Commands;

public record DeleteBankCommand : IRequest<Result<Bank, BankException>>
{
    public required Guid BankId { get; init; }
}

public class DeleteBankCommandHandler(IBankRepository bankRepository, IBankQueries bankQueries, IUserRepository userRepository, IUserQueries userQueries)
    : IRequestHandler<DeleteBankCommand, Result<Bank, BankException>>
{
    public async Task<Result<Bank, BankException>> Handle(DeleteBankCommand request, CancellationToken cancellationToken)
    {
        var bankId = new BankId(request.BankId);
        var existingBank = await bankQueries.GetById(bankId, cancellationToken);
        return await existingBank.Match<Task<Result<Bank, BankException>>>(async bank =>
        {
            var existingUser = await userQueries.GetById(bank.UserId, cancellationToken);
            return await existingUser.Match<Task<Result<Bank, BankException>>>(async user =>
            {
                return await ExecuteTransactionAsync(user, bank, cancellationToken);
            },
            () => Task.FromResult<Result<Bank, BankException>>(new BankUpdateFailedException(bankId, new UserNotFoundException(bank.UserId))));
        },
        () => Task.FromResult<Result<Bank, BankException>>(new BankNotFoundException(bankId)));
    }

    private async Task<Result<Bank, BankException>> ExecuteTransactionAsync(User user, Bank bank, CancellationToken cancellationToken)
    {
        using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            var updateBalanceResult = await UpdateBalance(user, bank, cancellationToken);
            return await updateBalanceResult.Match<Task<Result<Bank, BankException>>>(
                async _ =>
            {
                var deleteBankResult = await DeleteEntity(bank, cancellationToken);
                return await deleteBankResult.Match<Task<Result<Bank, BankException>>>(
                    bankDeleted =>
                {
                    scope.Complete();
                    return Task.FromResult<Result<Bank, BankException>>(bankDeleted);
                },
                bankException => Task.FromResult<Result<Bank, BankException>>(bankException));
            },
            userException => Task.FromResult<Result<Bank, BankException>>(new BankUpdateFailedException(bank.Id, userException)));
        }
    }

    private async Task<Result<Bank, BankException>> DeleteEntity(Bank bank, CancellationToken cancellationToken)
    {
        try
        {
            return await bankRepository.Delete(bank, cancellationToken);
        }
        catch (Exception exception)
        {
            return new BankUnknownException(bank.Id, exception);
        }
    }

    private async Task<Result<User, UserException>> UpdateBalance(User user, Bank bank, CancellationToken cancellationToken)
    {
        try
        {
            user.AddToBalance(bank.Balance);
            return await userRepository.Update(user, cancellationToken);
        }
        catch (Exception exception)
        {
            return new UserUnknownException(UserId.Empty(), exception);
        }
    }
}
