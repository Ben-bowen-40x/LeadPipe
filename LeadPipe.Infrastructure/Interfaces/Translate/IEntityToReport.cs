namespace LeadPipe.Infrastructure.Interfaces.Translate;

public interface IEntityToReport<TEntity, TReport>
{
    TReport Translate(TEntity data);
}