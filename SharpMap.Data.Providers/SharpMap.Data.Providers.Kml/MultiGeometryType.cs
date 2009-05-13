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
    [XmlType(Namespace = "http://www.opengis.net/kml/2.2")]
    [XmlRoot("MultiGeometry", Namespace = "http://www.opengis.net/kml/2.2", IsNullable = false)]
    public class MultiGeometryType : AbstractGeometryType
    {
        private AbstractGeometryType[] itemsField;

        private AbstractObjectType[] multiGeometryObjectExtensionGroupField;
        private string[] multiGeometrySimpleExtensionGroupField;

        /// <remarks/>
        [XmlElement("LineString", typeof (LineStringType))]
        [XmlElement("LinearRing", typeof (LinearRingType))]
        [XmlElement("Model", typeof (ModelType))]
        [XmlElement("MultiGeometry", typeof (MultiGeometryType))]
        [XmlElement("Point", typeof (PointType))]
        [XmlElement("Polygon", typeof (PolygonType))]
        public AbstractGeometryType[] Items
        {
            get { return itemsField; }
            set { itemsField = value; }
        }

        /// <remarks/>
        [XmlElement("MultiGeometrySimpleExtensionGroup")]
        public string[] MultiGeometrySimpleExtensionGroup
        {
            get { return multiGeometrySimpleExtensionGroupField; }
            set { multiGeometrySimpleExtensionGroupField = value; }
        }

        /// <remarks/>
        [XmlElement("MultiGeometryObjectExtensionGroup")]
        public AbstractObjectType[] MultiGeometryObjectExtensionGroup
        {
            get { return multiGeometryObjectExtensionGroupField; }
            set { multiGeometryObjectExtensionGroupField = value; }
        }
    }
}