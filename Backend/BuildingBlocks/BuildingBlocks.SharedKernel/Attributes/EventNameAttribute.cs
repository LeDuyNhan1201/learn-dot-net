namespace BuildingBlocks.SharedKernel.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public sealed class EventNameAttribute(string name) : Attribute
{
    public string Name { get; } = name;
}