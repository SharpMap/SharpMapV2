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
        [XmlIgnore] private List<AddressLine> _AddressLine;
        [XmlIgnore] private List<BuildingNameType> _BuildingName;
        [XmlIgnore] private Department _Department;
        [XmlIgnore] private LargeMailUserIdentifier _LargeMailUserIdentifier;
        [XmlIgnore] private List<LargeMailUserName> _LargeMailUserName;
        [XmlIgnore] private PostalCode _PostalCode;
        [XmlIgnore] private PostBox _PostBox;
        [XmlIgnore] private Thoroughfare _Thoroughfare;
        [XmlIgnore] private string _Type;
        [XmlAnyElement] public XmlElement[] Any;

        [XmlAnyAttribute] public XmlAttribute[] AnyAttr;

        [XmlAttribute(AttributeName = "Type", DataType = "string")]
        public string Type
        {
            get { return _Type; }
            set { _Type = value; }
        }

        [XmlElement(Type = typeof (AddressLine), ElementName = "AddressLine", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<AddressLine> AddressLine
        {
            get
            {
                if (_AddressLine == null) _AddressLine = new List<AddressLine>();
                return _AddressLine;
            }
            set { _AddressLine = value; }
        }

        [XmlElement(Type = typeof (LargeMailUserName), ElementName = "LargeMailUserName", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<LargeMailUserName> LargeMailUserName
        {
            get
            {
                if (_LargeMailUserName == null) _LargeMailUserName = new List<LargeMailUserName>();
                return _LargeMailUserName;
            }
            set { _LargeMailUserName = value; }
        }

        [XmlElement(Type = typeof (LargeMailUserIdentifier), ElementName = "LargeMailUserIdentifier", IsNullable = false
            , Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public LargeMailUserIdentifier LargeMailUserIdentifier
        {
            get { return _LargeMailUserIdentifier; }
            set { _LargeMailUserIdentifier = value; }
        }

        [XmlElement(Type = typeof (BuildingNameType), ElementName = "BuildingName", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<BuildingNameType> BuildingName
        {
            get
            {
                if (_BuildingName == null) _BuildingName = new List<BuildingNameType>();
                return _BuildingName;
            }
            set { _BuildingName = value; }
        }

        [XmlElement(Type = typeof (Department), ElementName = "Department", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public Department Department
        {
            get { return _Department; }
            set { _Department = value; }
        }

        [XmlElement(Type = typeof (PostBox), ElementName = "PostBox", IsNullable = false, Form = XmlSchemaForm.Qualified
            , Namespace = Declarations.SchemaVersion)]
        public PostBox PostBox
        {
            get { return _PostBox; }
            set { _PostBox = value; }
        }

        [XmlElement(Type = typeof (Thoroughfare), ElementName = "Thoroughfare", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public Thoroughfare Thoroughfare
        {
            get { return _Thoroughfare; }
            set { _Thoroughfare = value; }
        }

        [XmlElement(Type = typeof (PostalCode), ElementName = "PostalCode", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public PostalCode PostalCode
        {
            get { return _PostalCode; }
            set { _PostalCode = value; }
        }

        public void MakeSchemaCompliant()
        {
        }
    }
}