namespace CoverageIncr.Receivers.Options;

public class GlobbingOption
{
    public IEnumerable<GlobbingSegment> Segments { get; set; }
}

public class GlobbingSegment
{
    public string BaseDir { get; set; }

    public string Glob { get; set; }
}