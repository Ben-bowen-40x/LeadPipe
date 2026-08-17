namespace LeadPipe.Infrastructure.Attribute;

[AttributeUsage(AttributeTargets.Class)]
internal sealed class ScheduleKeyAttribute(Schedule key) : System.Attribute, IScheduleKeyAttribute
{
    public Schedule Key { get; } = key;
}

internal interface IScheduleKeyAttribute
{
    public Schedule Key { get; }
}