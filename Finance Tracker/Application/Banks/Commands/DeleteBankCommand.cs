using Application.Banks.Exceptions;
using Application.Common;
using Application.Common.Interfaces.Repositories;
using Domain.Banks;
using Domain.Users;
using MediatR;
using UserNotFoundException = Application.Banks.Exceptions.UserNotFoundException;

namespace Application.Banks.Commands;

public record DeleteBankCommand : IRequest<Result<Bank, BankException>>
{
    public required Guid BankId { get; init; }
}

public class DeleteBankCommandHandler(IBankRepository bankRepository, IUserRepository userRepository)
    : IRequestHandler<DeleteBankCommand, Result<Bank, BankException>>
{
    public async Task<Result<Bank, BankException>> Handle(DeleteBankCommand request,
        CancellationToken cancellationToken)
    {
        var bankId = new BankId(request.BankId);
        var existingBank = await bankRepository.GetById(bankId, cancellationToken);
        return await existingBank.Match<Task<Result<Bank, BankException>>>(async bank =>
            {
                var existingUser = await userRepository.GetById(bank.UserId, cancellationToken);

                return await existingUser.Match<Task<Result<Bank, BankException>>>(
                    async user => await DeleteEntity(user, bank, cancellationToken),
                    () => Task.FromResult<Result<Bank, BankException>>(new UserNotFoundException(bank.UserId)));
            },
            () => Task.FromResult<Result<Bank, BankException>>(new BankNotFoundException(bankId)));
    }

    private async Task<Result<Bank, BankException>> DeleteEntity(User user, Bank bank,
        CancellationToken cancellationToken)
    {
        try
        {
            user.AddToBalance(bank.Balance);
            await userRepository.Update(user, cancellationToken);
            return await bankRepository.Delete(bank, cancellationToken);
        }
        catch (Exception exception)
        {
            return new BankUnknownException(bank.Id, exception);
        }
    }
}