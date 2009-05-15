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
        [XmlIgnore] private List<AddressLine> _AddressLine;
        [XmlIgnore] private string _Connector;
        [XmlIgnore] private DependentLocalityType _DependentLocality;
        [XmlIgnore] private List<DependentLocalityName> _DependentLocalityName;
        [XmlIgnore] private DependentLocalityNumber _DependentLocalityNumber;
        [XmlIgnore] private string _Indicator;
        [XmlIgnore] private LargeMailUserType _LargeMailUser;
        [XmlIgnore] private PostalCode _PostalCode;
        [XmlIgnore] private PostalRouteType _PostalRoute;
        [XmlIgnore] private PostBox _PostBox;
        [XmlIgnore] private PostOffice _PostOffice;
        [XmlIgnore] private Premise _Premise;
        [XmlIgnore] private Thoroughfare _Thoroughfare;
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

        [XmlAttribute(AttributeName = "Connector")]
        public string Connector
        {
            get { return _Connector; }
            set { _Connector = value; }
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

        [XmlElement(Type = typeof (DependentLocalityName), ElementName = "DependentLocalityName", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<DependentLocalityName> DependentLocalityName
        {
            get
            {
                if (_DependentLocalityName == null) _DependentLocalityName = new List<DependentLocalityName>();
                return _DependentLocalityName;
            }
            set { _DependentLocalityName = value; }
        }

        [XmlElement(Type = typeof (DependentLocalityNumber), ElementName = "DependentLocalityNumber", IsNullable = false
            , Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public DependentLocalityNumber DependentLocalityNumber
        {
            get { return _DependentLocalityNumber; }
            set { _DependentLocalityNumber = value; }
        }

        [XmlElement(Type = typeof (PostBox), ElementName = "PostBox", IsNullable = false, Form = XmlSchemaForm.Qualified
            , Namespace = Declarations.SchemaVersion)]
        public PostBox PostBox
        {
            get { return _PostBox; }
            set { _PostBox = value; }
        }

        [XmlElement(Type = typeof (LargeMailUserType), ElementName = "LargeMailUser", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public LargeMailUserType LargeMailUser
        {
            get { return _LargeMailUser; }
            set { _LargeMailUser = value; }
        }

        [XmlElement(Type = typeof (PostOffice), ElementName = "PostOffice", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public PostOffice PostOffice
        {
            get { return _PostOffice; }
            set { _PostOffice = value; }
        }

        [XmlElement(Type = typeof (PostalRouteType), ElementName = "PostalRoute", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public PostalRouteType PostalRoute
        {
            get { return _PostalRoute; }
            set { _PostalRoute = value; }
        }

        [XmlElement(Type = typeof (Thoroughfare), ElementName = "Thoroughfare", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public Thoroughfare Thoroughfare
        {
            get { return _Thoroughfare; }
            set { _Thoroughfare = value; }
        }

        [XmlElement(Type = typeof (Premise), ElementName = "Premise", IsNullable = false, Form = XmlSchemaForm.Qualified
            , Namespace = Declarations.SchemaVersion)]
        public Premise Premise
        {
            get { return _Premise; }
            set { _Premise = value; }
        }

        [XmlElement(Type = typeof (DependentLocalityType), ElementName = "DependentLocality", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public DependentLocalityType DependentLocality
        {
            get { return _DependentLocality; }
            set { _DependentLocality = value; }
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
            PostBox.MakeSchemaCompliant();
            LargeMailUser.MakeSchemaCompliant();
            PostOffice.MakeSchemaCompliant();
            PostalRoute.MakeSchemaCompliant();
        }
    }
}