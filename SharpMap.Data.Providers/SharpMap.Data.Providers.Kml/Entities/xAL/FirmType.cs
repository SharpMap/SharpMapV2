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
    [XmlType(TypeName = "FirmType", Namespace = Declarations.SchemaVersion), Serializable]
    public class FirmType
    {
        [XmlIgnore] private List<AddressLine> _AddressLine;
        [XmlIgnore] private List<Department> _Department;
        [XmlIgnore] private List<FirmName> _FirmName;
        [XmlIgnore] private MailStopType _MailStop;
        [XmlIgnore] private PostalCode _PostalCode;
        [XmlIgnore] private string _Type;
        [XmlAnyElement] public XmlElement[] Any;

        [XmlAnyAttribute] public XmlAttribute[] AnyAttr;

        [XmlAttribute(AttributeName = "Type")]
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

        [XmlElement(Type = typeof (FirmName), ElementName = "FirmName", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<FirmName> FirmName
        {
            get
            {
                if (_FirmName == null) _FirmName = new List<FirmName>();
                return _FirmName;
            }
            set { _FirmName = value; }
        }

        [XmlElement(Type = typeof (Department), ElementName = "Department", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<Department> Department
        {
            get
            {
                if (_Department == null) _Department = new List<Department>();
                return _Department;
            }
            set { _Department = value; }
        }

        [XmlElement(Type = typeof (MailStopType), ElementName = "MailStop", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public MailStopType MailStop
        {
            get { return _MailStop; }
            set { _MailStop = value; }
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