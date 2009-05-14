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
using System.Collections.Generic;
using System.Xml.Schema;
using System.Xml.Serialization;
using SharpMap.Entities.Atom;
using SharpMap.Entities.xAL;

namespace SharpMap.Entities.Ogc.Kml
{
    [XmlType(TypeName = "AbstractFeatureType", Namespace = Declarations.SchemaVersion), Serializable]
    [XmlInclude(typeof(CameraType))]
    [XmlInclude(typeof(LookAtType))]
    [XmlInclude(typeof(TimeStampType))]
    [XmlInclude(typeof(TimeSpanType))]
    [XmlInclude(typeof(StyleMapType))]
    [XmlInclude(typeof(StyleType))]
    [XmlInclude(typeof(DataType))]
    [XmlInclude(typeof(AbstractTimePrimitiveType))]
    [XmlInclude(typeof(SchemaDataType))]
    [XmlInclude(typeof(ItemIconType))]
    [XmlInclude(typeof(AbstractLatLonBoxType))]
    [XmlInclude(typeof(OrientationType))]
    [XmlInclude(typeof(AbstractStyleSelectorType))]
    [XmlInclude(typeof(ResourceMapType))]
    [XmlInclude(typeof(LocationType))]
    [XmlInclude(typeof(AbstractSubStyleType))]
    [XmlInclude(typeof(RegionType))]
    [XmlInclude(typeof(AliasType))]
    [XmlInclude(typeof(AbstractViewType))]
    [XmlInclude(typeof(AbstractFeatureType))]
    [XmlInclude(typeof(AbstractGeometryType))]
    [XmlInclude(typeof(BasicLinkType))]
    [XmlInclude(typeof(PairType))]
    [XmlInclude(typeof(ImagePyramidType))]
    [XmlInclude(typeof(ScaleType))]
    [XmlInclude(typeof(LodType))]
    [XmlInclude(typeof(ViewVolumeType))]
    public abstract class AbstractFeatureType : AbstractObjectType
    {
        [XmlIgnore]
        private List<AbstractFeatureObjectExtensionGroup> __AbstractFeatureObjectExtensionGroup;
        [XmlIgnore]
        private List<string> __AbstractFeatureSimpleExtensionGroup;
        [XmlIgnore]
        private List<AbstractStyleSelectorGroup> __AbstractStyleSelectorGroup;
        [XmlIgnore]
        private AbstractTimePrimitiveGroup __AbstractTimePrimitiveGroup;
        [XmlIgnore]
        private AbstractViewGroup __AbstractViewGroup;
        [XmlIgnore]
        private string __address;
        [XmlIgnore]
        private AddressDetails __AddressDetails;
        [XmlIgnore]
        private Author __author;
        [XmlIgnore]
        private string __description;
        [XmlIgnore]
        private ExtendedData __ExtendedData;
        [XmlIgnore]
        private Atom.Link __link;
        [XmlIgnore]
        private Metadata __Metadata;
        [XmlIgnore]
        private string __name;
        [XmlIgnore]
        private bool __open;

        [XmlIgnore]
        public bool __openSpecified;
        [XmlIgnore]
        private string __phoneNumber;
        [XmlIgnore]
        private Region __Region;
        //[XmlIgnore]
        //private string __snippet;
        [XmlIgnore]
        private Snippet __Snippet;
        [XmlIgnore]
        private string __styleUrl;

        [XmlIgnore]
        private bool __visibility;

        [XmlIgnore]
        public bool __visibilitySpecified;

        public AbstractFeatureType()
        {
            Visibility = true;
            Open = false;
        }

        [XmlElement(ElementName = "name", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "string",
            Namespace = Declarations.SchemaVersion)]
        public string Name
        {
            get { return __name; }
            set { __name = value; }
        }


        [XmlElement(ElementName = "visibility", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "boolean"
            , Namespace = Declarations.SchemaVersion)]
        public bool Visibility
        {
            get { return __visibility; }
            set
            {
                __visibility = value;
                __visibilitySpecified = true;
            }
        }


        [XmlElement(ElementName = "open", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "boolean",
            Namespace = Declarations.SchemaVersion)]
        public bool Open
        {
            get { return __open; }
            set
            {
                __open = value;
                __openSpecified = true;
            }
        }

        [XmlElement(Type = typeof(Author), ElementName = "author", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = "http://www.w3.org/2005/Atom")]
        public Author Author
        {
            get
            {
                if (__author == null) __author = new Author();
                return __author;
            }
            set { __author = value; }
        }

        [XmlElement(Type = typeof(Atom.Link), ElementName = "link", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = "http://www.w3.org/2005/Atom")]
        public Atom.Link Link
        {
            get
            {
                if (__link == null) __link = new Atom.Link();
                return __link;
            }
            set { __link = value; }
        }

