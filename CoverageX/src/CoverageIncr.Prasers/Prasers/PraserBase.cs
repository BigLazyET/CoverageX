using System.Xml;
using System.Xml.Linq;
using CoverageIncr.Shared;

namespace CoverageIncr.Prasers;

public abstract class PraserBase: ICoverageParser
{
    public abstract CoverageFormat Format { get; }

    public abstract IEnumerable<ParserResult> Parse(string filePath, IList<ReportGeneratorFilter> filters);
    
    /// <summary>
    /// Load elements in memory balanced manner.
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="elementName"></param>
    /// <returns></returns>
    protected IEnumerable<XElement> GetXElements(string filePath, string elementName)
    {
        var readerSettings = new XmlReaderSettings() { DtdProcessing = DtdProcessing.Parse, XmlResolver = null };
        using (XmlReader reader = XmlReader.Create(filePath, readerSettings))
        {
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element &&
                    reader.Name == elementName)
                {
                    if (XNode.ReadFrom(reader) is XElement element)
                    {
                        yield return element;
                    }
                }
            }
        }
    }
}