namespace LeadPipe.Infrastructure.Interface.Translate;

public interface ITranslate<TIn, TOut>
{
    TOut Translate(TIn t);
}