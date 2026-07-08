using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ActorApp.Controllers;

public class ErrorFilterAttribute : ExceptionFilterAttribute
{
  public override void OnException(ExceptionContext context)
  {
    context.Result = new OkObjectResult(new
    {
      Error = context.Exception.ToString()
    });
  }
}
