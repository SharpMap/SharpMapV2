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
    [XmlRoot(ElementName = "Thoroughfare", Namespace = Declarations.SchemaVersion, IsNullable = false), Serializable]
    public class Thoroughfare
    {
        [XmlIgnore] private List<AddressLine> _AddressLine;
        [XmlIgnore] private DependentLocalityType _DependentLocality;
        [XmlIgnore] private DependentThoroughfare _DependentThoroughfare;
        [XmlIgnore] private DependentThoroughfares _DependentThoroughfares;
        [XmlIgnore] private string _DependentThoroughfaresConnector;
        [XmlIgnore] private string _DependentThoroughfaresIndicator;

        [XmlIgnore] public bool _DependentThoroughfaresSpecified;
        [XmlIgnore] private string _DependentThoroughfaresType;
        [XmlIgnore] private FirmType _Firm;
        [XmlIgnore] private PostalCode _PostalCode;
        [XmlIgnore] private Premise _Premise;
        [XmlIgnore] private ThoroughfareLeadingTypeType _ThoroughfareLeadingType;
        [XmlIgnore] private List<ThoroughfareNameType> _ThoroughfareName;
        [XmlIgnore] private List<ThoroughfareNumber> _ThoroughfareNumber;
        [XmlIgnore] private List<ThoroughfareNumberPrefix> _ThoroughfareNumberPrefix;
        [XmlIgnore] private List<ThoroughfareNumberRange> _ThoroughfareNumberRange;
        [XmlIgnore] private List<ThoroughfareNumberSuffix> _ThoroughfareNumberSuffix;
        [XmlIgnore] private ThoroughfarePostDirectionType _ThoroughfarePostDirection;
        [XmlIgnore] private ThoroughfarePreDirectionType _ThoroughfarePreDirection;
        [XmlIgnore] private ThoroughfareTrailingTypeType _ThoroughfareTrailingType;
        [XmlIgnore] private string _Type;
        [XmlAnyElement] public XmlElement[] Any;
        [XmlAnyAttribute] public XmlAttribute[] AnyAttr;

        [XmlAttribute(AttributeName = "Type")]
        public string Type
        {
            get { return _Type; }
            set { _Type = value; }
        }

        [XmlAttribute(AttributeName = "DependentThoroughfares")]
        public DependentThoroughfares DependentThoroughfares
        {
            get { return _DependentThoroughfares; }
            set
            {
                _DependentThoroughfares = value;
                _DependentThoroughfaresSpecified = true;
            }
        }

        [XmlAttribute(AttributeName = "DependentThoroughfaresIndicator")]
        public string DependentThoroughfaresIndicator
        {
            get { return _DependentThoroughfaresIndicator; }
            set { _DependentThoroughfaresIndicator = value; }
        }

        [XmlAttribute(AttributeName = "DependentThoroughfaresConnector")]
        public string DependentThoroughfaresConnector
        {
            get { return _DependentThoroughfaresConnector; }
            set { _DependentThoroughfaresConnector = value; }
        }

        [XmlAttribute(AttributeName = "DependentThoroughfaresType")]
        public string DependentThoroughfaresType
        {
            get { return _DependentThoroughfaresType; }
            set { _DependentThoroughfaresType = value; }
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

        [XmlElement(Type = typeof (ThoroughfareNumber), ElementName = "ThoroughfareNumber", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<ThoroughfareNumber> ThoroughfareNumber
        {
            get
            {
                if (_ThoroughfareNumber == null) _ThoroughfareNumber = new List<ThoroughfareNumber>();
                return _ThoroughfareNumber;
            }
            set { _ThoroughfareNumber = value; }
        }

        [XmlElement(Type = typeof (ThoroughfareNumberRange), ElementName = "ThoroughfareNumberRange", IsNullable = false
            , Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<ThoroughfareNumberRange> ThoroughfareNumberRange
        {
            get
            {
                if (_ThoroughfareNumberRange == null) _ThoroughfareNumberRange = new List<ThoroughfareNumberRange>();
                return _ThoroughfareNumberRange;
            }
            set { _ThoroughfareNumberRange = value; }
        }

        [XmlElement(Type = typeof (ThoroughfareNumberPrefix), ElementName = "ThoroughfareNumberPrefix",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<ThoroughfareNumberPrefix> ThoroughfareNumberPrefix
        {
            get
            {
                if (_ThoroughfareNumberPrefix == null)
                    _ThoroughfareNumberPrefix = new List<ThoroughfareNumberPrefix>();
                return _ThoroughfareNumberPrefix;
            }
            set { _ThoroughfareNumberPrefix = value; }
        }

        [XmlElement(Type = typeof (ThoroughfareNumberSuffix), ElementName = "ThoroughfareNumberSuffix",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<ThoroughfareNumberSuffix> ThoroughfareNumberSuffix
        {
            get
            {
                if (_ThoroughfareNumberSuffix == null)
                    _ThoroughfareNumberSuffix = new List<ThoroughfareNumberSuffix>();
                return _ThoroughfareNumberSuffix;
            }
            set { _ThoroughfareNumberSuffix = value; }
        }

        [XmlElement(Type = typeof (ThoroughfarePreDirectionType), ElementName = "ThoroughfarePreDirection",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public ThoroughfarePreDirectionType ThoroughfarePreDirection
        {
            get { return _ThoroughfarePreDirection; }
            set { _ThoroughfarePreDirection = value; }
        }

        [XmlElement(Type = typeof (ThoroughfareLeadingTypeType), ElementName = "ThoroughfareLeadingType",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public ThoroughfareLeadingTypeType ThoroughfareLeadingType
        {
            get { return _ThoroughfareLeadingType; }
            set { _ThoroughfareLeadingType = value; }
        }

        [XmlElement(Type = typeof (ThoroughfareNameType), ElementName = "ThoroughfareName", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<ThoroughfareNameType> ThoroughfareName
        {
            get
            {
                if (_ThoroughfareName == null) _ThoroughfareName = new List<ThoroughfareNameType>();
                return _ThoroughfareName;
            }
            set { _ThoroughfareName = value; }
        }

        [XmlElement(Type = typeof (ThoroughfareTrailingTypeType), ElementName = "ThoroughfareTrailingType",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public ThoroughfareTrailingTypeType ThoroughfareTrailingType
        {
            get { return _ThoroughfareTrailingType; }
            set { _ThoroughfareTrailingType = value; }
        }

        [XmlElement(Type = typeof (ThoroughfarePostDirectionType), ElementName = "ThoroughfarePostDirection",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public ThoroughfarePostDirectionType ThoroughfarePostDirection
        {
            get
            {
                if (_ThoroughfarePostDirection == null)
                    _ThoroughfarePostDirection = new ThoroughfarePostDirectionType();
                return _ThoroughfarePostDirection;
            }
            set { _ThoroughfarePostDirection = value; }
        }

        [XmlElement(Type = typeof (DependentThoroughfare), ElementName = "DependentThoroughfare", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public DependentThoroughfare DependentThoroughfare
        {
            get { return _DependentThoroughfare; }
            set { _DependentThoroughfare = value; }
        }

        [XmlElement(Type = typeof (DependentLocalityType), ElementName = "DependentLocality", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public DependentLocalityType DependentLocality
        {
            get { return _DependentLocality; }
            set { _DependentLocality = value; }
        }

        [XmlElement(Type = typeof (Premise), ElementName = "Premise", IsNullable = false, Form = XmlSchemaForm.Qualified
            , Namespace = Declarations.SchemaVersion)]
        public Premise Premise
        {
            get { return _Premise; }
            set { _Premise = value; }
        }

        [XmlElement(Type = typeof (FirmType), ElementName = "Firm", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = Declarations.SchemaVersion)]
        public FirmType Firm
        {
            get { return _Firm; }
            set { _Firm = value; }
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
            foreach (ThoroughfareNumber _c in ThoroughfareNumber) _c.MakeSchemaCompliant();
            foreach (ThoroughfareNumberRange _c in ThoroughfareNumberRange) _c.MakeSchemaCompliant();
            DependentLocality.MakeSchemaCompliant();
            Premise.MakeSchemaCompliant();
            Firm.MakeSchemaCompliant();
            PostalCode.MakeSchemaCompliant();
        }
    }
}