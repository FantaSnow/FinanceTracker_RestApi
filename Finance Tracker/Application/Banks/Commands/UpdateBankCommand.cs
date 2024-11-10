using Application.Banks.Exceptions;
using Application.Common;
using Application.Common.Interfaces.Repositories;
using Domain.Banks;
using Domain.Users;
using MediatR;

namespace Application.Banks.Commands;

public record UpdateBankCommand : IRequest<Result<Bank, BankException>>
{
    public required Guid BankId { get; init; }
    public required string Name { get; init; }
    public required decimal BalanceGoal { get; init; }
    public required Guid UserIdFromToken { get; init; }
}

public class UpdateBankCommandHandler(IBankRepository bankRepository, IUserRepository userRepository)
    : IRequestHandler<UpdateBankCommand, Result<Bank, BankException>>
{
    public async Task<Result<Bank, BankException>> Handle(UpdateBankCommand request,
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
                            async uft =>
                            {   
                                return await UpdateEntity(b,ufb,uft,request.Name, request.BalanceGoal, cancellationToken);
                            },
                            () => Task.FromResult<Result<Bank, BankException>>(new UserNotFoundException(userIdFromToken)));
                    },
                    () => Task.FromResult<Result<Bank, BankException>>(new UserNotFoundException(userFromBankId)));
            },
            () => Task.FromResult<Result<Bank, BankException>>(new BankNotFoundException(bankId)));
    }

    private async Task<Result<Bank, BankException>> UpdateEntity(
        Bank bank,
        User userFromBank,
        User userFromToken,
        string name,
        decimal balanceGoal,
        CancellationToken cancellationToken)
    {
        try
        {
            var existingBank = await bankRepository.GetByNameAndUser(name, userFromBank.Id, bank.Id, cancellationToken);
            
            return await existingBank.Match(
                c => Task.FromResult<Result<Bank, BankException>>(new BankAlreadyExistsException(c.Id)),
                async () =>
                {
                    if (userFromToken.Id == userFromBank.Id || userFromToken.IsAdmin)
                    {
                        bank.UpdateDatails(name, balanceGoal);
                        return await bankRepository.Update(bank, cancellationToken);
                    }
                    return await Task.FromResult<Result<Bank, BankException>>(new YouDoNotHaveTheAuthorityToDo(userFromToken.Id, userFromBank.Id));
                }
            );
        }
        catch (Exception exception)
        {
            return new BankUnknownException(bank.Id, exception);
        }
    }
}