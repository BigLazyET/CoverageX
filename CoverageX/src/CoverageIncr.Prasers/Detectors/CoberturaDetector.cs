namespace CoverageIncr.Prasers;

public class CoberturaDetector : DetectorBase
{
    public override CoverageFormat Format => CoverageFormat.Cobertura;
    public override int DetectConfidence(string filePath)
    {
        var elements = GetXElements(filePath, "coverage");
        if (elements.Any(element => element.Attributes().Count() > 1 || element.Elements("packages").Any()))
            return 100;

        return -1;
    }
}