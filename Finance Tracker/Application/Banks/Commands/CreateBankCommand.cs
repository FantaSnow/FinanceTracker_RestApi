using Application.Banks.Exceptions;
using Application.Common;
using Application.Common.Interfaces.Repositories;
using Domain.Banks;
using Domain.Users;
using MediatR;

namespace Application.Banks.Commands;

public record CreateBankCommand : IRequest<Result<Bank, BankException>>
{
    public required string Name { get; init; }
    public required decimal BalanceGoal { get; init; }
    public required Guid UserId { get; init; }
    public required Guid UserIdFromToken { get; init; }
}

public class CreateBankCommandHandler(IBankRepository bankRepository, IUserRepository userRepository)
    : IRequestHandler<CreateBankCommand, Result<Bank, BankException>>
{
    public async Task<Result<Bank, BankException>> Handle(CreateBankCommand request,
        CancellationToken cancellationToken)
    {
        var userForBankId = new UserId(request.UserId);
        var existingUserFromBank = await userRepository.GetById(userForBankId, cancellationToken);

        return await existingUserFromBank.Match<Task<Result<Bank, BankException>>>(
            async ufb =>
            {
                var userIdFromToken = new UserId(request.UserIdFromToken);
                var existingUserFromToken = await userRepository.GetById(userIdFromToken, cancellationToken);

                return await existingUserFromToken.Match<Task<Result<Bank, BankException>>>(
                    async uft => await CreateEntity(request.Name, request.BalanceGoal, ufb, uft,
                        cancellationToken),
                    () => Task.FromResult<Result<Bank, BankException>>(
                        new UserNotFoundException(userIdFromToken)));
            },
            () => Task.FromResult<Result<Bank, BankException>>(new UserNotFoundException(userForBankId)));
    }

    private async Task<Result<Bank, BankException>> CreateEntity(
        string name,
        decimal balanceGoal,
        User userForBank,
        User userFromToken,
        CancellationToken cancellationToken)
    {
        try
        {
            var existingBank = await bankRepository.GetByNameAndUser(name, userForBank.Id, cancellationToken);

            return await existingBank.Match(
                b => Task.FromResult<Result<Bank, BankException>>(new BankAlreadyExistsException(b.Id)),
                async () =>
                {
                    if (userFromToken.Id == userForBank.Id || userFromToken.IsAdmin)
                    {
                        var entity = Bank.New(BankId.New(), name, balanceGoal, userForBank.Id);

                        return await bankRepository.Add(entity, cancellationToken);
                    }

                    return await Task.FromResult<Result<Bank, BankException>>(
                        new YouDoNotHaveTheAuthorityToDo(userFromToken.Id, userForBank.Id));
                });
        }
        catch (Exception exception)
        {
            return new BankUnknownException(BankId.Empty(), exception);
        }
    }
}