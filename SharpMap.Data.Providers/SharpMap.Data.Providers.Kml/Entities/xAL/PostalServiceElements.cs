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
        [XmlIgnore] private List<AddressIdentifier> __AddressIdentifier;
        [XmlIgnore] private AddressLatitude __AddressLatitude;
        [XmlIgnore] private AddressLatitudeDirection __AddressLatitudeDirection;
        [XmlIgnore] private AddressLongitude __AddressLongitude;
        [XmlIgnore] private AddressLongitudeDirection __AddressLongitudeDirection;
        [XmlIgnore] private Barcode __Barcode;
        [XmlIgnore] private EndorsementLineCode __EndorsementLineCode;
        [XmlIgnore] private KeyLineCode __KeyLineCode;
        [XmlIgnore] private SortingCode __SortingCode;
        [XmlIgnore] private List<SupplementaryPostalServiceData> __SupplementaryPostalServiceData;
        [XmlIgnore] private string __Type;
        [XmlAnyElement] public XmlElement[] Any;

        [XmlAnyAttribute] public XmlAttribute[] AnyAttr;

        [XmlAttribute(AttributeName = "Type")]
        public string Type
        {
            get { return __Type; }
            set { __Type = value; }
        }

        [XmlElement(Type = typeof (AddressIdentifier), ElementName = "AddressIdentifier", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<AddressIdentifier> AddressIdentifier
        {
            get
            {
                if (__AddressIdentifier == null) __AddressIdentifier = new List<AddressIdentifier>();
                return __AddressIdentifier;
            }
            set { __AddressIdentifier = value; }
        }

        [XmlElement(Type = typeof (EndorsementLineCode), ElementName = "EndorsementLineCode", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public EndorsementLineCode EndorsementLineCode
        {
            get
            {
                if (__EndorsementLineCode == null) __EndorsementLineCode = new EndorsementLineCode();
                return __EndorsementLineCode;
            }
            set { __EndorsementLineCode = value; }
        }

        [XmlElement(Type = typeof (KeyLineCode), ElementName = "KeyLineCode", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public KeyLineCode KeyLineCode
        {
            get
            {
                if (__KeyLineCode == null) __KeyLineCode = new KeyLineCode();
                return __KeyLineCode;
            }
            set { __KeyLineCode = value; }
        }

        [XmlElement(Type = typeof (Barcode), ElementName = "Barcode", IsNullable = false, Form = XmlSchemaForm.Qualified
            , Namespace = Declarations.SchemaVersion)]
        public Barcode Barcode
        {
            get
            {
                if (__Barcode == null) __Barcode = new Barcode();
                return __Barcode;
            }
            set { __Barcode = value; }
        }

        [XmlElement(Type = typeof (SortingCode), ElementName = "SortingCode", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public SortingCode SortingCode
        {
            get
            {
                if (__SortingCode == null) __SortingCode = new SortingCode();
                return __SortingCode;
            }
            set { __SortingCode = value; }
        }

        [XmlElement(Type = typeof (AddressLatitude), ElementName = "AddressLatitude", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public AddressLatitude AddressLatitude
        {
            get
            {
                if (__AddressLatitude == null) __AddressLatitude = new AddressLatitude();
                return __AddressLatitude;
            }
            set { __AddressLatitude = value; }
        }

        [XmlElement(Type = typeof (AddressLatitudeDirection), ElementName = "AddressLatitudeDirection",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public AddressLatitudeDirection AddressLatitudeDirection
        {
            get
            {
                if (__AddressLatitudeDirection == null) __AddressLatitudeDirection = new AddressLatitudeDirection();
                return __AddressLatitudeDirection;
            }
            set { __AddressLatitudeDirection = value; }
        }

        [XmlElement(Type = typeof (AddressLongitude), ElementName = "AddressLongitude", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public AddressLongitude AddressLongitude
        {
            get
            {
                if (__AddressLongitude == null) __AddressLongitude = new AddressLongitude();
                return __AddressLongitude;
            }
            set { __AddressLongitude = value; }
        }

        [XmlElement(Type = typeof (AddressLongitudeDirection), ElementName = "AddressLongitudeDirection",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public AddressLongitudeDirection AddressLongitudeDirection
        {
            get
            {
                if (__AddressLongitudeDirection == null) __AddressLongitudeDirection = new AddressLongitudeDirection();
                return __AddressLongitudeDirection;
            }
            set { __AddressLongitudeDirection = value; }
        }

        [XmlElement(Type = typeof (SupplementaryPostalServiceData), ElementName = "SupplementaryPostalServiceData",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<SupplementaryPostalServiceData> SupplementaryPostalServiceData
        {
            get
            {
                if (__SupplementaryPostalServiceData == null)
                    __SupplementaryPostalServiceData = new List<SupplementaryPostalServiceData>();
                return __SupplementaryPostalServiceData;
            }
            set { __SupplementaryPostalServiceData = value; }
        }

        public void MakeSchemaCompliant()
        {
        }
    }
}