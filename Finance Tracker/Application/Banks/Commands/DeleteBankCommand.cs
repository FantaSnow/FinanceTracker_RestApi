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
    public required Guid UserIdFromToken { get; init; }
}

public class DeleteBankCommandHandler(IBankRepository bankRepository, IUserRepository userRepository)
    : IRequestHandler<DeleteBankCommand, Result<Bank, BankException>>
{
    public async Task<Result<Bank, BankException>> Handle(DeleteBankCommand request,
        CancellationToken cancellationToken)
    {
        var bankId = new BankId(request.BankId);
        var existingBank = await bankRepository.GetById(bankId, cancellationToken);

        return await existingBank.Match<Task<Result<Bank, BankException>>>(
            async b =>
            {
                var userFromBankId = b.UserId;
                var existingUserFromBank = await userRepository.GetById(userFromBankId, cancellationToken);

                return await existingUserFromBank.Match<Task<Result<Bank, BankException>>>(
                    async ufb =>
                    {
                        var userIdFromToken = new UserId(request.UserIdFromToken);
                        var existingUserFromToken = await userRepository.GetById(userIdFromToken, cancellationToken);

                        return await existingUserFromToken.Match<Task<Result<Bank, BankException>>>(
                            async uft => await DeleteEntity(ufb,uft, b, cancellationToken),
                            () => Task.FromResult<Result<Bank, BankException>>(new UserNotFoundException(b.UserId)));
                    },
                    () => Task.FromResult<Result<Bank, BankException>>(new UserNotFoundException(userFromBankId)));
            },
            () => Task.FromResult<Result<Bank, BankException>>(new BankNotFoundException(bankId)));
    }

    private async Task<Result<Bank, BankException>> DeleteEntity(User userFromBank,User userFromToken, Bank bank,
        CancellationToken cancellationToken)
    {
        try
        {
            if (userFromToken.Id == userFromBank.Id || userFromToken.IsAdmin)
            {
                userFromBank.AddToBalance(bank.Balance);
                await userRepository.Update(userFromBank, cancellationToken);
                return await bankRepository.Delete(bank, cancellationToken);
            }

            return await Task.FromResult<Result<Bank, BankException>>(new YouDoNotHaveTheAuthorityToDo(userFromToken.Id, userFromBank.Id));
        }
        catch (Exception exception)
        {
            return new BankUnknownException(bank.Id, exception);
        }
    }
}