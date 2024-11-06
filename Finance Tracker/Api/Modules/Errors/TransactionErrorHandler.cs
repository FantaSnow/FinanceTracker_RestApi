using Application.Transactions.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Errors;

public static class TransactionErrorHandler
{
    public static ObjectResult ToObjectResult(this TransactionException exception)
    {
        return new ObjectResult(exception.Message)
        {
            StatusCode = exception switch
            {
                CategoryNotFoundException => StatusCodes.Status404NotFound,
                UserNotFoundException => StatusCodes.Status404NotFound,
                TransactionNotFoundException => StatusCodes.Status404NotFound,
                TransactionAlreadyExistsException => StatusCodes.Status409Conflict,
                TransactionUnknownException => StatusCodes.Status500InternalServerError,
                YouDoNotHaveTheAuthorityToDo => StatusCodes.Status403Forbidden,
                _ => throw new NotImplementedException("Transaction error handler does not implemented")
            }
        };
    }
}