using Application.Common;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Users.Exceptions;
using Domain.Users;
using MediatR;

namespace Application.Users.Commands;

public record UpdateUserBalanceCommand : IRequest<Result<User, UserException>>
{
    public required Guid UserId { get; init; }
    public required decimal Balance { get; init; }
}

public class UpdateUserBalanceCommandHandler(IUserRepository userRepository, IUserQueries userQueries) : IRequestHandler<UpdateUserCommand, Result<User, UserException>>
{
    public async Task<Result<User, UserException>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var userId = new UserId(request.UserId);

        var existingUser = await userQueries.GetById(userId, cancellationToken);

        return await existingUser.Match(
            async u => await UpdateEntity(u, request.Balance, cancellationToken),
            () => Task.FromResult<Result<User, UserException>>(new UserNotFoundException(userId)));
    }

    private async Task<Result<User, UserException>> UpdateEntity(
        User entity,
        decimal balance,
        CancellationToken cancellationToken)
    {
        try
        {
            entity.AddToBalance(balance);

            return await userRepository.Update(entity, cancellationToken);
        }
        catch (Exception exception)
        {
            return new UserUnknownException(entity.Id, exception);
        }
    }
}