using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Ogc.Kml
{
    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.3038")]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://www.w3.org/2005/Atom")]
    [XmlRoot(Namespace = "http://www.w3.org/2005/Atom", IsNullable = false)]
    public class link
    {
        private string hrefField;

        private string hreflangField;

        private string lengthField;
        private string relField;
        private string titleField;
        private string typeField;

        /// <remarks/>
        [XmlAttribute]
        public string href
        {
            get { return hrefField; }
            set { hrefField = value; }
        }

        /// <remarks/>
        [XmlAttribute]
        public string rel
        {
            get { return relField; }
            set { relField = value; }
        }

        /// <remarks/>
        [XmlAttribute]
        public string type
        {
            get { return typeField; }
            set { typeField = value; }
        }

        /// <remarks/>
        [XmlAttribute]
        public string hreflang
        {
            get { return hreflangField; }
            set { hreflangField = value; }
        }

        /// <remarks/>
        [XmlAttribute]
        public string title
        {
            get { return titleField; }
            set { titleField = value; }
        }

        /// <remarks/>
        [XmlAttribute]
        public string length
        {
            get { return lengthField; }
            set { lengthField = value; }
        }
    }
}