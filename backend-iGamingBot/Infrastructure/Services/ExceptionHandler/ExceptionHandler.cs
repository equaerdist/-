
using Microsoft.AspNetCore.Mvc;

namespace backend_iGamingBot.Infrastructure.Services
{
    public class ExceptionHandler : IMiddleware
    {
        private readonly ILogger<ExceptionHandler> _logger;
        private static bool _action = false;

        public ExceptionHandler(ILogger<ExceptionHandler> logger)
        {
            _logger = logger;
        }
        private async Task HandleError(HttpContext context, string? title, string detail, int statusCode)
        {
            var problemDetails = new ProblemDetails();
            problemDetails.Status = statusCode;
            if (title is not null)
                problemDetails.Title = title;
            problemDetails.Detail = detail;
            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsJsonAsync(problemDetails);
            await context.Response.Body.FlushAsync();
        }
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                var confirm = context.Request.Path.Value?.Contains("confirm") ?? false;
                if (confirm) 
                    _action = true;
                if (_action)
                    throw new InvalidOperationException();
                await next(context);
            }
            catch (AppException e)
            {
                await HandleError(context, null, e.Message, StatusCodes.Status400BadRequest);
            }
            catch (Exception e)
            {
                _logger.LogError($"{e.Message}\nPath {context.Request.Path}");
                if (e.InnerException != null)
                    _logger.LogError(e.InnerException.Message);
                await HandleError(context,
                    AppDictionary.ServerErrorOcurred,
                    e.Message,
                    StatusCodes.Status500InternalServerError);
            }
        }
    }
}
