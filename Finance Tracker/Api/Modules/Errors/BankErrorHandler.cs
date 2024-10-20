
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
                _ => throw new NotImplementedException("Course error handler does not implemented")
            }
        };
    }
}