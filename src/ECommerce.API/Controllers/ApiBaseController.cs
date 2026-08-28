using ECommerce.Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiBaseController : ControllerBase
    {
        public static IActionResult ToActionResult<T>(Result<T> result)
        {
            if (result.IsSuccess)
                return new OkObjectResult(result.data);

            return ToProblem(result.Error);

        }

        public static IActionResult ToActionResult(Result result)
        {
            if (result.IsSuccess)
                return new OkObjectResult(result);

            return ToProblem(result.Error);

        }


        public static ObjectResult ToProblem(IReadOnlyList<Error> Errors)
        {
            var first = Errors[0];

            var status = first.Type switch
            {
                ErrorType.NotFound => StatusCodes.Status404NotFound,
                ErrorType.Validation => StatusCodes.Status400BadRequest,
                ErrorType.Conflict => StatusCodes.Status409Conflict,
                ErrorType.Failure => StatusCodes.Status500InternalServerError,
                _ => StatusCodes.Status500InternalServerError
            };

            var problem = new ProblemDetails
            {
                Status = status,
                Type = first.code,
                Detail = first.Description,
                Extensions = { ["errors"] = Errors }
            };

            return new ObjectResult(problem) { StatusCode = status };



        }
    }
}
