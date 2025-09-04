namespace CoverageIncr.Shared.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class ReceiverAttribute : Attribute
{
    public string Name { get; set; }
    
    public Type OptionType { get; set; }
}