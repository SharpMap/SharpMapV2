using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Ogc.Kml
{
    /// <remarks/>
    [XmlInclude(typeof (ModelType))]
    [XmlInclude(typeof (PolygonType))]
    [XmlInclude(typeof (LinearRingType))]
    [XmlInclude(typeof (LineStringType))]
    [XmlInclude(typeof (PointType))]
    [XmlInclude(typeof (MultiGeometryType))]
    [GeneratedCode("xsd", "2.0.50727.3038")]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://www.opengis.net/kml/2.2")]
    public abstract class AbstractGeometryType : AbstractObjectType
    {
        private AbstractObjectType[] abstractGeometryObjectExtensionGroupField;
        private string[] abstractGeometrySimpleExtensionGroupField;

        /// <remarks/>
        [XmlElement("AbstractGeometrySimpleExtensionGroup")]
        public string[] AbstractGeometrySimpleExtensionGroup
        {
            get { return abstractGeometrySimpleExtensionGroupField; }
            set { abstractGeometrySimpleExtensionGroupField = value; }
        }

        /// <remarks/>
        [XmlElement("AbstractGeometryObjectExtensionGroup")]
        public AbstractObjectType[] AbstractGeometryObjectExtensionGroup
        {
            get { return abstractGeometryObjectExtensionGroupField; }
            set { abstractGeometryObjectExtensionGroupField = value; }
        }
    }
}