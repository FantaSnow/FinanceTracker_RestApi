using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.Users.Exceptions;
using Domain.Users;
using MediatR;

namespace Application.Users.Commands;

public record UpdateUserCommand : IRequest<Result<User, UserException>>
{
    public required Guid UserId { get; init; }
    public required string Login { get; init; }
    public required string Password { get; init; }
    public required decimal Balance { get; init; }
}

public class UpdateUserCommandHandler(IUserRepository userRepository)
    : IRequestHandler<UpdateUserCommand, Result<User, UserException>>
{
    public async Task<Result<User, UserException>> Handle(UpdateUserCommand request,
        CancellationToken cancellationToken)
    {
        var userId = new UserId(request.UserId);

        var existingUser = await userRepository.GetById(userId, cancellationToken);

        return await existingUser.Match(
            async u =>
            {
                var existingUserForDublicate = await userRepository.GetByLogin(request.Login, cancellationToken);

                return await existingUserForDublicate.Match<Task<Result<User, UserException>>>(
                    d => Task.FromResult<Result<User, UserException>>(new UserAlreadyExistsException(d.Id)),
                    async () => await UpdateEntity(u, request.Login, request.Password, request.Balance,
                        cancellationToken));
            },
            () => Task.FromResult<Result<User, UserException>>(new UserNotFoundException(userId)));
    }

    private async Task<Result<User, UserException>> UpdateEntity(
        User entity,
        string login,
        string password,
        decimal balance,
        CancellationToken cancellationToken)
    {
        try
        {
            entity.ChangeLogin(login);
            entity.ChangePassword(password);
            entity.SetBalance(balance);

            return await userRepository.Update(entity, cancellationToken);
        }
        catch (Exception exception)
        {
            return new UserUnknownException(entity.Id, exception);
        }
    }
}