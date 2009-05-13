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
    [XmlRoot("Polygon", Namespace = "http://www.opengis.net/kml/2.2", IsNullable = false)]
    public class PolygonType : AbstractGeometryType
    {
        private bool extrudeField;

        private bool extrudeFieldSpecified;
        private BoundaryType[] innerBoundaryIsField;

        private altitudeModeEnumType itemField;

        private BoundaryType outerBoundaryIsField;

        private AbstractObjectType[] polygonObjectExtensionGroupField;
        private string[] polygonSimpleExtensionGroupField;
        private bool tessellateField;

        private bool tessellateFieldSpecified;

        public PolygonType()
        {
            extrudeField = false;
            tessellateField = false;
            itemField = altitudeModeEnumType.clampToGround;
        }

        /// <remarks/>
        public bool extrude
        {
            get { return extrudeField; }
            set { extrudeField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool extrudeSpecified
        {
            get { return extrudeFieldSpecified; }
            set { extrudeFieldSpecified = value; }
        }

        /// <remarks/>
        public bool tessellate
        {
            get { return tessellateField; }
            set { tessellateField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool tessellateSpecified
        {
            get { return tessellateFieldSpecified; }
            set { tessellateFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement("altitudeMode")]
        public altitudeModeEnumType Item
        {
            get { return itemField; }
            set { itemField = value; }
        }

        /// <remarks/>
        public BoundaryType outerBoundaryIs
        {
            get { return outerBoundaryIsField; }
            set { outerBoundaryIsField = value; }
        }

        /// <remarks/>
        [XmlElement("innerBoundaryIs")]
        public BoundaryType[] innerBoundaryIs
        {
            get { return innerBoundaryIsField; }
            set { innerBoundaryIsField = value; }
        }

        /// <remarks/>
        [XmlElement("PolygonSimpleExtensionGroup")]
        public string[] PolygonSimpleExtensionGroup
        {
            get { return polygonSimpleExtensionGroupField; }
            set { polygonSimpleExtensionGroupField = value; }
        }

        /// <remarks/>
        [XmlElement("PolygonObjectExtensionGroup")]
        public AbstractObjectType[] PolygonObjectExtensionGroup
        {
            get { return polygonObjectExtensionGroupField; }
            set { polygonObjectExtensionGroupField = value; }
        }
    }
}