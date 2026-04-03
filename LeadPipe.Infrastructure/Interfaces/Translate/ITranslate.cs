namespace LeadPipe.Infrastructure.Interfaces.Translate;

public interface ITranslate<TIn, TOut>
{
    TOut Translate(TIn t);
}