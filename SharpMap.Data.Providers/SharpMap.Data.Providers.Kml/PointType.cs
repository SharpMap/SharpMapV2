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
    [XmlRoot("Point", Namespace = "http://www.opengis.net/kml/2.2", IsNullable = false)]
    public class PointType : AbstractGeometryType
    {
        private string coordinatesField;
        private bool extrudeField;

        private bool extrudeFieldSpecified;

        private altitudeModeEnumType itemField;

        private AbstractObjectType[] pointObjectExtensionGroupField;
        private string[] pointSimpleExtensionGroupField;

        public PointType()
        {
            extrudeField = false;
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
        [XmlElement("altitudeMode")]
        public altitudeModeEnumType Item
        {
            get { return itemField; }
            set { itemField = value; }
        }

        /// <remarks/>
        public string coordinates
        {
            get { return coordinatesField; }
            set { coordinatesField = value; }
        }

        /// <remarks/>
        [XmlElement("PointSimpleExtensionGroup")]
        public string[] PointSimpleExtensionGroup
        {
            get { return pointSimpleExtensionGroupField; }
            set { pointSimpleExtensionGroupField = value; }
        }

        /// <remarks/>
        [XmlElement("PointObjectExtensionGroup")]
        public AbstractObjectType[] PointObjectExtensionGroup
        {
            get { return pointObjectExtensionGroupField; }
            set { pointObjectExtensionGroupField = value; }
        }
    }
}