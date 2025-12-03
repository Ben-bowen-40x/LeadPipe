namespace LeadPipe.Infrastructure.Translate;

public interface IDtoToVo<TDto, TVo>
{
    TVo Translate(TDto data);
}
