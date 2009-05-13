// /*
//  *  The attached / following is part of SharpMap.Data.Providers.Kml
//  *  SharpMap.Data.Providers.Kml is free software © 2008 Newgrove Consultants Limited, 
//  *  www.newgrove.com; you can redistribute it and/or modify it under the terms 
//  *  of the current GNU Lesser General Public License (LGPL) as published by and 
//  *  available from the Free Software Foundation, Inc., 
//  *  59 Temple Place, Suite 330, Boston, MA 02111-1307 USA: http://fsf.org/    
//  *  This program is distributed without any warranty; 
//  *  without even the implied warranty of merchantability or fitness for purpose.  
//  *  See the GNU Lesser General Public License for the full details. 
//  *  
//  *  Author: John Diss 2009
//  * 
//  */
using System;
using System.Xml.Serialization;
using SharpMap.Entities.Atom;
using SharpMap.Entities.xAl;

namespace SharpMap.Entities.Ogc.Kml
{
    /// <remarks/>
    [XmlInclude(typeof (OverlayBase))]
    [XmlInclude(typeof (PhotoOverlay))]
    [XmlInclude(typeof (ScreenOverlay))]
    [XmlInclude(typeof (GroundOverlay))]
    [XmlInclude(typeof (NetworkLink))]
    [XmlInclude(typeof (Placemark))]
    [XmlInclude(typeof (ContainerBase))]
    [XmlInclude(typeof (Folder))]
    [XmlInclude(typeof (Document))]
    [Serializable]
    [XmlType(TypeName = "AbstractFeatureType", Namespace = "http://www.opengis.net/kml/2.2")]
    public abstract class FeatureBase : KmlObjectBase
    {
        private KmlObjectBase[] _kmlFeatureObjectExtensionGroupField;
        private string[] abstractFeatureSimpleExtensionGroupField;
        private AddressDetails addressDetailsField;
        private string addressField;
        private Person authorField;

        private string descriptionField;

        private ViewBase item1Field;

        private TimePrimitiveBase item2Field;
        private object item3Field;
        private object itemField;

        private StyleSelectorBase[] itemsField;
        private Link linkField;
        private string nameField;
        private bool openField;

        private bool openFieldSpecified;
        private string phoneNumberField;

        private Region regionField;
        private string styleUrlField;
        private bool visibilityField;

        private bool visibilityFieldSpecified;

        public FeatureBase()
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
        public Person author
        {
            get { return authorField; }
            set { authorField = value; }
        }

        /// <remarks/>
        [XmlElement(Namespace = "http://www.w3.org/2005/Atom")]
        public Link link
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
        [XmlElement("Snippet", typeof (Snippet))]
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
        [XmlElement("Camera", typeof (Camera))]
        [XmlElement("LookAt", typeof (LookAt))]
        public ViewBase Item1
        {
            get { return item1Field; }
            set { item1Field = value; }
        }

        /// <remarks/>
        [XmlElement("TimeSpan", typeof (TimeSpan))]
        [XmlElement("TimeStamp", typeof (TimeStamp))]
        public TimePrimitiveBase Item2
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
        [XmlElement("Style", typeof (Style))]
        [XmlElement("StyleMap", typeof (StyleMap))]
        public StyleSelectorBase[] Items
        {
            get { return itemsField; }
            set { itemsField = value; }
        }

        /// <remarks/>
        public Region Region
        {
            get { return regionField; }
            set { regionField = value; }
        }

        /// <remarks/>
        [XmlElement("ExtendedData", typeof (ExtendedData))]
        [XmlElement("Metadata", typeof (Metadata))]
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
        public KmlObjectBase[] KmlFeatureObjectExtensionGroup
        {
            get { return _kmlFeatureObjectExtensionGroupField; }
            set { _kmlFeatureObjectExtensionGroupField = value; }
        }
    }
}