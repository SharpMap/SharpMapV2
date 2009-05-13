using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Ogc.Kml
{
    /// <remarks/>
    [XmlInclude(typeof (AbstractOverlayType))]
    [XmlInclude(typeof (PhotoOverlayType))]
    [XmlInclude(typeof (ScreenOverlayType))]
    [XmlInclude(typeof (GroundOverlayType))]
    [XmlInclude(typeof (NetworkLinkType))]
    [XmlInclude(typeof (PlacemarkType))]
    [XmlInclude(typeof (AbstractContainerType))]
    [XmlInclude(typeof (FolderType))]
    [XmlInclude(typeof (DocumentType))]
    [GeneratedCode("xsd", "2.0.50727.3038")]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://www.opengis.net/kml/2.2")]
    public abstract class AbstractFeatureType : AbstractObjectType
    {
        private AbstractObjectType[] abstractFeatureObjectExtensionGroupField;
        private string[] abstractFeatureSimpleExtensionGroupField;
        private AddressDetails addressDetailsField;
        private string addressField;
        private atomPersonConstruct authorField;

        private string descriptionField;

        private AbstractViewType item1Field;

        private AbstractTimePrimitiveType item2Field;
        private object item3Field;
        private object itemField;

        private AbstractStyleSelectorType[] itemsField;
        private link linkField;
        private string nameField;
        private bool openField;

        private bool openFieldSpecified;
        private string phoneNumberField;

        private RegionType regionField;
        private string styleUrlField;
        private bool visibilityField;

        private bool visibilityFieldSpecified;

        public AbstractFeatureType()
        {
            visibilityField = true;
            openField = false;
        }

        /// <remarks/>
        public string name
        {
            get { return nameField; }
            set { nameField = value; }
        }

        /// <remarks/>
        public bool visibility
        {
            get { return visibilityField; }
            set { visibilityField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool visibilitySpecified
        {
            get { return visibilityFieldSpecified; }
            set { visibilityFieldSpecified = value; }
        }

        /// <remarks/>
        public bool open
        {
            get { return openField; }
            set { openField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool openSpecified
        {
            get { return openFieldSpecified; }
            set { openFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Namespace = "http://www.w3.org/2005/Atom")]
        public atomPersonConstruct author
        {
            get { return authorField; }
            set { authorField = value; }
        }

        /// <remarks/>
        [XmlElement(Namespace = "http://www.w3.org/2005/Atom")]
        public link link
        {
            get { return linkField; }
            set { linkField = value; }
        }

        /// <remarks/>
        public string address
        {
            get { return addressField; }
            set { addressField = value; }
        }

        /// <remarks/>
        [XmlElement(Namespace = "urn:oasis:names:tc:ciq:xsdschema:xAL:2.0")]
        public AddressDetails AddressDetails
        {
            get { return addressDetailsField; }
            set { addressDetailsField = value; }
        }

        /// <remarks/>
        public string phoneNumber
        {
            get { return phoneNumberField; }
            set { phoneNumberField = value; }
        }

        /// <remarks/>
        [XmlElement("Snippet", typeof (SnippetType))]
        [XmlElement("snippet", typeof (string))]
        public object Item
        {
            get { return itemField; }
            set { itemField = value; }
        }

        /// <remarks/>
        public string description
        {
            get { return descriptionField; }
            set { descriptionField = value; }
        }

        /// <remarks/>
        [XmlElement("Camera", typeof (CameraType))]
        [XmlElement("LookAt", typeof (LookAtType))]
        public AbstractViewType Item1
        {
            get { return item1Field; }
            set { item1Field = value; }
        }

        /// <remarks/>
        [XmlElement("TimeSpan", typeof (TimeSpanType))]
        [XmlElement("TimeStamp", typeof (TimeStampType))]
        public AbstractTimePrimitiveType Item2
        {
            get { return item2Field; }
            set { item2Field = value; }
        }

        /// <remarks/>
        [XmlElement(DataType = "anyURI")]
        public string styleUrl
        {
            get { return styleUrlField; }
            set { styleUrlField = value; }
        }

        /// <remarks/>
        [XmlElement("Style", typeof (StyleType))]
        [XmlElement("StyleMap", typeof (StyleMapType))]
        public AbstractStyleSelectorType[] Items
        {
            get { return itemsField; }
            set { itemsField = value; }
        }

        /// <remarks/>
        public RegionType Region
        {
            get { return regionField; }
            set { regionField = value; }
        }

        /// <remarks/>
        [XmlElement("ExtendedData", typeof (ExtendedDataType))]
        [XmlElement("Metadata", typeof (MetadataType))]
        public object Item3
        {
            get { return item3Field; }
            set { item3Field = value; }
        }

        /// <remarks/>
        [XmlElement("AbstractFeatureSimpleExtensionGroup")]
        public string[] AbstractFeatureSimpleExtensionGroup
        {
            get { return abstractFeatureSimpleExtensionGroupField; }
            set { abstractFeatureSimpleExtensionGroupField = value; }
        }

        /// <remarks/>
        [XmlElement("AbstractFeatureObjectExtensionGroup")]
        public AbstractObjectType[] AbstractFeatureObjectExtensionGroup
        {
            get { return abstractFeatureObjectExtensionGroupField; }
            set { abstractFeatureObjectExtensionGroupField = value; }
        }
    }
}