using CSharpFunctionalExtensions;

namespace LeadPipe.Infrastructure.Interface.Core;

internal interface ITransform<TIn, TOut>
{
    Task<Result<List<TOut>>> TransformAsync(List<TIn> data);
}
