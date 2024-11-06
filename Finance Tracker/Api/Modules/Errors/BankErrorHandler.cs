using Application.Banks.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Errors;

public static class BankErrorHandler
{
    public static ObjectResult ToObjectResult(this BankException exception)
    {
        return new ObjectResult(exception.Message)
        {
            StatusCode = exception switch
            {
                BankNotFoundException => StatusCodes.Status404NotFound,
                BankAlreadyExistsException => StatusCodes.Status409Conflict,
                BankUnknownException => StatusCodes.Status500InternalServerError,
                UserNotFoundException => StatusCodes.Status404NotFound,
                YouDoNotHaveTheAuthorityToDo => StatusCodes.Status403Forbidden,
                _ => throw new NotImplementedException("Bank error handler does not implemented")
            }
        };
    }
}