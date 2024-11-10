using Application.Statistics.Exceptions;
using Domain.Categorys;
using Domain.Users;

namespace Application.Categorys.Exceptions;

public abstract class CategoryException : Exception
{
    public CategoryId? CategoryId { get; }
    public UserId? UserId { get; }

    protected CategoryException(CategoryId? categoryId, string message, Exception? innerException = null)
        : base(message, innerException)
    {
        CategoryId = categoryId;
    }

    protected CategoryException(UserId? userId, string message, Exception? innerException = null)
        : base(message, innerException)
    {
        UserId = userId;
    }
}

public class CategoryNotFoundException(CategoryId id) : CategoryException(id, $"Category under id: {id} not found");

public class CategoryAlreadyExistsException(CategoryId id) : CategoryException(id, $"Category already exists: {id}");

public class UserNotFoundException(UserId id) : CategoryException(id, $"User under id: {id} not found");
public class YouDoNotHaveTheAuthorityToDo : CategoryException
{
    public YouDoNotHaveTheAuthorityToDo(UserId idFromToken, UserId idFromUser)
        : base(idFromToken, $"You do not have the authority to do. Your id:{idFromToken} | Id what u want to change: {idFromUser}") { }

    public YouDoNotHaveTheAuthorityToDo(UserId idFromToken)
        : base(idFromToken, $"You do not have the authority to do. Your id:{idFromToken}") { }
}

public class CategoryUnknownException(CategoryId id, Exception innerException)
    : CategoryException(id, $"Unknown exception for the category under id: {id}", innerException);