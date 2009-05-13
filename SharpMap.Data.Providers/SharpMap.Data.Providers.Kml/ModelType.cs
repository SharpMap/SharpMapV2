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
    [XmlRoot("Model", Namespace = "http://www.opengis.net/kml/2.2", IsNullable = false)]
    public class ModelType : AbstractGeometryType
    {
        private altitudeModeEnumType itemField;
        private LinkType linkField;

        private LocationType locationField;
        private AbstractObjectType[] modelObjectExtensionGroupField;
        private string[] modelSimpleExtensionGroupField;

        private OrientationType orientationField;

        private ResourceMapType resourceMapField;
        private ScaleType scaleField;

        public ModelType()
        {
            itemField = altitudeModeEnumType.clampToGround;
        }

        /// <remarks/>
        [XmlElement("altitudeMode")]
        public altitudeModeEnumType Item
        {
            get { return itemField; }
            set { itemField = value; }
        }

        /// <remarks/>
        public LocationType Location
        {
            get { return locationField; }
            set { locationField = value; }
        }

        /// <remarks/>
        public OrientationType Orientation
        {
            get { return orientationField; }
            set { orientationField = value; }
        }

        /// <remarks/>
        public ScaleType Scale
        {
            get { return scaleField; }
            set { scaleField = value; }
        }

        /// <remarks/>
        public LinkType Link
        {
            get { return linkField; }
            set { linkField = value; }
        }

        /// <remarks/>
        public ResourceMapType ResourceMap
        {
            get { return resourceMapField; }
            set { resourceMapField = value; }
        }

        /// <remarks/>
        [XmlElement("ModelSimpleExtensionGroup")]
        public string[] ModelSimpleExtensionGroup
        {
            get { return modelSimpleExtensionGroupField; }
            set { modelSimpleExtensionGroupField = value; }
        }

        /// <remarks/>
        [XmlElement("ModelObjectExtensionGroup")]
        public AbstractObjectType[] ModelObjectExtensionGroup
        {
            get { return modelObjectExtensionGroupField; }
            set { modelObjectExtensionGroupField = value; }
        }
    }
}