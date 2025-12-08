using CSharpFunctionalExtensions;

namespace LeadPipe.Infrastructure.Interfaces.Core;

internal interface ITransform<TIn, TOut>
{
    Task<Result<List<TOut>>> TransformAsync(List<TIn> data);
}
