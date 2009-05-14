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
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SharpMap.Entities.xAL
{
    [XmlType(TypeName = "LargeMailUserType", Namespace = Declarations.SchemaVersion), Serializable]
    public class LargeMailUserType
    {
        [XmlIgnore] private List<AddressLine> __AddressLine;
        [XmlIgnore] private List<BuildingNameType> __BuildingName;
        [XmlIgnore] private Department __Department;
        [XmlIgnore] private LargeMailUserIdentifier __LargeMailUserIdentifier;
        [XmlIgnore] private List<LargeMailUserName> __LargeMailUserName;
        [XmlIgnore] private PostalCode __PostalCode;
        [XmlIgnore] private PostBox __PostBox;
        [XmlIgnore] private Thoroughfare __Thoroughfare;
        [XmlIgnore] private string __Type;
        [XmlAnyElement] public XmlElement[] Any;

        [XmlAnyAttribute] public XmlAttribute[] AnyAttr;

        [XmlAttribute(AttributeName = "Type", DataType = "string")]
        public string Type
        {
            get { return __Type; }
            set { __Type = value; }
        }

        [XmlElement(Type = typeof (AddressLine), ElementName = "AddressLine", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<AddressLine> AddressLine
        {
            get
            {
                if (__AddressLine == null) __AddressLine = new List<AddressLine>();
                return __AddressLine;
            }
            set { __AddressLine = value; }
        }

        [XmlElement(Type = typeof (LargeMailUserName), ElementName = "LargeMailUserName", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<LargeMailUserName> LargeMailUserName
        {
            get
            {
                if (__LargeMailUserName == null) __LargeMailUserName = new List<LargeMailUserName>();
                return __LargeMailUserName;
            }
            set { __LargeMailUserName = value; }
        }

        [XmlElement(Type = typeof (LargeMailUserIdentifier), ElementName = "LargeMailUserIdentifier", IsNullable = false
            , Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public LargeMailUserIdentifier LargeMailUserIdentifier
        {
            get
            {
                if (__LargeMailUserIdentifier == null) __LargeMailUserIdentifier = new LargeMailUserIdentifier();
                return __LargeMailUserIdentifier;
            }
            set { __LargeMailUserIdentifier = value; }
        }

        [XmlElement(Type = typeof (BuildingNameType), ElementName = "BuildingName", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<BuildingNameType> BuildingName
        {
            get
            {
                if (__BuildingName == null) __BuildingName = new List<BuildingNameType>();
                return __BuildingName;
            }
            set { __BuildingName = value; }
        }

        [XmlElement(Type = typeof (Department), ElementName = "Department", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public Department Department
        {
            get
            {
                if (__Department == null) __Department = new Department();
                return __Department;
            }
            set { __Department = value; }
        }

        [XmlElement(Type = typeof (PostBox), ElementName = "PostBox", IsNullable = false, Form = XmlSchemaForm.Qualified
            , Namespace = Declarations.SchemaVersion)]
        public PostBox PostBox
        {
            get
            {
                if (__PostBox == null) __PostBox = new PostBox();
                return __PostBox;
            }
            set { __PostBox = value; }
        }

        [XmlElement(Type = typeof (Thoroughfare), ElementName = "Thoroughfare", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public Thoroughfare Thoroughfare
        {
            get
            {
                if (__Thoroughfare == null) __Thoroughfare = new Thoroughfare();
                return __Thoroughfare;
            }
            set { __Thoroughfare = value; }
        }

        [XmlElement(Type = typeof (PostalCode), ElementName = "PostalCode", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public PostalCode PostalCode
        {
            get
            {
                if (__PostalCode == null) __PostalCode = new PostalCode();
                return __PostalCode;
            }
            set { __PostalCode = value; }
        }

        public void MakeSchemaCompliant()
        {
        }
    }
}