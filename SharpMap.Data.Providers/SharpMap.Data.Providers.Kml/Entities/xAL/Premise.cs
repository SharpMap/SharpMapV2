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
    [XmlRoot(ElementName = "Premise", Namespace = Declarations.SchemaVersion, IsNullable = false), Serializable]
    public class Premise
    {
        [XmlIgnore] private List<AddressLine> _AddressLine;
        [XmlIgnore] private List<BuildingNameType> _BuildingName;
        [XmlIgnore] private FirmType _Firm;
        [XmlIgnore] private MailStopType _MailStop;
        [XmlIgnore] private PostalCode _PostalCode;
        [XmlIgnore] private Premise _Premise;
        [XmlIgnore] private string _PremiseDependency;
        [XmlIgnore] private string _PremiseDependencyType;
        [XmlIgnore] private PremiseLocation _PremiseLocation;
        [XmlIgnore] private List<PremiseName> _PremiseName;
        [XmlIgnore] private List<PremiseNumber> _PremiseNumber;
        [XmlIgnore] private List<PremiseNumberPrefix> _PremiseNumberPrefix;
        [XmlIgnore] private PremiseNumberRange _PremiseNumberRange;
        [XmlIgnore] private List<PremiseNumberSuffix> _PremiseNumberSuffix;
        [XmlIgnore] private string _PremiseThoroughfareConnector;
        [XmlIgnore] private List<SubPremiseType> _SubPremise;
        [XmlIgnore] private string _Type;
        [XmlAnyElement] public XmlElement[] Any;
        [XmlAnyAttribute] public XmlAttribute[] AnyAttr;

        [XmlAttribute(AttributeName = "Type")]
        public string Type
        {
            get { return _Type; }
            set { _Type = value; }
        }

        [XmlAttribute(AttributeName = "PremiseDependency")]
        public string PremiseDependency
        {
            get { return _PremiseDependency; }
            set { _PremiseDependency = value; }
        }

        [XmlAttribute(AttributeName = "PremiseDependencyType")]
        public string PremiseDependencyType
        {
            get { return _PremiseDependencyType; }
            set { _PremiseDependencyType = value; }
        }

        [XmlAttribute(AttributeName = "PremiseThoroughfareConnector")]
        public string PremiseThoroughfareConnector
        {
            get { return _PremiseThoroughfareConnector; }
            set { _PremiseThoroughfareConnector = value; }
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

        [XmlElement(Type = typeof (PremiseName), ElementName = "PremiseName", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<PremiseName> PremiseName
        {
            get
            {
                if (_PremiseName == null) _PremiseName = new List<PremiseName>();
                return _PremiseName;
            }
            set { _PremiseName = value; }
        }

        [XmlElement(Type = typeof (PremiseLocation), ElementName = "PremiseLocation", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public PremiseLocation PremiseLocation
        {
            get { return _PremiseLocation; }
            set { _PremiseLocation = value; }
        }

        [XmlElement(Type = typeof (PremiseNumber), ElementName = "PremiseNumber", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<PremiseNumber> PremiseNumber
        {
            get
            {
                if (_PremiseNumber == null) _PremiseNumber = new List<PremiseNumber>();
                return _PremiseNumber;
            }
            set { _PremiseNumber = value; }
        }

        [XmlElement(Type = typeof (PremiseNumberRange), ElementName = "PremiseNumberRange", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public PremiseNumberRange PremiseNumberRange
        {
            get { return _PremiseNumberRange; }
            set { _PremiseNumberRange = value; }
        }

        [XmlElement(Type = typeof (PremiseNumberPrefix), ElementName = "PremiseNumberPrefix", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<PremiseNumberPrefix> PremiseNumberPrefix
        {
            get
            {
                if (_PremiseNumberPrefix == null) _PremiseNumberPrefix = new List<PremiseNumberPrefix>();
                return _PremiseNumberPrefix;
            }
            set { _PremiseNumberPrefix = value; }
        }

        [XmlElement(Type = typeof (PremiseNumberSuffix), ElementName = "PremiseNumberSuffix", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<PremiseNumberSuffix> PremiseNumberSuffix
        {
            get
            {
                if (_PremiseNumberSuffix == null) _PremiseNumberSuffix = new List<PremiseNumberSuffix>();
                return _PremiseNumberSuffix;
            }
            set { _PremiseNumberSuffix = value; }
        }

        [XmlElement(Type = typeof (BuildingNameType), ElementName = "BuildingName", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<BuildingNameType> BuildingName
        {
            get
            {
                if (_BuildingName == null) _BuildingName = new List<BuildingNameType>();
                return _BuildingName;
            }
            set { _BuildingName = value; }
        }

        [XmlElement(Type = typeof (SubPremiseType), ElementName = "SubPremise", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<SubPremiseType> SubPremise
        {
            get
            {
                if (_SubPremise == null) _SubPremise = new List<SubPremiseType>();
                return _SubPremise;
            }
            set { _SubPremise = value; }
        }

        [XmlElement(Type = typeof (FirmType), ElementName = "Firm", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = Declarations.SchemaVersion)]
        public FirmType Firm
        {
            get { return _Firm; }
            set { _Firm = value; }
        }

        [XmlElement(Type = typeof (MailStopType), ElementName = "MailStop", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public MailStopType MailStop
        {
            get { return _MailStop; }
            set { _MailStop = value; }
        }

        [XmlElement(Type = typeof (PostalCode), ElementName = "PostalCode", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public PostalCode PostalCode
        {
            get { return _PostalCode; }
            set { _PostalCode = value; }
        }

        [XmlElement(Type = typeof (Premise), ElementName = "Premise", IsNullable = false, Form = XmlSchemaForm.Qualified
            , Namespace = Declarations.SchemaVersion)]
        public Premise Premise1
        {
            get { return _Premise; }
            set { _Premise = value; }
        }

        public void MakeSchemaCompliant()
        {
            PremiseLocation.MakeSchemaCompliant();
            foreach (PremiseNumber _c in PremiseNumber) _c.MakeSchemaCompliant();
            PremiseNumberRange.MakeSchemaCompliant();
        }
    }
}