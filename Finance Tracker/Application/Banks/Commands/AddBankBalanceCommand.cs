using Application.Banks.Exceptions;
using Application.Common;
using Application.Common.Interfaces.Repositories;
using Domain.Banks;
using Domain.Users;
using MediatR;

namespace Application.Banks.Commands;

public record AddBankBalanceCommand : IRequest<Result<Bank, BankException>>
{
    public required Guid BankId { get; init; }
    public required decimal BalanceToAdd { get; init; }
    public required Guid UserIdFromToken { get; init; }
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
                var userFromBankId = b.UserId;
                var existingUserFromBank = await userRepository.GetById(userFromBankId, cancellationToken);

                return await existingUserFromBank.Match(
                    async ufb =>
                    {
                        var userIdFromToken = new UserId(request.UserIdFromToken);
                        var existingUserFromToken = await userRepository.GetById(userIdFromToken, cancellationToken);

                        return await existingUserFromToken.Match<Task<Result<Bank, BankException>>>(
                            async uft => await UpdateBalance(b, ufb, uft, request.BalanceToAdd, cancellationToken),
                            () => Task.FromResult<Result<Bank, BankException>>(new UserNotFoundException(b.UserId)));
                    },
                    () => Task.FromResult<Result<Bank, BankException>>(new UserNotFoundException(userFromBankId)));
            },
            () => Task.FromResult<Result<Bank, BankException>>(new BankNotFoundException(bankId)));
    }

    private async Task<Result<Bank, BankException>> UpdateBalance(
        Bank bank,
        User userFromBank,
        User userFromToken,
        decimal balanceToAdd,
        CancellationToken cancellationToken)
    {
        try
        {
            if (userFromToken.Id == userFromBank.Id || userFromToken.IsAdmin)
            {
                userFromBank.AddToBalance(-balanceToAdd);
                await userRepository.Update(userFromBank, cancellationToken);
                bank.AddToBalance(balanceToAdd);
                var updatedBank = await bankRepository.Update(bank, cancellationToken);
                return updatedBank;
            }

            return await Task.FromResult<Result<Bank, BankException>>(
                new YouDoNotHaveTheAuthorityToDo(userFromToken.Id, userFromBank.Id));
        }
        catch (Exception exception)
        {
            return new BankUnknownException(BankId.Empty(), exception);
        }
    }
}