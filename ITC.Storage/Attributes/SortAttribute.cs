namespace ITC.Storage.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class SortAttribute(string? sortPath = null) : Attribute
{
    public string? SortPath { get; } = sortPath;
}