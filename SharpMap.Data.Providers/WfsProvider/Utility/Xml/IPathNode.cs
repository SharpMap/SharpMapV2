using System.Xml;

namespace SharpMap.Data.Providers.WfsProvider.Utility.Xml
{
    internal interface IPathNode
    {
        bool IsActive { get; set; }
        bool Matches(XmlReader reader);
    }
}