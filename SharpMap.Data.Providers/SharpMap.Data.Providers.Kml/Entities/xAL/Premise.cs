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
        [XmlIgnore] private List<AddressLine> __AddressLine;
        [XmlIgnore] private List<BuildingNameType> __BuildingName;
        [XmlIgnore] private FirmType __Firm;
        [XmlIgnore] private MailStopType __MailStop;
        [XmlIgnore] private PostalCode __PostalCode;
        [XmlIgnore] private Premise __Premise;
        [XmlIgnore] private string __PremiseDependency;
        [XmlIgnore] private string __PremiseDependencyType;
        [XmlIgnore] private PremiseLocation __PremiseLocation;
        [XmlIgnore] private List<PremiseName> __PremiseName;
        [XmlIgnore] private List<PremiseNumber> __PremiseNumber;
        [XmlIgnore] private List<PremiseNumberPrefix> __PremiseNumberPrefix;
        [XmlIgnore] private PremiseNumberRange __PremiseNumberRange;
        [XmlIgnore] private List<PremiseNumberSuffix> __PremiseNumberSuffix;
        [XmlIgnore] private string __PremiseThoroughfareConnector;
        [XmlIgnore] private List<SubPremiseType> __SubPremise;
        [XmlIgnore] private string __Type;
        [XmlAnyElement] public XmlElement[] Any;
        [XmlAnyAttribute] public XmlAttribute[] AnyAttr;

        [XmlAttribute(AttributeName = "Type")]
        public string Type
        {
            get { return __Type; }
            set { __Type = value; }
        }

        [XmlAttribute(AttributeName = "PremiseDependency")]
        public string PremiseDependency
        {
            get { return __PremiseDependency; }
            set { __PremiseDependency = value; }
        }

        [XmlAttribute(AttributeName = "PremiseDependencyType")]
        public string PremiseDependencyType
        {
            get { return __PremiseDependencyType; }
            set { __PremiseDependencyType = value; }
        }

        [XmlAttribute(AttributeName = "PremiseThoroughfareConnector")]
        public string PremiseThoroughfareConnector
        {
            get { return __PremiseThoroughfareConnector; }
            set { __PremiseThoroughfareConnector = value; }
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

        [XmlElement(Type = typeof (PremiseName), ElementName = "PremiseName", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<PremiseName> PremiseName
        {
            get
            {
                if (__PremiseName == null) __PremiseName = new List<PremiseName>();
                return __PremiseName;
            }
            set { __PremiseName = value; }
        }

        [XmlElement(Type = typeof (PremiseLocation), ElementName = "PremiseLocation", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public PremiseLocation PremiseLocation
        {
            get
            {
                
                return __PremiseLocation;
            }
            set { __PremiseLocation = value; }
        }

        [XmlElement(Type = typeof (PremiseNumber), ElementName = "PremiseNumber", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<PremiseNumber> PremiseNumber
        {
            get
            {
                if (__PremiseNumber == null) __PremiseNumber = new List<PremiseNumber>();
                return __PremiseNumber;
            }
            set { __PremiseNumber = value; }
        }

        [XmlElement(Type = typeof (PremiseNumberRange), ElementName = "PremiseNumberRange", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public PremiseNumberRange PremiseNumberRange
        {
            get
            {
                
                return __PremiseNumberRange;
            }
            set { __PremiseNumberRange = value; }
        }

        [XmlElement(Type = typeof (PremiseNumberPrefix), ElementName = "PremiseNumberPrefix", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<PremiseNumberPrefix> PremiseNumberPrefix
        {
            get
            {
                if (__PremiseNumberPrefix == null) __PremiseNumberPrefix = new List<PremiseNumberPrefix>();
                return __PremiseNumberPrefix;
            }
            set { __PremiseNumberPrefix = value; }
        }

        [XmlElement(Type = typeof (PremiseNumberSuffix), ElementName = "PremiseNumberSuffix", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<PremiseNumberSuffix> PremiseNumberSuffix
        {
            get
            {
                if (__PremiseNumberSuffix == null) __PremiseNumberSuffix = new List<PremiseNumberSuffix>();
                return __PremiseNumberSuffix;
            }
            set { __PremiseNumberSuffix = value; }
        }

        [XmlElement(Type = typeof (BuildingNameType), ElementName = "BuildingName", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<BuildingNameType> BuildingName
        {
            get
            {
                if (__BuildingName == null) __BuildingName = new List<BuildingNameType>();
                return __BuildingName;
            }
            set { __BuildingName = value; }
        }

        [XmlElement(Type = typeof (SubPremiseType), ElementName = "SubPremise", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<SubPremiseType> SubPremise
        {
            get
            {
                if (__SubPremise == null) __SubPremise = new List<SubPremiseType>();
                return __SubPremise;
            }
            set { __SubPremise = value; }
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

        [XmlElement(Type = typeof (MailStopType), ElementName = "MailStop", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public MailStopType MailStop
        {
            get
            {
                
                return __MailStop;
            }
            set { __MailStop = value; }
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

        [XmlElement(Type = typeof (Premise), ElementName = "Premise", IsNullable = false, Form = XmlSchemaForm.Qualified
            , Namespace = Declarations.SchemaVersion)]
        public Premise Premise1
        {
            get
            {
                
                return __Premise;
            }
            set { __Premise = value; }
        }

        public void MakeSchemaCompliant()
        {
            PremiseLocation.MakeSchemaCompliant();
            foreach (PremiseNumber _c in PremiseNumber) _c.MakeSchemaCompliant();
            PremiseNumberRange.MakeSchemaCompliant();
        }
    }
}