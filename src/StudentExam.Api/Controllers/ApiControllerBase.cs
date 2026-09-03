using Microsoft.AspNetCore.Mvc;
using StudentExam.Application.Common;

namespace StudentExam.Api.Controllers;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    protected IActionResult FromResult(ServiceResult result) =>
        result.Succeeded ? NoContent() : MapError(result.Error!, result.ErrorType);

    protected ActionResult<T> FromResult<T>(ServiceResult<T> result) =>
        result.Succeeded ? Ok(result.Data) : MapError(result.Error!, result.ErrorType);

    private ObjectResult MapError(string error, ServiceErrorType errorType) => errorType switch
    {
        ServiceErrorType.NotFound => NotFound(error),
        ServiceErrorType.Conflict => Conflict(error),
        ServiceErrorType.Validation => BadRequest(error),
        _ => Problem(error)
    };
}
