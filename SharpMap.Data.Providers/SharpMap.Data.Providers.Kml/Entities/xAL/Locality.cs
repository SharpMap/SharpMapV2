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
    [XmlRoot(ElementName = "Locality", Namespace = Declarations.SchemaVersion, IsNullable = false), Serializable]
    public class Locality
    {
        [XmlIgnore] private List<AddressLine> __AddressLine;
        [XmlIgnore] private DependentLocalityType __DependentLocality;
        [XmlIgnore] private string __Indicator;
        [XmlIgnore] private LargeMailUserType __LargeMailUser;
        [XmlIgnore] private List<LocalityName> __LocalityName;
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

        [XmlElement(Type = typeof (LocalityName), ElementName = "LocalityName", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<LocalityName> LocalityName
        {
            get
            {
                if (__LocalityName == null) __LocalityName = new List<LocalityName>();
                return __LocalityName;
            }
            set { __LocalityName = value; }
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

        [XmlElement(Type = typeof (LargeMailUserType), ElementName = "LargeMailUser", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public LargeMailUserType LargeMailUser
        {
            get
            {
                if (__LargeMailUser == null) __LargeMailUser = new LargeMailUserType();
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
                if (__PostOffice == null) __PostOffice = new PostOffice();
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
                if (__PostalRoute == null) __PostalRoute = new PostalRouteType();
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
                if (__Thoroughfare == null) __Thoroughfare = new Thoroughfare();
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
                if (__Premise == null) __Premise = new Premise();
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
                if (__DependentLocality == null) __DependentLocality = new DependentLocalityType();
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
                if (__PostalCode == null) __PostalCode = new PostalCode();
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