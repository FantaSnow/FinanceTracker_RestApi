using Application.Banks.Exceptions;
using Application.Common;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.Banks;
using Domain.Users;
using MediatR;

namespace Application.Banks.Commands;

public record AddBankBalanceCommand : IRequest<Result<Bank, BankException>>
{
    public required Guid BankId { get; init; }
    public required decimal BalanceToAdd { get; init; }
}

public class AddBankBalanceCommandHandler(IBankRepository bankRepository, IUserRepository userRepository)
    : IRequestHandler<AddBankBalanceCommand, Result<Bank, BankException>>
{
    public async Task<Result<Bank, BankException>> Handle(AddBankBalanceCommand request,
        CancellationToken cancellationToken)
    {
        var bankId = new BankId(request.BankId);
        var existingBank = await bankRepository.GetById(bankId, cancellationToken);

        return await existingBank.Match(
            async b =>
            {
                var existingUser = await userRepository.GetById(b.UserId, cancellationToken);
                return await existingUser.Match(
                    async u => await UpdateBalance(b, u, request.BalanceToAdd, cancellationToken),
                    () => Task.FromResult<Result<Bank, BankException>>(new UserNotFoundException(b.UserId)));
            },
            () => Task.FromResult<Result<Bank, BankException>>(new BankNotFoundException(bankId)));
    }

    private async Task<Result<Bank, BankException>> UpdateBalance(
        Bank bank,
        User user,
        decimal balanceToAdd,
        CancellationToken cancellationToken)
    {
        try
        {
            user.AddToBalance(-balanceToAdd);
            await userRepository.Update(user, cancellationToken);
            bank.AddToBalance(balanceToAdd);
            return await bankRepository.Update(bank, cancellationToken);
        }
        catch (Exception exception)
        {
            return new BankUnknownException(BankId.Empty(), exception);
        }
    }
}