        [XmlElement(ElementName = "address", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "string",
            Namespace = Declarations.SchemaVersion)]
        public string Address
        {
            get { return __address; }
            set { __address = value; }
        }

        [XmlElement(Type = typeof(AddressDetails), ElementName = "AddressDetails", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "urn:oasis:names:tc:ciq:xsdschema:xAL:2.0")]
        public AddressDetails AddressDetails
        {
            get
            {
                if (__AddressDetails == null) __AddressDetails = new AddressDetails();
                return __AddressDetails;
            }
            set { __AddressDetails = value; }
        }

        [XmlElement(ElementName = "phoneNumber", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "string"
            , Namespace = Declarations.SchemaVersion)]
        public string PhoneNumber
        {
            get { return __phoneNumber; }
            set { __phoneNumber = value; }
        }

        [XmlElement(Type = typeof(Snippet), ElementName = "Snippet", IsNullable = false, Form = XmlSchemaForm.Qualified
            , Namespace = Declarations.SchemaVersion)]
        public Snippet Snippet
        {
            get
            {
                if (__Snippet == null) __Snippet = new Snippet();
                return __Snippet;
            }
            set { __Snippet = value; }
        }

        //[XmlElement(ElementName = "snippet", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "string",
        //    Namespace = Declarations.SchemaVersion)]
        //public string snippet
        //{
        //    get { return __snippet; }
        //    set { __snippet = value; }
        //}

        [XmlElement(ElementName = "description", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "string"
            , Namespace = Declarations.SchemaVersion)]
        public string Description
        {
            get { return __description; }
            set { __description = value; }
        }

        [XmlElement(Type = typeof(AbstractViewGroup), ElementName = "AbstractViewGroup", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public AbstractViewGroup AbstractViewGroup
        {
            get { return __AbstractViewGroup; }
            set { __AbstractViewGroup = value; }
        }

        [XmlElement(Type = typeof(AbstractTimePrimitiveGroup), ElementName = "AbstractTimePrimitiveGroup",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public AbstractTimePrimitiveGroup AbstractTimePrimitiveGroup
        {
            get { return __AbstractTimePrimitiveGroup; }
            set { __AbstractTimePrimitiveGroup = value; }
        }

        [XmlElement(ElementName = "styleUrl", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "anyURI",
            Namespace = Declarations.SchemaVersion)]
        public string StyleUrl
        {
            get { return __styleUrl; }
            set { __styleUrl = value; }
        }

        [XmlElement(Type = typeof(AbstractStyleSelectorGroup), ElementName = "AbstractStyleSelectorGroup",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<AbstractStyleSelectorGroup> AbstractStyleSelectorGroup
        {
            get
            {
                if (__AbstractStyleSelectorGroup == null)
                    __AbstractStyleSelectorGroup = new List<AbstractStyleSelectorGroup>();
                return __AbstractStyleSelectorGroup;
            }
            set { __AbstractStyleSelectorGroup = value; }
        }

        [XmlElement(Type = typeof(Region), ElementName = "Region", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = Declarations.SchemaVersion)]
        public Region Region
        {
            get
            {
                if (__Region == null) __Region = new Region();
                return __Region;
            }
            set { __Region = value; }
        }

        [XmlElement(Type = typeof(Metadata), ElementName = "Metadata", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public Metadata Metadata
        {
            get
            {
                if (__Metadata == null) __Metadata = new Metadata();
                return __Metadata;
            }
            set { __Metadata = value; }
        }

        [XmlElement(Type = typeof(ExtendedData), ElementName = "ExtendedData", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public ExtendedData ExtendedData
        {
            get
            {
                if (__ExtendedData == null) __ExtendedData = new ExtendedData();
                return __ExtendedData;
            }
            set { __ExtendedData = value; }
        }

        [XmlElement(Type = typeof(string), ElementName = "AbstractFeatureSimpleExtensionGroup", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<string> AbstractFeatureSimpleExtensionGroup
        {
            get
            {
                if (__AbstractFeatureSimpleExtensionGroup == null)
                    __AbstractFeatureSimpleExtensionGroup = new List<string>();
                return __AbstractFeatureSimpleExtensionGroup;
            }
            set { __AbstractFeatureSimpleExtensionGroup = value; }
        }

        [XmlElement(Type = typeof(AbstractFeatureObjectExtensionGroup),
            ElementName = "AbstractFeatureObjectExtensionGroup", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = Declarations.SchemaVersion)]
        public List<AbstractFeatureObjectExtensionGroup> AbstractFeatureObjectExtensionGroup
        {
            get
            {
                if (__AbstractFeatureObjectExtensionGroup == null)
                    __AbstractFeatureObjectExtensionGroup = new List<AbstractFeatureObjectExtensionGroup>();
                return __AbstractFeatureObjectExtensionGroup;
            }
            set { __AbstractFeatureObjectExtensionGroup = value; }
        }

    }
}