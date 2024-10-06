using System.Diagnostics;
using System.Text;
using ItemStorage.Data;
using ItemStorage.Data.Entities;
using Microsoft.IO;

namespace ItemStorage.Middleware;

public class RequestResponseLoggingMiddleware(RequestDelegate next)
{
    private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager = new();

    public async Task Invoke(HttpContext context, IDataContextProvider dataContextProvider)
    {
        var requetsDate = DateTime.UtcNow;

        var originalBodyStream = context.Response.Body;

        var request = await GetRequestAsync(context.Request);

        await using var responseBody = _recyclableMemoryStreamManager.GetStream();
        context.Response.Body = responseBody;

        Stopwatch watch = Stopwatch.StartNew();
        await next(context);
        watch.Stop();

        var response = await GetResponseAsync(context.Response);

        await using var dbContext = dataContextProvider.GetContext();
        await dbContext.RequestResponseLogs.AddAsync(new RequestResponseLog()
        {
            Duration = watch.ElapsedMilliseconds,
            Request = request,
            RequestDate = requetsDate,
            Response = response,
            ResponseCode = context.Response.StatusCode
        });
        await dbContext.SaveChangesAsync();

        await context.Response.Body.CopyToAsync(originalBodyStream);
    }

    private async Task<string> GetResponseAsync(HttpResponse response)
    {
        response.Body.Seek(0, SeekOrigin.Begin);

        var text = await new StreamReader(response.Body).ReadToEndAsync();
        response.Body.Seek(0, SeekOrigin.Begin);

        return text;
    }

    private async Task<string> GetRequestAsync(HttpRequest request)
    {
        request.EnableBuffering();

        var buffer = new byte[Convert.ToInt32(request.ContentLength)];

        await request.Body.ReadAsync(buffer, 0, buffer.Length);

        var bodyText = Encoding.UTF8.GetString(buffer);

        request.Body.Position = 0;

        return
            $"Scheme: {request.Scheme}\nMethod: {request.Method}\nUrl: {request.Host}{request.Path}\nQueryString: {request.QueryString}\nBody: {bodyText}";
    }
}