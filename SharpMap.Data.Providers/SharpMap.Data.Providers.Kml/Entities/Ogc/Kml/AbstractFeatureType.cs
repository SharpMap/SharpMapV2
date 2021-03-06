// /*
//  *  The attached / following is part of SharpMap.Data.Providers.Kml
//  *  SharpMap.Data.Providers.Kml is free software � 2008 Newgrove Consultants Limited, 
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
using System.Collections.Generic;
using System.Xml.Schema;
using System.Xml.Serialization;
using SharpMap.Entities.Atom;
using SharpMap.Entities.xAL;

namespace SharpMap.Entities.Ogc.Kml
{
    [XmlType(TypeName = "AbstractFeatureType", Namespace = Declarations.SchemaVersion), Serializable]
    [XmlInclude(typeof (CameraType))]
    [XmlInclude(typeof (LookAtType))]
    [XmlInclude(typeof (TimeStampType))]
    [XmlInclude(typeof (TimeSpanType))]
    [XmlInclude(typeof (StyleMapType))]
    [XmlInclude(typeof (StyleType))]
    [XmlInclude(typeof (DataType))]
    [XmlInclude(typeof (AbstractTimePrimitiveType))]
    [XmlInclude(typeof (SchemaDataType))]
    [XmlInclude(typeof (ItemIconType))]
    [XmlInclude(typeof (AbstractLatLonBoxType))]
    [XmlInclude(typeof (OrientationType))]
    [XmlInclude(typeof (AbstractStyleSelectorType))]
    [XmlInclude(typeof (ResourceMapType))]
    [XmlInclude(typeof (LocationType))]
    [XmlInclude(typeof (AbstractSubStyleType))]
    [XmlInclude(typeof (RegionType))]
    [XmlInclude(typeof (AliasType))]
    [XmlInclude(typeof (AbstractViewType))]
    [XmlInclude(typeof (AbstractFeatureType))]
    [XmlInclude(typeof (AbstractGeometryType))]
    [XmlInclude(typeof (BasicLinkType))]
    [XmlInclude(typeof (PairType))]
    [XmlInclude(typeof (ImagePyramidType))]
    [XmlInclude(typeof (ScaleType))]
    [XmlInclude(typeof (LodType))]
    [XmlInclude(typeof (ViewVolumeType))]
    public abstract class AbstractFeatureType : AbstractObjectType
    {
        [XmlIgnore] private List<AbstractFeatureObjectExtensionGroup> _abstractFeatureObjectExtensionGroup;
        [XmlIgnore] private List<string> _abstractFeatureSimpleExtensionGroup;
        [XmlIgnore] private List<AbstractStyleSelectorGroup> _abstractStyleSelectorGroup;
        [XmlIgnore] private AbstractTimePrimitiveGroup _abstractTimePrimitiveGroup;
        [XmlIgnore] private AbstractViewGroup _abstractViewGroup;
        [XmlIgnore] private string _address;
        [XmlIgnore] private AddressDetails _addressDetails;
        [XmlIgnore] private Author _author;
        [XmlIgnore] private string _description;
        [XmlIgnore] private ExtendedData _extendedData;
        [XmlIgnore] private Atom.Link _atomLink;
        [XmlIgnore] private Metadata _metadata;
        [XmlIgnore] private string _name;
        [XmlIgnore] private bool _open;

        [XmlIgnore] public bool _openSpecified;
        [XmlIgnore] private string _phoneNumber;
        [XmlIgnore] private Region _region;
        //[XmlIgnore]
        //private string _snippet;
        [XmlIgnore] private Snippet _snippet;
        [XmlIgnore] private string _styleUrl;

        [XmlIgnore] private bool _visibility;

        [XmlIgnore] public bool _visibilitySpecified;

        public AbstractFeatureType()
        {
            Visibility = true;
            Open = false;
        }

        [XmlElement(ElementName = "name", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "string",
            Namespace = Declarations.SchemaVersion)]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }


        [XmlElement(ElementName = "visibility", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "boolean"
            , Namespace = Declarations.SchemaVersion)]
        public bool Visibility
        {
            get { return _visibility; }
            set
            {
                _visibility = value;
                _visibilitySpecified = true;
            }
        }


        [XmlElement(ElementName = "open", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "boolean",
            Namespace = Declarations.SchemaVersion)]
        public bool Open
        {
            get { return _open; }
            set
            {
                _open = value;
                _openSpecified = true;
            }
        }

        [XmlElement(Type = typeof (Author), ElementName = "author", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = "http://www.w3.org/2005/Atom")]
        public Author Author
        {
            get { return _author; }
            set { _author = value; }
        }

