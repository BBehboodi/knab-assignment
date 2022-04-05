using Knab.Assignment.API.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Knab.Assignment.API.ActionFilters
{
    public class HttpResponseExceptionFilter : IActionFilter, IOrderedFilter
    {
        public int Order { get; } = int.MaxValue;

        public void OnActionExecuting(ActionExecutingContext context)
        { }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception is not null)
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<HttpResponseExceptionFilter>>();
                logger.LogError(context.Exception, "An error has been occurred");

                if (context.Exception is HttpResponseException httpResponseException)
                {
                    context.Result = new ObjectResult(httpResponseException.Message)
                    {
                        StatusCode = httpResponseException.StatusCode
                    };

                    context.ExceptionHandled = true;
                }
            }
        }
    }
}