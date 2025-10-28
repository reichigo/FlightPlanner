using System.Net;

namespace FlightPlanner.Domain.Exceptions;

public class GlobalExceptions(HttpStatusCode statusCode, string clientMessage, string? logMessage = null) : Exception(logMessage)
{
    public HttpStatusCode StatusCode { get; } = statusCode;
    public string ClientMessage { get; } = clientMessage;
    public string? LogMessage { get; } = logMessage;
}