        [XmlElement(Type = typeof (Atom.Link), ElementName = "link", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = "http://www.w3.org/2005/Atom")]
        public Atom.Link AtomLink
        {
            get
            {
                if (_atomLink == null) _atomLink = new Atom.Link();
                return _atomLink;
            }
            set { _atomLink = value; }
        }

        [XmlElement(ElementName = "address", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "string",
            Namespace = Declarations.SchemaVersion)]
        public string Address
        {
            get { return _address; }
            set { _address = value; }
        }

        [XmlElement(Type = typeof (AddressDetails), ElementName = "AddressDetails", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "urn:oasis:names:tc:ciq:xsdschema:xAL:2.0")]
        public AddressDetails AddressDetails
        {
            get { return _addressDetails; }
            set { _addressDetails = value; }
        }

        [XmlElement(ElementName = "phoneNumber", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "string"
            , Namespace = Declarations.SchemaVersion)]
        public string PhoneNumber
        {
            get { return _phoneNumber; }
            set { _phoneNumber = value; }
        }

        [XmlElement(Type = typeof (Snippet), ElementName = "Snippet", IsNullable = false, Form = XmlSchemaForm.Qualified
            , Namespace = Declarations.SchemaVersion)]
        public Snippet Snippet
        {
            get { return _snippet; }
            set { _snippet = value; }
        }

        //[XmlElement(ElementName = "snippet", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "string",
        //    Namespace = Declarations.SchemaVersion)]
        //public string snippet
        //{
        //    get { return _snippet; }
        //    set { _snippet = value; }
        //}

        [XmlElement(ElementName = "description", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "string"
            , Namespace = Declarations.SchemaVersion)]
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        [XmlElement(Type = typeof (AbstractViewGroup), ElementName = "AbstractViewGroup", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public AbstractViewGroup AbstractViewGroup
        {
            get { return _abstractViewGroup; }
            set { _abstractViewGroup = value; }
        }

        [XmlElement(Type = typeof (AbstractTimePrimitiveGroup), ElementName = "AbstractTimePrimitiveGroup",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public AbstractTimePrimitiveGroup AbstractTimePrimitiveGroup
        {
            get { return _abstractTimePrimitiveGroup; }
            set { _abstractTimePrimitiveGroup = value; }
        }

        [XmlElement(ElementName = "styleUrl", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "anyURI",
            Namespace = Declarations.SchemaVersion)]
        public string StyleUrl
        {
            get { return _styleUrl; }
            set { _styleUrl = value; }
        }

        [XmlElement(Type = typeof (AbstractStyleSelectorGroup), ElementName = "AbstractStyleSelectorGroup",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<AbstractStyleSelectorGroup> AbstractStyleSelectorGroup
        {
            get
            {
                if (_abstractStyleSelectorGroup == null)
                    _abstractStyleSelectorGroup = new List<AbstractStyleSelectorGroup>();
                return _abstractStyleSelectorGroup;
            }
            set { _abstractStyleSelectorGroup = value; }
        }

        [XmlElement(Type = typeof (Region), ElementName = "Region", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = Declarations.SchemaVersion)]
        public Region Region
        {
            get { return _region; }
            set { _region = value; }
        }

        [XmlElement(Type = typeof (Metadata), ElementName = "Metadata", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public Metadata Metadata
        {
            get { return _metadata; }
            set { _metadata = value; }
        }

        [XmlElement(Type = typeof (ExtendedData), ElementName = "ExtendedData", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public ExtendedData ExtendedData
        {
            get { return _extendedData; }
            set { _extendedData = value; }
        }

        [XmlElement(Type = typeof (string), ElementName = "AbstractFeatureSimpleExtensionGroup", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<string> AbstractFeatureSimpleExtensionGroup
        {
            get
            {
                if (_abstractFeatureSimpleExtensionGroup == null)
                    _abstractFeatureSimpleExtensionGroup = new List<string>();
                return _abstractFeatureSimpleExtensionGroup;
            }
            set { _abstractFeatureSimpleExtensionGroup = value; }
        }

        [XmlElement(Type = typeof (AbstractFeatureObjectExtensionGroup),
            ElementName = "AbstractFeatureObjectExtensionGroup", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = Declarations.SchemaVersion)]
        public List<AbstractFeatureObjectExtensionGroup> AbstractFeatureObjectExtensionGroup
        {
            get
            {
                if (_abstractFeatureObjectExtensionGroup == null)
                    _abstractFeatureObjectExtensionGroup = new List<AbstractFeatureObjectExtensionGroup>();
                return _abstractFeatureObjectExtensionGroup;
            }
            set { _abstractFeatureObjectExtensionGroup = value; }
        }
    }
}