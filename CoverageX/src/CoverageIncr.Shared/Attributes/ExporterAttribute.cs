namespace CoverageIncr.Shared.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class ExporterAttribute : Attribute
{
    public string Name { get; set; }
    
    public Type OptionType { get; set; }
    

    public ExporterAttribute(string name, Type optionType)
    {
        Name = name;
        OptionType = optionType;
    }
}