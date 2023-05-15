namespace Application.Core;

public record GetPageQuery<T>(int Offset, int PageSize) : IGetPageQuery<T>;
