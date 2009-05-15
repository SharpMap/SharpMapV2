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
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SharpMap.Entities.xAL
{
    [XmlType(TypeName = "LargeMailUserType", Namespace = Declarations.SchemaVersion), Serializable]
    public class LargeMailUserType
    {
        [XmlIgnore] private List<AddressLine> _addressLine;
        [XmlIgnore] private List<BuildingNameType> _buildingName;
        [XmlIgnore] private Department _department;
        [XmlIgnore] private LargeMailUserIdentifier _largeMailUserIdentifier;
        [XmlIgnore] private List<LargeMailUserName> _largeMailUserName;
        [XmlIgnore] private PostalCode _postalCode;
        [XmlIgnore] private PostBox _postBox;
        [XmlIgnore] private Thoroughfare _thoroughfare;
        [XmlIgnore] private string _type;
        [XmlAnyElement] public XmlElement[] Any;

        [XmlAnyAttribute] public XmlAttribute[] AnyAttr;

        [XmlAttribute(AttributeName = "Type", DataType = "string")]
        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }

        [XmlElement(Type = typeof (AddressLine), ElementName = "AddressLine", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<AddressLine> AddressLine
        {
            get
            {
                if (_addressLine == null) _addressLine = new List<AddressLine>();
                return _addressLine;
            }
            set { _addressLine = value; }
        }

        [XmlElement(Type = typeof (LargeMailUserName), ElementName = "LargeMailUserName", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<LargeMailUserName> LargeMailUserName
        {
            get
            {
                if (_largeMailUserName == null) _largeMailUserName = new List<LargeMailUserName>();
                return _largeMailUserName;
            }
            set { _largeMailUserName = value; }
        }

        [XmlElement(Type = typeof (LargeMailUserIdentifier), ElementName = "LargeMailUserIdentifier", IsNullable = false
            , Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public LargeMailUserIdentifier LargeMailUserIdentifier
        {
            get { return _largeMailUserIdentifier; }
            set { _largeMailUserIdentifier = value; }
        }

        [XmlElement(Type = typeof (BuildingNameType), ElementName = "BuildingName", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<BuildingNameType> BuildingName
        {
            get
            {
                if (_buildingName == null) _buildingName = new List<BuildingNameType>();
                return _buildingName;
            }
            set { _buildingName = value; }
        }

        [XmlElement(Type = typeof (Department), ElementName = "Department", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public Department Department
        {
            get { return _department; }
            set { _department = value; }
        }

        [XmlElement(Type = typeof (PostBox), ElementName = "PostBox", IsNullable = false, Form = XmlSchemaForm.Qualified
            , Namespace = Declarations.SchemaVersion)]
        public PostBox PostBox
        {
            get { return _postBox; }
            set { _postBox = value; }
        }

        [XmlElement(Type = typeof (Thoroughfare), ElementName = "Thoroughfare", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public Thoroughfare Thoroughfare
        {
            get { return _thoroughfare; }
            set { _thoroughfare = value; }
        }

        [XmlElement(Type = typeof (PostalCode), ElementName = "PostalCode", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public PostalCode PostalCode
        {
            get { return _postalCode; }
            set { _postalCode = value; }
        }

        public void MakeSchemaCompliant()
        {
        }
    }
}