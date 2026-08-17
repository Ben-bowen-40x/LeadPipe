using LeadPipe.Domain.ValueObjects;

namespace LeadPipe.Infrastructure.Attribute;

[AttributeUsage(AttributeTargets.Class)]
internal class SourceKeyAttribute(Source key) : System.Attribute, ISourceKeyAttribute
{
    public Source Key { get; } = key;
}
internal interface ISourceKeyAttribute
{
    public Source Key { get; }
}
