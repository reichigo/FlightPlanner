namespace FlightPlanner.Application.Dto.Response;

public sealed record PagedResponse<T>(
    List<T> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages
);

