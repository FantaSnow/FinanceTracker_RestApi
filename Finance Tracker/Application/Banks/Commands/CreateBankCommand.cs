using Application.Banks.Exceptions;
using Application.Common;
using Application.Common.Interfaces.Queries;
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
}

public class CreateBankCommandHandler(IBankRepository bankRepository, IBankQueries bankQueries)
    : IRequestHandler<CreateBankCommand, Result<Bank, BankException>>
{
    public async Task<Result<Bank, BankException>> Handle(CreateBankCommand request, CancellationToken cancellationToken)
    {
        var userId = new UserId(request.UserId);
        var existingBank = await bankQueries.GetByNameAndUser(request.Name, userId, cancellationToken);

        return await existingBank.Match(
            c => Task.FromResult<Result<Bank, BankException>>(new BankAlreadyExistsException(c.Id)),
            async () => await CreateEntity(request.Name, request.BalanceGoal, userId, cancellationToken));
    }

    private async Task<Result<Bank, BankException>> CreateEntity(
        string name,
        decimal balanceGoal,
        UserId userId,
        CancellationToken cancellationToken)
    {
        try
        {
            var entity = Bank.New(BankId.New(), name, balanceGoal, userId);

            return await bankRepository.Add(entity, cancellationToken);
        }
        catch (Exception exception)
        {
            return new BankUnknownException(BankId.Empty(), exception);
        }
    }
}