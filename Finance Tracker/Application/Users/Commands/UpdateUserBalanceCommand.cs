using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.Users.Exceptions;
using Domain.Users;
using MediatR;

namespace Application.Users.Commands;

public record UpdateUserBalanceCommand : IRequest<Result<User, UserException>>
{
    public required Guid UserId { get; init; }
    public required decimal Balance { get; init; }
    public required Guid UserIdFromToken { get; init; }

}

public class UpdateUserBalanceCommandHandler(IUserRepository userRepository)
    : IRequestHandler<UpdateUserBalanceCommand, Result<User, UserException>>
{
    public async Task<Result<User, UserException>> Handle(UpdateUserBalanceCommand request,
        CancellationToken cancellationToken)
    {
        var userId = new UserId(request.UserId);
        var existingUser = await userRepository.GetById(userId, cancellationToken);

        return await existingUser.Match<Task<Result<User, UserException>>>(
            async u =>
            {
                var userIdFromToken = new UserId(request.UserIdFromToken);
                var existingUserFromToken = await userRepository.GetById(userIdFromToken, cancellationToken);

                return await existingUserFromToken.Match<Task<Result<User, UserException>>>(
                    async userFromToken => await UpdateEntity(u, userFromToken, request.Balance, cancellationToken),
                    () => Task.FromResult<Result<User, UserException>>(new UserNotFoundException(userIdFromToken))
                );   
            },
            () => Task.FromResult<Result<User, UserException>>(new UserNotFoundException(userId)));
    }

    private async Task<Result<User, UserException>> UpdateEntity(
        User user,
        User userFromToken,
        decimal balance,
        CancellationToken cancellationToken)
    {
        try
        {
            if (userFromToken.Id == user.Id || userFromToken.IsAdmin)
            {
                user.AddToBalance(balance);
                return await userRepository.Update(user, cancellationToken);
            }

            return await Task.FromResult<Result<User, UserException>>(
                new YouDoNotHaveTheAuthorityToDo(userFromToken.Id, user.Id)
            );
        }
        catch (Exception exception)
        {
            return new UserUnknownException(user.Id, exception);
        }
    }
}