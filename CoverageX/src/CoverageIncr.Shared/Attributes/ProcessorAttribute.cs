namespace CoverageIncr.Shared.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class ProcessorAttribute : Attribute
{
    public string Name { get; set; }
}