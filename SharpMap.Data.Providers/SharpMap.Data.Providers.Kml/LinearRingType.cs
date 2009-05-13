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
    [XmlRoot("LinearRing", Namespace = "http://www.opengis.net/kml/2.2", IsNullable = false)]
    public class LinearRingType : AbstractGeometryType
    {
        private string coordinatesField;
        private bool extrudeField;

        private bool extrudeFieldSpecified;

        private altitudeModeEnumType itemField;

        private AbstractObjectType[] linearRingObjectExtensionGroupField;
        private string[] linearRingSimpleExtensionGroupField;
        private bool tessellateField;

        private bool tessellateFieldSpecified;

        public LinearRingType()
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
        public string coordinates
        {
            get { return coordinatesField; }
            set { coordinatesField = value; }
        }

        /// <remarks/>
        [XmlElement("LinearRingSimpleExtensionGroup")]
        public string[] LinearRingSimpleExtensionGroup
        {
            get { return linearRingSimpleExtensionGroupField; }
            set { linearRingSimpleExtensionGroupField = value; }
        }

        /// <remarks/>
        [XmlElement("LinearRingObjectExtensionGroup")]
        public AbstractObjectType[] LinearRingObjectExtensionGroup
        {
            get { return linearRingObjectExtensionGroupField; }
            set { linearRingObjectExtensionGroupField = value; }
        }
    }
}