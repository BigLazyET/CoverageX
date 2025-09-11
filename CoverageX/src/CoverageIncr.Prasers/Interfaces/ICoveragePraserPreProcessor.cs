using System.Xml.Linq;

namespace CoverageIncr.Prasers;

public interface ICoveragePraserPreProcessor
{
    Task PreProcess(XElement element);
}