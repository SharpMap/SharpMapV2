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
    [XmlRoot(ElementName = "Premise", Namespace = Declarations.SchemaVersion, IsNullable = false), Serializable]
    public class Premise
    {
        [XmlIgnore] private List<AddressLine> _addressLine;
        [XmlIgnore] private List<BuildingNameType> _buildingName;
        [XmlIgnore] private FirmType _firm;
        [XmlIgnore] private MailStopType _mailStop;
        [XmlIgnore] private PostalCode _postalCode;
        [XmlIgnore] private Premise _premise;
        [XmlIgnore] private string _premiseDependency;
        [XmlIgnore] private string _premiseDependencyType;
        [XmlIgnore] private PremiseLocation _premiseLocation;
        [XmlIgnore] private List<PremiseName> _premiseName;
        [XmlIgnore] private List<PremiseNumber> _premiseNumber;
        [XmlIgnore] private List<PremiseNumberPrefix> _premiseNumberPrefix;
        [XmlIgnore] private PremiseNumberRange _premiseNumberRange;
        [XmlIgnore] private List<PremiseNumberSuffix> _premiseNumberSuffix;
        [XmlIgnore] private string _premiseThoroughfareConnector;
        [XmlIgnore] private List<SubPremiseType> _subPremise;
        [XmlIgnore] private string _type;
        [XmlAnyElement] public XmlElement[] Any;
        [XmlAnyAttribute] public XmlAttribute[] AnyAttr;

        [XmlAttribute(AttributeName = "Type")]
        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }

        [XmlAttribute(AttributeName = "PremiseDependency")]
        public string PremiseDependency
        {
            get { return _premiseDependency; }
            set { _premiseDependency = value; }
        }

        [XmlAttribute(AttributeName = "PremiseDependencyType")]
        public string PremiseDependencyType
        {
            get { return _premiseDependencyType; }
            set { _premiseDependencyType = value; }
        }

        [XmlAttribute(AttributeName = "PremiseThoroughfareConnector")]
        public string PremiseThoroughfareConnector
        {
            get { return _premiseThoroughfareConnector; }
            set { _premiseThoroughfareConnector = value; }
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

        [XmlElement(Type = typeof (PremiseName), ElementName = "PremiseName", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<PremiseName> PremiseName
        {
            get
            {
                if (_premiseName == null) _premiseName = new List<PremiseName>();
                return _premiseName;
            }
            set { _premiseName = value; }
        }

        [XmlElement(Type = typeof (PremiseLocation), ElementName = "PremiseLocation", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public PremiseLocation PremiseLocation
        {
            get { return _premiseLocation; }
            set { _premiseLocation = value; }
        }

        [XmlElement(Type = typeof (PremiseNumber), ElementName = "PremiseNumber", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<PremiseNumber> PremiseNumber
        {
            get
            {
                if (_premiseNumber == null) _premiseNumber = new List<PremiseNumber>();
                return _premiseNumber;
            }
            set { _premiseNumber = value; }
        }

        [XmlElement(Type = typeof (PremiseNumberRange), ElementName = "PremiseNumberRange", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public PremiseNumberRange PremiseNumberRange
        {
            get { return _premiseNumberRange; }
            set { _premiseNumberRange = value; }
        }

        [XmlElement(Type = typeof (PremiseNumberPrefix), ElementName = "PremiseNumberPrefix", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<PremiseNumberPrefix> PremiseNumberPrefix
        {
            get
            {
                if (_premiseNumberPrefix == null) _premiseNumberPrefix = new List<PremiseNumberPrefix>();
                return _premiseNumberPrefix;
            }
            set { _premiseNumberPrefix = value; }
        }

        [XmlElement(Type = typeof (PremiseNumberSuffix), ElementName = "PremiseNumberSuffix", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<PremiseNumberSuffix> PremiseNumberSuffix
        {
            get
            {
                if (_premiseNumberSuffix == null) _premiseNumberSuffix = new List<PremiseNumberSuffix>();
                return _premiseNumberSuffix;
            }
            set { _premiseNumberSuffix = value; }
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

        [XmlElement(Type = typeof (SubPremiseType), ElementName = "SubPremise", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<SubPremiseType> SubPremise
        {
            get
            {
                if (_subPremise == null) _subPremise = new List<SubPremiseType>();
                return _subPremise;
            }
            set { _subPremise = value; }
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

        [XmlElement(Type = typeof (Premise), ElementName = "Premise", IsNullable = false, Form = XmlSchemaForm.Qualified
            , Namespace = Declarations.SchemaVersion)]
        public Premise Premise1
        {
            get { return _premise; }
            set { _premise = value; }
        }

        public void MakeSchemaCompliant()
        {
            PremiseLocation.MakeSchemaCompliant();
            foreach (PremiseNumber _c in PremiseNumber) _c.MakeSchemaCompliant();
            PremiseNumberRange.MakeSchemaCompliant();
        }
    }
}