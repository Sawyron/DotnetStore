using MediatR;

namespace Application.Core;

public interface IGetPageQuery<T> : IRequest<IEnumerable<T>>
{
    int Offset { get; }
    int PageSize { get; }
}
