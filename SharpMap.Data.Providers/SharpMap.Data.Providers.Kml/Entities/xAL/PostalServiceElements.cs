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
    [XmlType(TypeName = "PostalServiceElements", Namespace = Declarations.SchemaVersion), Serializable]
    public class PostalServiceElements
    {
        [XmlIgnore] private List<AddressIdentifier> _AddressIdentifier;
        [XmlIgnore] private AddressLatitude _AddressLatitude;
        [XmlIgnore] private AddressLatitudeDirection _AddressLatitudeDirection;
        [XmlIgnore] private AddressLongitude _AddressLongitude;
        [XmlIgnore] private AddressLongitudeDirection _AddressLongitudeDirection;
        [XmlIgnore] private Barcode _Barcode;
        [XmlIgnore] private EndorsementLineCode _EndorsementLineCode;
        [XmlIgnore] private KeyLineCode _KeyLineCode;
        [XmlIgnore] private SortingCode _SortingCode;
        [XmlIgnore] private List<SupplementaryPostalServiceData> _SupplementaryPostalServiceData;
        [XmlIgnore] private string _Type;
        [XmlAnyElement] public XmlElement[] Any;

        [XmlAnyAttribute] public XmlAttribute[] AnyAttr;

        [XmlAttribute(AttributeName = "Type")]
        public string Type
        {
            get { return _Type; }
            set { _Type = value; }
        }

        [XmlElement(Type = typeof (AddressIdentifier), ElementName = "AddressIdentifier", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<AddressIdentifier> AddressIdentifier
        {
            get
            {
                if (_AddressIdentifier == null) _AddressIdentifier = new List<AddressIdentifier>();
                return _AddressIdentifier;
            }
            set { _AddressIdentifier = value; }
        }

        [XmlElement(Type = typeof (EndorsementLineCode), ElementName = "EndorsementLineCode", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public EndorsementLineCode EndorsementLineCode
        {
            get { return _EndorsementLineCode; }
            set { _EndorsementLineCode = value; }
        }

        [XmlElement(Type = typeof (KeyLineCode), ElementName = "KeyLineCode", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public KeyLineCode KeyLineCode
        {
            get { return _KeyLineCode; }
            set { _KeyLineCode = value; }
        }

        [XmlElement(Type = typeof (Barcode), ElementName = "Barcode", IsNullable = false, Form = XmlSchemaForm.Qualified
            , Namespace = Declarations.SchemaVersion)]
        public Barcode Barcode
        {
            get { return _Barcode; }
            set { _Barcode = value; }
        }

        [XmlElement(Type = typeof (SortingCode), ElementName = "SortingCode", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public SortingCode SortingCode
        {
            get { return _SortingCode; }
            set { _SortingCode = value; }
        }

        [XmlElement(Type = typeof (AddressLatitude), ElementName = "AddressLatitude", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public AddressLatitude AddressLatitude
        {
            get { return _AddressLatitude; }
            set { _AddressLatitude = value; }
        }

        [XmlElement(Type = typeof (AddressLatitudeDirection), ElementName = "AddressLatitudeDirection",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public AddressLatitudeDirection AddressLatitudeDirection
        {
            get { return _AddressLatitudeDirection; }
            set { _AddressLatitudeDirection = value; }
        }

        [XmlElement(Type = typeof (AddressLongitude), ElementName = "AddressLongitude", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public AddressLongitude AddressLongitude
        {
            get { return _AddressLongitude; }
            set { _AddressLongitude = value; }
        }

        [XmlElement(Type = typeof (AddressLongitudeDirection), ElementName = "AddressLongitudeDirection",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public AddressLongitudeDirection AddressLongitudeDirection
        {
            get { return _AddressLongitudeDirection; }
            set { _AddressLongitudeDirection = value; }
        }

        [XmlElement(Type = typeof (SupplementaryPostalServiceData), ElementName = "SupplementaryPostalServiceData",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<SupplementaryPostalServiceData> SupplementaryPostalServiceData
        {
            get
            {
                if (_SupplementaryPostalServiceData == null)
                    _SupplementaryPostalServiceData = new List<SupplementaryPostalServiceData>();
                return _SupplementaryPostalServiceData;
            }
            set { _SupplementaryPostalServiceData = value; }
        }

        public void MakeSchemaCompliant()
        {
        }
    }
}