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
    [XmlType(TypeName = "Country", Namespace = Declarations.SchemaVersion), Serializable]
    public class Country
    {
        [XmlIgnore] private List<AddressLine> _addressLine;
        [XmlIgnore] private AdministrativeArea _administrativeArea;
        [XmlIgnore] private List<CountryName> _countryName;

        [XmlIgnore] private List<CountryNameCode> _countryNameCode;
        [XmlIgnore] private Locality _locality;
        [XmlIgnore] private Thoroughfare _thoroughfare;
        [XmlAnyElement] public XmlElement[] Any;
        [XmlAnyAttribute] public XmlAttribute[] AnyAttr;

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

        [XmlElement(Type = typeof (CountryNameCode), ElementName = "CountryNameCode", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<CountryNameCode> CountryNameCode
        {
            get
            {
                if (_countryNameCode == null) _countryNameCode = new List<CountryNameCode>();
                return _countryNameCode;
            }
            set { _countryNameCode = value; }
        }

        [XmlElement(Type = typeof (CountryName), ElementName = "CountryName", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<CountryName> CountryName
        {
            get
            {
                if (_countryName == null) _countryName = new List<CountryName>();
                return _countryName;
            }
            set { _countryName = value; }
        }

        [XmlElement(Type = typeof (AdministrativeArea), ElementName = "AdministrativeArea", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public AdministrativeArea AdministrativeArea
        {
            get { return _administrativeArea; }
            set { _administrativeArea = value; }
        }

        [XmlElement(Type = typeof (Locality), ElementName = "Locality", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public Locality Locality
        {
            get { return _locality; }
            set { _locality = value; }
        }

        [XmlElement(Type = typeof (Thoroughfare), ElementName = "Thoroughfare", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public Thoroughfare Thoroughfare
        {
            get { return _thoroughfare; }
            set { _thoroughfare = value; }
        }

        public void MakeSchemaCompliant()
        {
            AdministrativeArea.MakeSchemaCompliant();
            Locality.MakeSchemaCompliant();
            Thoroughfare.MakeSchemaCompliant();
        }
    }
}