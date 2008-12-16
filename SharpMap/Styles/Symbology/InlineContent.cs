using System;
using System.Xml;
using System.Xml.Serialization;

namespace SharpMap.Styles.Symbology
{
    [Serializable]
    [XmlType(Namespace = "http://www.opengis.net/se", TypeName = "InlineContentType")]
    [XmlRoot("InlineContent", Namespace = "http://www.opengis.net/se", IsNullable = false)]
    internal class InlineContent
    {
        private XmlNode[] _any;
        private InlineContentEncoding _encoding;

        [XmlText]
        [XmlAnyElement]
        public XmlNode[] Any
        {
            get { return _any; }
            set { _any = value; }
        }

        [XmlAttribute(AttributeName = "encoding")]
        public InlineContentEncoding Encoding
        {
            get { return _encoding; }
            set { _encoding = value; }
        }
    }

    [Serializable]
    [XmlType(AnonymousType = true, Namespace = "http://www.opengis.net/se")]
    public enum InlineContentEncoding
    {
        [XmlEnum("xml")]
        Xml,
        [XmlEnum("base64")]
        Base64,
    }
}