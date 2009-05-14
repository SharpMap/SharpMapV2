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
    [XmlType(TypeName = "SubAdministrativeArea", Namespace = Declarations.SchemaVersion), Serializable]
    public class SubAdministrativeArea
    {
        [XmlIgnore] private List<AddressLine> __AddressLine;
        [XmlIgnore] private string __Indicator;
        [XmlIgnore] private Locality __Locality;
        [XmlIgnore] private PostalCode __PostalCode;
        [XmlIgnore] private PostOffice __PostOffice;
        [XmlIgnore] private List<SubAdministrativeAreaName> __SubAdministrativeAreaName;
        [XmlIgnore] private string __Type;

        [XmlIgnore] private string __UsageType;
        [XmlAnyElement] public XmlElement[] Any;
        [XmlAnyAttribute] public XmlAttribute[] AnyAttr;

        [XmlAttribute(AttributeName = "Type")]
        public string Type
        {
            get { return __Type; }
            set { __Type = value; }
        }

        [XmlAttribute(AttributeName = "UsageType")]
        public string UsageType
        {
            get { return __UsageType; }
            set { __UsageType = value; }
        }

        [XmlAttribute(AttributeName = "Indicator")]
        public string Indicator
        {
            get { return __Indicator; }
            set { __Indicator = value; }
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

        [XmlElement(Type = typeof (SubAdministrativeAreaName), ElementName = "SubAdministrativeAreaName",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<SubAdministrativeAreaName> SubAdministrativeAreaName
        {
            get
            {
                if (__SubAdministrativeAreaName == null)
                    __SubAdministrativeAreaName = new List<SubAdministrativeAreaName>();
                return __SubAdministrativeAreaName;
            }
            set { __SubAdministrativeAreaName = value; }
        }

        [XmlElement(Type = typeof (Locality), ElementName = "Locality", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public Locality Locality
        {
            get
            {
                if (__Locality == null) __Locality = new Locality();
                return __Locality;
            }
            set { __Locality = value; }
        }

        [XmlElement(Type = typeof (PostOffice), ElementName = "PostOffice", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public PostOffice PostOffice
        {
            get
            {
                if (__PostOffice == null) __PostOffice = new PostOffice();
                return __PostOffice;
            }
            set { __PostOffice = value; }
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
            Locality.MakeSchemaCompliant();
            PostOffice.MakeSchemaCompliant();
            PostalCode.MakeSchemaCompliant();
        }
    }
}