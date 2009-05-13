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
    [XmlRoot("Change", Namespace = "http://www.opengis.net/kml/2.2", IsNullable = false)]
    public class ChangeType
    {
        private ItemsChoiceType1[] itemsElementNameField;
        private AbstractObjectType[] itemsField;

        /// <remarks/>
        [XmlElement("Alias", typeof (AliasType))]
        [XmlElement("Data", typeof (DataType))]
        [XmlElement("Icon", typeof (LinkType))]
        [XmlElement("ImagePyramid", typeof (ImagePyramidType))]
        [XmlElement("ItemIcon", typeof (ItemIconType))]
        [XmlElement("LatLonAltBox", typeof (LatLonAltBoxType))]
        [XmlElement("LatLonBox", typeof (LatLonBoxType))]
        [XmlElement("Link", typeof (LinkType))]
        [XmlElement("Location", typeof (LocationType))]
        [XmlElement("Lod", typeof (LodType))]
        [XmlElement("Orientation", typeof (OrientationType))]
        [XmlElement("Pair", typeof (PairType))]
        [XmlElement("Region", typeof (RegionType))]
        [XmlElement("ResourceMap", typeof (ResourceMapType))]
        [XmlElement("Scale", typeof (ScaleType))]
        [XmlElement("SchemaData", typeof (SchemaDataType))]
        [XmlElement("Url", typeof (LinkType))]
        [XmlElement("ViewVolume", typeof (ViewVolumeType))]
        [XmlChoiceIdentifier("ItemsElementName")]
        public AbstractObjectType[] Items
        {
            get { return itemsField; }
            set { itemsField = value; }
        }

        /// <remarks/>
        [XmlElement("ItemsElementName")]
        [XmlIgnore]
        public ItemsChoiceType1[] ItemsElementName
        {
            get { return itemsElementNameField; }
            set { itemsElementNameField = value; }
        }
    }
}