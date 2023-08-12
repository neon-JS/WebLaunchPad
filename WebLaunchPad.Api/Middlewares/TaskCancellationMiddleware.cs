namespace WebLaunchPad.Api.Middlewares;

/// <see href="https://stackoverflow.com/a/72825700"/>
public class TaskCancellationMiddleware
{
    private readonly RequestDelegate _next;

    public TaskCancellationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (TaskCanceledException)
        {
            // Set StatusCode 499 Client Closed Request
            context.Response.StatusCode = 499;
        }
    }
}