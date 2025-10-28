using System.Net;
using System.Text.RegularExpressions;
using FlightPlanner.Domain.Exceptions;

namespace FlightPlanner.Domain.Entities;

public readonly record struct  IataCode(string Code)
{
    public string Code { get; } = string.IsNullOrWhiteSpace(Code) || Code.Length != 3 || !Code.All(char.IsLetter)
        ? throw new GlobalExceptions(HttpStatusCode.BadRequest, "IATA must be 3 letters.")
        : Code.ToUpperInvariant();
}