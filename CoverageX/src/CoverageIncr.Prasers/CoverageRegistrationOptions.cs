namespace CoverageIncr.Prasers;

public class CoverageRegistrationOptions
{
    public bool OverrideIfExists { get; set; } = false;
    public HashSet<CoverageFormat> DisabledFormats { get; set; } = new();
}