using System.Text.Json;
using ItemStorage.Models;

namespace ItemStorage.Middleware;

public class ExceptionHandlerMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next.Invoke(context);
        }
        catch(Exception ex)
        {
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";

            var error = new ErrorDetails
            {
                Message = ex.Message
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(error));
        }
    }
}