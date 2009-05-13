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
    [XmlRoot("Placemark", Namespace = "http://www.opengis.net/kml/2.2", IsNullable = false)]
    public class PlacemarkType : AbstractFeatureType
    {
        private AbstractGeometryType itemField;

        private AbstractObjectType[] placemarkObjectExtensionGroupField;
        private string[] placemarkSimpleExtensionGroupField;

        /// <remarks/>
        [XmlElement("LineString", typeof (LineStringType))]
        [XmlElement("LinearRing", typeof (LinearRingType))]
        [XmlElement("Model", typeof (ModelType))]
        [XmlElement("MultiGeometry", typeof (MultiGeometryType))]
        [XmlElement("Point", typeof (PointType))]
        [XmlElement("Polygon", typeof (PolygonType))]
        public AbstractGeometryType Item
        {
            get { return itemField; }
            set { itemField = value; }
        }

        /// <remarks/>
        [XmlElement("PlacemarkSimpleExtensionGroup")]
        public string[] PlacemarkSimpleExtensionGroup
        {
            get { return placemarkSimpleExtensionGroupField; }
            set { placemarkSimpleExtensionGroupField = value; }
        }

        /// <remarks/>
        [XmlElement("PlacemarkObjectExtensionGroup")]
        public AbstractObjectType[] PlacemarkObjectExtensionGroup
        {
            get { return placemarkObjectExtensionGroupField; }
            set { placemarkObjectExtensionGroupField = value; }
        }
    }
}