namespace LeadPipe.Infrastructure.Attributes;

[AttributeUsage(AttributeTargets.Class)]
internal sealed class ScheduleKeyAttribute(Schedule key) : Attribute, IScheduleKeyAttribute
{
    public Schedule Key { get; } = key;
}

internal interface IScheduleKeyAttribute
{
    public Schedule Key { get; }
}