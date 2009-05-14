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
    [XmlType(TypeName = "SubPremiseType", Namespace = Declarations.SchemaVersion), Serializable]
    public class SubPremiseType
    {
        [XmlIgnore] private List<AddressLine> __AddressLine;
        [XmlIgnore] private List<BuildingNameType> __BuildingName;
        [XmlIgnore] private FirmType __Firm;
        [XmlIgnore] private MailStopType __MailStop;
        [XmlIgnore] private PostalCode __PostalCode;
        [XmlIgnore] private SubPremiseType __SubPremise;
        [XmlIgnore] private SubPremiseLocation __SubPremiseLocation;
        [XmlIgnore] private List<SubPremiseName> __SubPremiseName;
        [XmlIgnore] private List<SubPremiseNumber> __SubPremiseNumber;
        [XmlIgnore] private List<SubPremiseNumberPrefix> __SubPremiseNumberPrefix;
        [XmlIgnore] private List<SubPremiseNumberSuffix> __SubPremiseNumberSuffix;
        [XmlIgnore] private string __Type;
        [XmlAnyElement] public XmlElement[] Any;

        [XmlAnyAttribute] public XmlAttribute[] AnyAttr;

        [XmlAttribute(AttributeName = "Type")]
        public string Type
        {
            get { return __Type; }
            set { __Type = value; }
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

        [XmlElement(Type = typeof (SubPremiseName), ElementName = "SubPremiseName", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<SubPremiseName> SubPremiseName
        {
            get
            {
                if (__SubPremiseName == null) __SubPremiseName = new List<SubPremiseName>();
                return __SubPremiseName;
            }
            set { __SubPremiseName = value; }
        }

        [XmlElement(Type = typeof (SubPremiseLocation), ElementName = "SubPremiseLocation", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public SubPremiseLocation SubPremiseLocation
        {
            get
            {
                if (__SubPremiseLocation == null) __SubPremiseLocation = new SubPremiseLocation();
                return __SubPremiseLocation;
            }
            set { __SubPremiseLocation = value; }
        }

        [XmlElement(Type = typeof (SubPremiseNumber), ElementName = "SubPremiseNumber", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<SubPremiseNumber> SubPremiseNumber
        {
            get
            {
                if (__SubPremiseNumber == null) __SubPremiseNumber = new List<SubPremiseNumber>();
                return __SubPremiseNumber;
            }
            set { __SubPremiseNumber = value; }
        }

        [XmlElement(Type = typeof (SubPremiseNumberPrefix), ElementName = "SubPremiseNumberPrefix", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<SubPremiseNumberPrefix> SubPremiseNumberPrefix
        {
            get
            {
                if (__SubPremiseNumberPrefix == null) __SubPremiseNumberPrefix = new List<SubPremiseNumberPrefix>();
                return __SubPremiseNumberPrefix;
            }
            set { __SubPremiseNumberPrefix = value; }
        }

        [XmlElement(Type = typeof (SubPremiseNumberSuffix), ElementName = "SubPremiseNumberSuffix", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<SubPremiseNumberSuffix> SubPremiseNumberSuffix
        {
            get
            {
                if (__SubPremiseNumberSuffix == null) __SubPremiseNumberSuffix = new List<SubPremiseNumberSuffix>();
                return __SubPremiseNumberSuffix;
            }
            set { __SubPremiseNumberSuffix = value; }
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

        [XmlElement(Type = typeof (FirmType), ElementName = "Firm", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = Declarations.SchemaVersion)]
        public FirmType Firm
        {
            get
            {
                if (__Firm == null) __Firm = new FirmType();
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
                if (__MailStop == null) __MailStop = new MailStopType();
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
                if (__PostalCode == null) __PostalCode = new PostalCode();
                return __PostalCode;
            }
            set { __PostalCode = value; }
        }

        [XmlElement(Type = typeof (SubPremiseType), ElementName = "SubPremise", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public SubPremiseType SubPremise
        {
            get
            {
                if (__SubPremise == null) __SubPremise = new SubPremiseType();
                return __SubPremise;
            }
            set { __SubPremise = value; }
        }

        public void MakeSchemaCompliant()
        {
            SubPremiseLocation.MakeSchemaCompliant();
        }
    }
}