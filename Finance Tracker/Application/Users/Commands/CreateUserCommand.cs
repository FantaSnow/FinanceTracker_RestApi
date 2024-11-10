using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.Users.Exceptions;
using Domain.Users;
using MediatR;

namespace Application.Users.Commands;

public record CreateUserCommand : IRequest<Result<User, UserException>>
{
    public required string Login { get; init; }
    public required string Password { get; init; }
}

public class CreateUserCommandHandler(IUserRepository userRepository)
    : IRequestHandler<CreateUserCommand, Result<User, UserException>>
{
    public async Task<Result<User, UserException>> Handle(CreateUserCommand request,
        CancellationToken cancellationToken)
    {
        var existingUser = await userRepository.GetByLogin(request.Login, cancellationToken);

        return await existingUser.Match<Task<Result<User, UserException>>>(
            u => Task.FromResult<Result<User, UserException>>(new UserAlreadyExistsException(u.Id)),
            async () => await CreateEntity(request.Login, request.Password, cancellationToken));
    }

    private async Task<Result<User, UserException>> CreateEntity(
        string login,
        string password,
        CancellationToken cancellationToken)
    {
        try
        {
            var entity = User.New(UserId.New(), login, password);

            return await userRepository.Add(entity, cancellationToken);
        }
        catch (Exception exception)
        {
            return new UserUnknownException(UserId.Empty(), exception);
        }
    }
}