using System.Net;
using System.Text.Json;

namespace UMS.UI.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<ExceptionHandlingMiddleware> logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            this.next = next;
            this.logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (KeyNotFoundException ex)
            {
                logger.LogWarning(ex, "Resource not found");
                await WriteResponse(context, HttpStatusCode.NotFound, ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                logger.LogWarning(ex, "Invalid operation");
                await WriteResponse(context, HttpStatusCode.BadRequest, ex.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unhandled exception");
                await WriteResponse(context, HttpStatusCode.InternalServerError, "An unexpected error occurred.");
            }
        }

        private static async Task WriteResponse(HttpContext context, HttpStatusCode code, string message)
        {
            context.Response.StatusCode = (int)code;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new { error = message }));
        }
    }
}
