using System.Text.Json;
using FlightPlanner.Domain.Exceptions;

namespace FlightPlanner.Api.Middleware;

record ResponseObject(string Message);

public sealed class GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
{
    public async Task Invoke(HttpContext ctx)
    {
        try
        {
            await next(ctx);
        }
        catch (OperationCanceledException) when (ctx.RequestAborted.IsCancellationRequested)
        {
            if (!ctx.Response.HasStarted)
            {
                ctx.Response.StatusCode = StatusCodes.Status499ClientClosedRequest;
            }
        }
        catch (GlobalExceptions gex)
        {
            if (ctx.Response.HasStarted)
            {
                logger.LogWarning(gex, "GlobalException after response started.");
                throw; 
            }

            logger.LogInformation(gex, $"Handled GlobalException: message: {gex.LogMessage} statusCode: {(int)gex.StatusCode}");

            ctx.Response.ContentType = "application/json; charset=utf-8";
            ctx.Response.StatusCode = (int)gex.StatusCode;

            var response = new ResponseObject(gex.ClientMessage);
            
            var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            });

            await ctx.Response.WriteAsync(json);
        }
    }
}