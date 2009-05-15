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
    [XmlRoot(ElementName = "AdministrativeArea", Namespace = Declarations.SchemaVersion, IsNullable = false),
     Serializable]
    public class AdministrativeArea
    {
        [XmlIgnore] private List<AddressLine> _AddressLine;
        [XmlIgnore] private List<AdministrativeAreaName> _AdministrativeAreaName;
        [XmlIgnore] private string _Indicator;
        [XmlIgnore] private Locality _Locality;
        [XmlIgnore] private PostalCode _PostalCode;
        [XmlIgnore] private PostOffice _PostOffice;
        [XmlIgnore] private SubAdministrativeArea _SubAdministrativeArea;
        [XmlIgnore] private string _Type;

        [XmlIgnore] private string _UsageType;
        [XmlAnyElement] public XmlElement[] Any;
        [XmlAnyAttribute] public XmlAttribute[] AnyAttr;

        [XmlAttribute(AttributeName = "Type")]
        public string Type
        {
            get { return _Type; }
            set { _Type = value; }
        }

        [XmlAttribute(AttributeName = "UsageType")]
        public string UsageType
        {
            get { return _UsageType; }
            set { _UsageType = value; }
        }

        [XmlAttribute(AttributeName = "Indicator")]
        public string Indicator
        {
            get { return _Indicator; }
            set { _Indicator = value; }
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

        [XmlElement(Type = typeof (AdministrativeAreaName), ElementName = "AdministrativeAreaName", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<AdministrativeAreaName> AdministrativeAreaName
        {
            get
            {
                if (_AdministrativeAreaName == null) _AdministrativeAreaName = new List<AdministrativeAreaName>();
                return _AdministrativeAreaName;
            }
            set { _AdministrativeAreaName = value; }
        }

        [XmlElement(Type = typeof (SubAdministrativeArea), ElementName = "SubAdministrativeArea", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public SubAdministrativeArea SubAdministrativeArea
        {
            get { return _SubAdministrativeArea; }
            set { _SubAdministrativeArea = value; }
        }

        [XmlElement(Type = typeof (Locality), ElementName = "Locality", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public Locality Locality
        {
            get { return _Locality; }
            set { _Locality = value; }
        }

        [XmlElement(Type = typeof (PostOffice), ElementName = "PostOffice", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public PostOffice PostOffice
        {
            get { return _PostOffice; }
            set { _PostOffice = value; }
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
            Locality.MakeSchemaCompliant();
            PostOffice.MakeSchemaCompliant();
            PostalCode.MakeSchemaCompliant();
        }
    }
}