using Microsoft.AspNetCore.Mvc;
using OnBoardy.API.Results;

namespace OnBoardy.API.Extensions
{
    public static class ResultHttpExtensions
    {
        extension(ControllerBase controller)
        {
            public ActionResult ToProblem(Error error) {
                return controller.StatusCode(error.StatusCode, new ProblemDetails
                {
                    Status = error.StatusCode,
                    Title = error.Code,
                    Detail = error.Message,
                    Instance = controller.HttpContext.Request.Path
                });
            }
        }
    }
}