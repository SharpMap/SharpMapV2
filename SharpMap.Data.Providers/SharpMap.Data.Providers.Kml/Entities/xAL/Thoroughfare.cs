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
        [XmlIgnore] private List<AddressLine> __AddressLine;
        [XmlIgnore] private DependentLocalityType __DependentLocality;
        [XmlIgnore] private DependentThoroughfare __DependentThoroughfare;
        [XmlIgnore] private DependentThoroughfares __DependentThoroughfares;
        [XmlIgnore] private string __DependentThoroughfaresConnector;
        [XmlIgnore] private string __DependentThoroughfaresIndicator;

        [XmlIgnore] public bool __DependentThoroughfaresSpecified;
        [XmlIgnore] private string __DependentThoroughfaresType;
        [XmlIgnore] private FirmType __Firm;
        [XmlIgnore] private PostalCode __PostalCode;
        [XmlIgnore] private Premise __Premise;
        [XmlIgnore] private ThoroughfareLeadingTypeType __ThoroughfareLeadingType;
        [XmlIgnore] private List<ThoroughfareNameType> __ThoroughfareName;
        [XmlIgnore] private List<ThoroughfareNumber> __ThoroughfareNumber;
        [XmlIgnore] private List<ThoroughfareNumberPrefix> __ThoroughfareNumberPrefix;
        [XmlIgnore] private List<ThoroughfareNumberRange> __ThoroughfareNumberRange;
        [XmlIgnore] private List<ThoroughfareNumberSuffix> __ThoroughfareNumberSuffix;
        [XmlIgnore] private ThoroughfarePostDirectionType __ThoroughfarePostDirection;
        [XmlIgnore] private ThoroughfarePreDirectionType __ThoroughfarePreDirection;
        [XmlIgnore] private ThoroughfareTrailingTypeType __ThoroughfareTrailingType;
        [XmlIgnore] private string __Type;
        [XmlAnyElement] public XmlElement[] Any;
        [XmlAnyAttribute] public XmlAttribute[] AnyAttr;

        [XmlAttribute(AttributeName = "Type")]
        public string Type
        {
            get { return __Type; }
            set { __Type = value; }
        }

        [XmlAttribute(AttributeName = "DependentThoroughfares")]
        public DependentThoroughfares DependentThoroughfares
        {
            get { return __DependentThoroughfares; }
            set
            {
                __DependentThoroughfares = value;
                __DependentThoroughfaresSpecified = true;
            }
        }

        [XmlAttribute(AttributeName = "DependentThoroughfaresIndicator")]
        public string DependentThoroughfaresIndicator
        {
            get { return __DependentThoroughfaresIndicator; }
            set { __DependentThoroughfaresIndicator = value; }
        }

        [XmlAttribute(AttributeName = "DependentThoroughfaresConnector")]
        public string DependentThoroughfaresConnector
        {
            get { return __DependentThoroughfaresConnector; }
            set { __DependentThoroughfaresConnector = value; }
        }

        [XmlAttribute(AttributeName = "DependentThoroughfaresType")]
        public string DependentThoroughfaresType
        {
            get { return __DependentThoroughfaresType; }
            set { __DependentThoroughfaresType = value; }
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

        [XmlElement(Type = typeof (ThoroughfareNumber), ElementName = "ThoroughfareNumber", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<ThoroughfareNumber> ThoroughfareNumber
        {
            get
            {
                if (__ThoroughfareNumber == null) __ThoroughfareNumber = new List<ThoroughfareNumber>();
                return __ThoroughfareNumber;
            }
            set { __ThoroughfareNumber = value; }
        }

        [XmlElement(Type = typeof (ThoroughfareNumberRange), ElementName = "ThoroughfareNumberRange", IsNullable = false
            , Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<ThoroughfareNumberRange> ThoroughfareNumberRange
        {
            get
            {
                if (__ThoroughfareNumberRange == null) __ThoroughfareNumberRange = new List<ThoroughfareNumberRange>();
                return __ThoroughfareNumberRange;
            }
            set { __ThoroughfareNumberRange = value; }
        }

        [XmlElement(Type = typeof (ThoroughfareNumberPrefix), ElementName = "ThoroughfareNumberPrefix",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<ThoroughfareNumberPrefix> ThoroughfareNumberPrefix
        {
            get
            {
                if (__ThoroughfareNumberPrefix == null)
                    __ThoroughfareNumberPrefix = new List<ThoroughfareNumberPrefix>();
                return __ThoroughfareNumberPrefix;
            }
            set { __ThoroughfareNumberPrefix = value; }
        }

        [XmlElement(Type = typeof (ThoroughfareNumberSuffix), ElementName = "ThoroughfareNumberSuffix",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<ThoroughfareNumberSuffix> ThoroughfareNumberSuffix
        {
            get
            {
                if (__ThoroughfareNumberSuffix == null)
                    __ThoroughfareNumberSuffix = new List<ThoroughfareNumberSuffix>();
                return __ThoroughfareNumberSuffix;
            }
            set { __ThoroughfareNumberSuffix = value; }
        }

        [XmlElement(Type = typeof (ThoroughfarePreDirectionType), ElementName = "ThoroughfarePreDirection",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public ThoroughfarePreDirectionType ThoroughfarePreDirection
        {
            get
            {
                
                return __ThoroughfarePreDirection;
            }
            set { __ThoroughfarePreDirection = value; }
        }

        [XmlElement(Type = typeof (ThoroughfareLeadingTypeType), ElementName = "ThoroughfareLeadingType",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public ThoroughfareLeadingTypeType ThoroughfareLeadingType
        {
            get
            {
                
                return __ThoroughfareLeadingType;
            }
            set { __ThoroughfareLeadingType = value; }
        }

        [XmlElement(Type = typeof (ThoroughfareNameType), ElementName = "ThoroughfareName", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<ThoroughfareNameType> ThoroughfareName
        {
            get
            {
                if (__ThoroughfareName == null) __ThoroughfareName = new List<ThoroughfareNameType>();
                return __ThoroughfareName;
            }
            set { __ThoroughfareName = value; }
        }

        [XmlElement(Type = typeof (ThoroughfareTrailingTypeType), ElementName = "ThoroughfareTrailingType",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public ThoroughfareTrailingTypeType ThoroughfareTrailingType
        {
            get
            {
                
                return __ThoroughfareTrailingType;
            }
            set { __ThoroughfareTrailingType = value; }
        }

        [XmlElement(Type = typeof (ThoroughfarePostDirectionType), ElementName = "ThoroughfarePostDirection",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public ThoroughfarePostDirectionType ThoroughfarePostDirection
        {
            get
            {
                if (__ThoroughfarePostDirection == null)
                    __ThoroughfarePostDirection = new ThoroughfarePostDirectionType();
                return __ThoroughfarePostDirection;
            }
            set { __ThoroughfarePostDirection = value; }
        }

        [XmlElement(Type = typeof (DependentThoroughfare), ElementName = "DependentThoroughfare", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public DependentThoroughfare DependentThoroughfare
        {
            get
            {
                
                return __DependentThoroughfare;
            }
            set { __DependentThoroughfare = value; }
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

        [XmlElement(Type = typeof (FirmType), ElementName = "Firm", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = Declarations.SchemaVersion)]
        public FirmType Firm
        {
            get
            {
                
                return __Firm;
            }
            set { __Firm = value; }
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
            foreach (ThoroughfareNumber _c in ThoroughfareNumber) _c.MakeSchemaCompliant();
            foreach (ThoroughfareNumberRange _c in ThoroughfareNumberRange) _c.MakeSchemaCompliant();
            DependentLocality.MakeSchemaCompliant();
            Premise.MakeSchemaCompliant();
            Firm.MakeSchemaCompliant();
            PostalCode.MakeSchemaCompliant();
        }
    }
}