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
    [XmlRoot(ElementName = "Thoroughfare", Namespace = Declarations.SchemaVersion, IsNullable = false), Serializable]
    public class Thoroughfare
    {
        [XmlIgnore] private List<AddressLine> _addressLine;
        [XmlIgnore] private DependentLocalityType _dependentLocality;
        [XmlIgnore] private DependentThoroughfare _dependentThoroughfare;
        [XmlIgnore] private DependentThoroughfares _dependentThoroughfares;
        [XmlIgnore] private string _dependentThoroughfaresConnector;
        [XmlIgnore] private string _dependentThoroughfaresIndicator;

        [XmlIgnore] public bool _dependentThoroughfaresSpecified;
        [XmlIgnore] private string _dependentThoroughfaresType;
        [XmlIgnore] private FirmType _firm;
        [XmlIgnore] private PostalCode _postalCode;
        [XmlIgnore] private Premise _premise;
        [XmlIgnore] private ThoroughfareLeadingTypeType _thoroughfareLeadingType;
        [XmlIgnore] private List<ThoroughfareNameType> _thoroughfareName;
        [XmlIgnore] private List<ThoroughfareNumber> _thoroughfareNumber;
        [XmlIgnore] private List<ThoroughfareNumberPrefix> _thoroughfareNumberPrefix;
        [XmlIgnore] private List<ThoroughfareNumberRange> _thoroughfareNumberRange;
        [XmlIgnore] private List<ThoroughfareNumberSuffix> _thoroughfareNumberSuffix;
        [XmlIgnore] private ThoroughfarePostDirectionType _thoroughfarePostDirection;
        [XmlIgnore] private ThoroughfarePreDirectionType _thoroughfarePreDirection;
        [XmlIgnore] private ThoroughfareTrailingTypeType _thoroughfareTrailingType;
        [XmlIgnore] private string _type;
        [XmlAnyElement] public XmlElement[] Any;
        [XmlAnyAttribute] public XmlAttribute[] AnyAttr;

        [XmlAttribute(AttributeName = "Type")]
        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }

        [XmlAttribute(AttributeName = "DependentThoroughfares")]
        public DependentThoroughfares DependentThoroughfares
        {
            get { return _dependentThoroughfares; }
            set
            {
                _dependentThoroughfares = value;
                _dependentThoroughfaresSpecified = true;
            }
        }

        [XmlAttribute(AttributeName = "DependentThoroughfaresIndicator")]
        public string DependentThoroughfaresIndicator
        {
            get { return _dependentThoroughfaresIndicator; }
            set { _dependentThoroughfaresIndicator = value; }
        }

        [XmlAttribute(AttributeName = "DependentThoroughfaresConnector")]
        public string DependentThoroughfaresConnector
        {
            get { return _dependentThoroughfaresConnector; }
            set { _dependentThoroughfaresConnector = value; }
        }

        [XmlAttribute(AttributeName = "DependentThoroughfaresType")]
        public string DependentThoroughfaresType
        {
            get { return _dependentThoroughfaresType; }
            set { _dependentThoroughfaresType = value; }
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

        [XmlElement(Type = typeof (ThoroughfareNumber), ElementName = "ThoroughfareNumber", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<ThoroughfareNumber> ThoroughfareNumber
        {
            get
            {
                if (_thoroughfareNumber == null) _thoroughfareNumber = new List<ThoroughfareNumber>();
                return _thoroughfareNumber;
            }
            set { _thoroughfareNumber = value; }
        }

        [XmlElement(Type = typeof (ThoroughfareNumberRange), ElementName = "ThoroughfareNumberRange", IsNullable = false
            , Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<ThoroughfareNumberRange> ThoroughfareNumberRange
        {
            get
            {
                if (_thoroughfareNumberRange == null) _thoroughfareNumberRange = new List<ThoroughfareNumberRange>();
                return _thoroughfareNumberRange;
            }
            set { _thoroughfareNumberRange = value; }
        }

        [XmlElement(Type = typeof (ThoroughfareNumberPrefix), ElementName = "ThoroughfareNumberPrefix",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<ThoroughfareNumberPrefix> ThoroughfareNumberPrefix
        {
            get
            {
                if (_thoroughfareNumberPrefix == null)
                    _thoroughfareNumberPrefix = new List<ThoroughfareNumberPrefix>();
                return _thoroughfareNumberPrefix;
            }
            set { _thoroughfareNumberPrefix = value; }
        }

        [XmlElement(Type = typeof (ThoroughfareNumberSuffix), ElementName = "ThoroughfareNumberSuffix",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<ThoroughfareNumberSuffix> ThoroughfareNumberSuffix
        {
            get
            {
                if (_thoroughfareNumberSuffix == null)
                    _thoroughfareNumberSuffix = new List<ThoroughfareNumberSuffix>();
                return _thoroughfareNumberSuffix;
            }
            set { _thoroughfareNumberSuffix = value; }
        }

        [XmlElement(Type = typeof (ThoroughfarePreDirectionType), ElementName = "ThoroughfarePreDirection",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public ThoroughfarePreDirectionType ThoroughfarePreDirection
        {
            get { return _thoroughfarePreDirection; }
            set { _thoroughfarePreDirection = value; }
        }

        [XmlElement(Type = typeof (ThoroughfareLeadingTypeType), ElementName = "ThoroughfareLeadingType",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public ThoroughfareLeadingTypeType ThoroughfareLeadingType
        {
            get { return _thoroughfareLeadingType; }
            set { _thoroughfareLeadingType = value; }
        }

        [XmlElement(Type = typeof (ThoroughfareNameType), ElementName = "ThoroughfareName", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<ThoroughfareNameType> ThoroughfareName
        {
            get
            {
                if (_thoroughfareName == null) _thoroughfareName = new List<ThoroughfareNameType>();
                return _thoroughfareName;
            }
            set { _thoroughfareName = value; }
        }

        [XmlElement(Type = typeof (ThoroughfareTrailingTypeType), ElementName = "ThoroughfareTrailingType",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public ThoroughfareTrailingTypeType ThoroughfareTrailingType
        {
            get { return _thoroughfareTrailingType; }
            set { _thoroughfareTrailingType = value; }
        }

        [XmlElement(Type = typeof (ThoroughfarePostDirectionType), ElementName = "ThoroughfarePostDirection",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public ThoroughfarePostDirectionType ThoroughfarePostDirection
        {
            get
            {
                if (_thoroughfarePostDirection == null)
                    _thoroughfarePostDirection = new ThoroughfarePostDirectionType();
                return _thoroughfarePostDirection;
            }
            set { _thoroughfarePostDirection = value; }
        }

        [XmlElement(Type = typeof (DependentThoroughfare), ElementName = "DependentThoroughfare", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public DependentThoroughfare DependentThoroughfare
        {
            get { return _dependentThoroughfare; }
            set { _dependentThoroughfare = value; }
        }

        [XmlElement(Type = typeof (DependentLocalityType), ElementName = "DependentLocality", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public DependentLocalityType DependentLocality
        {
            get { return _dependentLocality; }
            set { _dependentLocality = value; }
        }

        [XmlElement(Type = typeof (Premise), ElementName = "Premise", IsNullable = false, Form = XmlSchemaForm.Qualified
            , Namespace = Declarations.SchemaVersion)]
        public Premise Premise
        {
            get { return _premise; }
            set { _premise = value; }
        }

        [XmlElement(Type = typeof (FirmType), ElementName = "Firm", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = Declarations.SchemaVersion)]
        public FirmType Firm
        {
            get { return _firm; }
            set { _firm = value; }
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
            foreach (ThoroughfareNumber _c in ThoroughfareNumber) _c.MakeSchemaCompliant();
            foreach (ThoroughfareNumberRange _c in ThoroughfareNumberRange) _c.MakeSchemaCompliant();
            DependentLocality.MakeSchemaCompliant();
            Premise.MakeSchemaCompliant();
            Firm.MakeSchemaCompliant();
            PostalCode.MakeSchemaCompliant();
        }
    }
}