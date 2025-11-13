namespace LeadPipe.Application.Service;

public interface IDateTimeConversionService
{
    DateTimeOffset Convert(DateTime date, TimeSpan offset);
    DateTimeOffset Convert(DateTime localTime, ETimeZone zone);
    bool Convert(DateTime localTime, ETimeZone zone, out DateTimeOffset result);
}
public enum ETimeZone
{
    Pacific,
    Mountain,
    Central,
    Eastern
}
