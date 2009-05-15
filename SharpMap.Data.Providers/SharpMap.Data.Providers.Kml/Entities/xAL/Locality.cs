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
    [XmlRoot(ElementName = "Locality", Namespace = Declarations.SchemaVersion, IsNullable = false), Serializable]
    public class Locality
    {
        [XmlIgnore] private List<AddressLine> _addressLine;
        [XmlIgnore] private DependentLocalityType _dependentLocality;
        [XmlIgnore] private string _indicator;
        [XmlIgnore] private LargeMailUserType _largeMailUser;
        [XmlIgnore] private List<LocalityName> _localityName;
        [XmlIgnore] private PostalCode _postalCode;
        [XmlIgnore] private PostalRouteType _postalRoute;
        [XmlIgnore] private PostBox _postBox;
        [XmlIgnore] private PostOffice _postOffice;
        [XmlIgnore] private Premise _premise;
        [XmlIgnore] private Thoroughfare _thoroughfare;
        [XmlIgnore] private string _type;

        [XmlIgnore] private string _usageType;
        [XmlAnyElement] public XmlElement[] Any;
        [XmlAnyAttribute] public XmlAttribute[] AnyAttr;

        [XmlAttribute(AttributeName = "Type")]
        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }

        [XmlAttribute(AttributeName = "UsageType")]
        public string UsageType
        {
            get { return _usageType; }
            set { _usageType = value; }
        }

        [XmlAttribute(AttributeName = "Indicator")]
        public string Indicator
        {
            get { return _indicator; }
            set { _indicator = value; }
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

        [XmlElement(Type = typeof (LocalityName), ElementName = "LocalityName", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<LocalityName> LocalityName
        {
            get
            {
                if (_localityName == null) _localityName = new List<LocalityName>();
                return _localityName;
            }
            set { _localityName = value; }
        }

        [XmlElement(Type = typeof (PostBox), ElementName = "PostBox", IsNullable = false, Form = XmlSchemaForm.Qualified
            , Namespace = Declarations.SchemaVersion)]
        public PostBox PostBox
        {
            get { return _postBox; }
            set { _postBox = value; }
        }

        [XmlElement(Type = typeof (LargeMailUserType), ElementName = "LargeMailUser", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public LargeMailUserType LargeMailUser
        {
            get { return _largeMailUser; }
            set { _largeMailUser = value; }
        }

        [XmlElement(Type = typeof (PostOffice), ElementName = "PostOffice", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public PostOffice PostOffice
        {
            get { return _postOffice; }
            set { _postOffice = value; }
        }

        [XmlElement(Type = typeof (PostalRouteType), ElementName = "PostalRoute", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public PostalRouteType PostalRoute
        {
            get { return _postalRoute; }
            set { _postalRoute = value; }
        }

        [XmlElement(Type = typeof (Thoroughfare), ElementName = "Thoroughfare", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public Thoroughfare Thoroughfare
        {
            get { return _thoroughfare; }
            set { _thoroughfare = value; }
        }

        [XmlElement(Type = typeof (Premise), ElementName = "Premise", IsNullable = false, Form = XmlSchemaForm.Qualified
            , Namespace = Declarations.SchemaVersion)]
        public Premise Premise
        {
            get { return _premise; }
            set { _premise = value; }
        }

        [XmlElement(Type = typeof (DependentLocalityType), ElementName = "DependentLocality", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public DependentLocalityType DependentLocality
        {
            get
            {
                //jd: stack overflow awaits you when serializing
                //
                return _dependentLocality;
            }
            set { _dependentLocality = value; }
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
            PostBox.MakeSchemaCompliant();
            LargeMailUser.MakeSchemaCompliant();
            PostOffice.MakeSchemaCompliant();
            PostalRoute.MakeSchemaCompliant();
        }
    }
}