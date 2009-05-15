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
    [XmlType(TypeName = "PostalServiceElements", Namespace = Declarations.SchemaVersion), Serializable]
    public class PostalServiceElements
    {
        [XmlIgnore] private List<AddressIdentifier> _addressIdentifier;
        [XmlIgnore] private AddressLatitude _addressLatitude;
        [XmlIgnore] private AddressLatitudeDirection _addressLatitudeDirection;
        [XmlIgnore] private AddressLongitude _addressLongitude;
        [XmlIgnore] private AddressLongitudeDirection _addressLongitudeDirection;
        [XmlIgnore] private Barcode _barcode;
        [XmlIgnore] private EndorsementLineCode _endorsementLineCode;
        [XmlIgnore] private KeyLineCode _keyLineCode;
        [XmlIgnore] private SortingCode _sortingCode;
        [XmlIgnore] private List<SupplementaryPostalServiceData> _supplementaryPostalServiceData;
        [XmlIgnore] private string _type;
        [XmlAnyElement] public XmlElement[] Any;

        [XmlAnyAttribute] public XmlAttribute[] AnyAttr;

        [XmlAttribute(AttributeName = "Type")]
        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }

        [XmlElement(Type = typeof (AddressIdentifier), ElementName = "AddressIdentifier", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<AddressIdentifier> AddressIdentifier
        {
            get
            {
                if (_addressIdentifier == null) _addressIdentifier = new List<AddressIdentifier>();
                return _addressIdentifier;
            }
            set { _addressIdentifier = value; }
        }

        [XmlElement(Type = typeof (EndorsementLineCode), ElementName = "EndorsementLineCode", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public EndorsementLineCode EndorsementLineCode
        {
            get { return _endorsementLineCode; }
            set { _endorsementLineCode = value; }
        }

        [XmlElement(Type = typeof (KeyLineCode), ElementName = "KeyLineCode", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public KeyLineCode KeyLineCode
        {
            get { return _keyLineCode; }
            set { _keyLineCode = value; }
        }

        [XmlElement(Type = typeof (Barcode), ElementName = "Barcode", IsNullable = false, Form = XmlSchemaForm.Qualified
            , Namespace = Declarations.SchemaVersion)]
        public Barcode Barcode
        {
            get { return _barcode; }
            set { _barcode = value; }
        }

        [XmlElement(Type = typeof (SortingCode), ElementName = "SortingCode", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public SortingCode SortingCode
        {
            get { return _sortingCode; }
            set { _sortingCode = value; }
        }

        [XmlElement(Type = typeof (AddressLatitude), ElementName = "AddressLatitude", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public AddressLatitude AddressLatitude
        {
            get { return _addressLatitude; }
            set { _addressLatitude = value; }
        }

        [XmlElement(Type = typeof (AddressLatitudeDirection), ElementName = "AddressLatitudeDirection",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public AddressLatitudeDirection AddressLatitudeDirection
        {
            get { return _addressLatitudeDirection; }
            set { _addressLatitudeDirection = value; }
        }

        [XmlElement(Type = typeof (AddressLongitude), ElementName = "AddressLongitude", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public AddressLongitude AddressLongitude
        {
            get { return _addressLongitude; }
            set { _addressLongitude = value; }
        }

        [XmlElement(Type = typeof (AddressLongitudeDirection), ElementName = "AddressLongitudeDirection",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public AddressLongitudeDirection AddressLongitudeDirection
        {
            get { return _addressLongitudeDirection; }
            set { _addressLongitudeDirection = value; }
        }

        [XmlElement(Type = typeof (SupplementaryPostalServiceData), ElementName = "SupplementaryPostalServiceData",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<SupplementaryPostalServiceData> SupplementaryPostalServiceData
        {
            get
            {
                if (_supplementaryPostalServiceData == null)
                    _supplementaryPostalServiceData = new List<SupplementaryPostalServiceData>();
                return _supplementaryPostalServiceData;
            }
            set { _supplementaryPostalServiceData = value; }
        }

        public void MakeSchemaCompliant()
        {
        }
    }
}