namespace WebLaunchPad.Api.Middlewares;

/// <see href="https://stackoverflow.com/a/72825700"/>
public class TaskCancellationMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (TaskCanceledException)
        {
            // Set StatusCode 499 Client Closed Request
            context.Response.StatusCode = 499;
        }
    }
}