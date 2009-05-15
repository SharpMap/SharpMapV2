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
    [XmlType(TypeName = "DependentLocalityType", Namespace = Declarations.SchemaVersion), Serializable]
    public class DependentLocalityType
    {
        [XmlIgnore] private List<AddressLine> __AddressLine;
        [XmlIgnore] private string __Connector;
        [XmlIgnore] private DependentLocalityType __DependentLocality;
        [XmlIgnore] private List<DependentLocalityName> __DependentLocalityName;
        [XmlIgnore] private DependentLocalityNumber __DependentLocalityNumber;
        [XmlIgnore] private string __Indicator;
        [XmlIgnore] private LargeMailUserType __LargeMailUser;
        [XmlIgnore] private PostalCode __PostalCode;
        [XmlIgnore] private PostalRouteType __PostalRoute;
        [XmlIgnore] private PostBox __PostBox;
        [XmlIgnore] private PostOffice __PostOffice;
        [XmlIgnore] private Premise __Premise;
        [XmlIgnore] private Thoroughfare __Thoroughfare;
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

        [XmlAttribute(AttributeName = "Connector")]
        public string Connector
        {
            get { return __Connector; }
            set { __Connector = value; }
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

        [XmlElement(Type = typeof (DependentLocalityName), ElementName = "DependentLocalityName", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<DependentLocalityName> DependentLocalityName
        {
            get
            {
                if (__DependentLocalityName == null) __DependentLocalityName = new List<DependentLocalityName>();
                return __DependentLocalityName;
            }
            set { __DependentLocalityName = value; }
        }

        [XmlElement(Type = typeof (DependentLocalityNumber), ElementName = "DependentLocalityNumber", IsNullable = false
            , Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public DependentLocalityNumber DependentLocalityNumber
        {
            get
            {
                
                return __DependentLocalityNumber;
            }
            set { __DependentLocalityNumber = value; }
        }

        [XmlElement(Type = typeof (PostBox), ElementName = "PostBox", IsNullable = false, Form = XmlSchemaForm.Qualified
            , Namespace = Declarations.SchemaVersion)]
        public PostBox PostBox
        {
            get
            {
                
                return __PostBox;
            }
            set { __PostBox = value; }
        }

        [XmlElement(Type = typeof (LargeMailUserType), ElementName = "LargeMailUser", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public LargeMailUserType LargeMailUser
        {
            get
            {
                
                return __LargeMailUser;
            }
            set { __LargeMailUser = value; }
        }

        [XmlElement(Type = typeof (PostOffice), ElementName = "PostOffice", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public PostOffice PostOffice
        {
            get
            {
                
                return __PostOffice;
            }
            set { __PostOffice = value; }
        }

        [XmlElement(Type = typeof (PostalRouteType), ElementName = "PostalRoute", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public PostalRouteType PostalRoute
        {
            get
            {
                
                return __PostalRoute;
            }
            set { __PostalRoute = value; }
        }

        [XmlElement(Type = typeof (Thoroughfare), ElementName = "Thoroughfare", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public Thoroughfare Thoroughfare
        {
            get
            {
                
                return __Thoroughfare;
            }
            set { __Thoroughfare = value; }
        }

        [XmlElement(Type = typeof (Premise), ElementName = "Premise", IsNullable = false, Form = XmlSchemaForm.Qualified
            , Namespace = Declarations.SchemaVersion)]
        public Premise Premise
        {
            get
            {
                
                return __Premise;
            }
            set { __Premise = value; }
        }

        [XmlElement(Type = typeof (DependentLocalityType), ElementName = "DependentLocality", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public DependentLocalityType DependentLocality
        {
            get
            {
                
                return __DependentLocality;
            }
            set { __DependentLocality = value; }
        }

        [XmlElement(Type = typeof (PostalCode), ElementName = "PostalCode", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public PostalCode PostalCode
        {
            get
            {
                
                return __PostalCode;
            }
            set { __PostalCode = value; }
        }

        public void MakeSchemaCompliant()
        {
            PostBox.MakeSchemaCompliant();
            LargeMailUser.MakeSchemaCompliant();
            PostOffice.MakeSchemaCompliant();
            PostalRoute.MakeSchemaCompliant();
        }
    }
}