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
    [XmlType(TypeName = "SubPremiseType", Namespace = Declarations.SchemaVersion), Serializable]
    public class SubPremiseType
    {
        [XmlIgnore] private List<AddressLine> _addressLine;
        [XmlIgnore] private List<BuildingNameType> _buildingName;
        [XmlIgnore] private FirmType _firm;
        [XmlIgnore] private MailStopType _mailStop;
        [XmlIgnore] private PostalCode _postalCode;
        [XmlIgnore] private SubPremiseType _subPremise;
        [XmlIgnore] private SubPremiseLocation _subPremiseLocation;
        [XmlIgnore] private List<SubPremiseName> _subPremiseName;
        [XmlIgnore] private List<SubPremiseNumber> _subPremiseNumber;
        [XmlIgnore] private List<SubPremiseNumberPrefix> _subPremiseNumberPrefix;
        [XmlIgnore] private List<SubPremiseNumberSuffix> _subPremiseNumberSuffix;
        [XmlIgnore] private string _type;
        [XmlAnyElement] public XmlElement[] Any;

        [XmlAnyAttribute] public XmlAttribute[] AnyAttr;

        [XmlAttribute(AttributeName = "Type")]
        public string Type
        {
            get { return _type; }
            set { _type = value; }
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

        [XmlElement(Type = typeof (SubPremiseName), ElementName = "SubPremiseName", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<SubPremiseName> SubPremiseName
        {
            get
            {
                if (_subPremiseName == null) _subPremiseName = new List<SubPremiseName>();
                return _subPremiseName;
            }
            set { _subPremiseName = value; }
        }

        [XmlElement(Type = typeof (SubPremiseLocation), ElementName = "SubPremiseLocation", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public SubPremiseLocation SubPremiseLocation
        {
            get { return _subPremiseLocation; }
            set { _subPremiseLocation = value; }
        }

        [XmlElement(Type = typeof (SubPremiseNumber), ElementName = "SubPremiseNumber", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<SubPremiseNumber> SubPremiseNumber
        {
            get
            {
                if (_subPremiseNumber == null) _subPremiseNumber = new List<SubPremiseNumber>();
                return _subPremiseNumber;
            }
            set { _subPremiseNumber = value; }
        }

        [XmlElement(Type = typeof (SubPremiseNumberPrefix), ElementName = "SubPremiseNumberPrefix", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<SubPremiseNumberPrefix> SubPremiseNumberPrefix
        {
            get
            {
                if (_subPremiseNumberPrefix == null) _subPremiseNumberPrefix = new List<SubPremiseNumberPrefix>();
                return _subPremiseNumberPrefix;
            }
            set { _subPremiseNumberPrefix = value; }
        }

        [XmlElement(Type = typeof (SubPremiseNumberSuffix), ElementName = "SubPremiseNumberSuffix", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<SubPremiseNumberSuffix> SubPremiseNumberSuffix
        {
            get
            {
                if (_subPremiseNumberSuffix == null) _subPremiseNumberSuffix = new List<SubPremiseNumberSuffix>();
                return _subPremiseNumberSuffix;
            }
            set { _subPremiseNumberSuffix = value; }
        }

        [XmlElement(Type = typeof (BuildingNameType), ElementName = "BuildingName", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<BuildingNameType> BuildingName
        {
            get
            {
                if (_buildingName == null) _buildingName = new List<BuildingNameType>();
                return _buildingName;
            }
            set { _buildingName = value; }
        }

        [XmlElement(Type = typeof (FirmType), ElementName = "Firm", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = Declarations.SchemaVersion)]
        public FirmType Firm
        {
            get { return _firm; }
            set { _firm = value; }
        }

        [XmlElement(Type = typeof (MailStopType), ElementName = "MailStop", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public MailStopType MailStop
        {
            get { return _mailStop; }
            set { _mailStop = value; }
        }

        [XmlElement(Type = typeof (PostalCode), ElementName = "PostalCode", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public PostalCode PostalCode
        {
            get { return _postalCode; }
            set { _postalCode = value; }
        }

        [XmlElement(Type = typeof (SubPremiseType), ElementName = "SubPremise", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public SubPremiseType SubPremise
        {
            get { return _subPremise; }
            set { _subPremise = value; }
        }

        public void MakeSchemaCompliant()
        {
            SubPremiseLocation.MakeSchemaCompliant();
        }
    }
}