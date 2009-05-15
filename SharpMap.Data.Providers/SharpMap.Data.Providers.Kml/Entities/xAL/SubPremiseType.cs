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
        [XmlIgnore] private List<AddressLine> _AddressLine;
        [XmlIgnore] private List<BuildingNameType> _BuildingName;
        [XmlIgnore] private FirmType _Firm;
        [XmlIgnore] private MailStopType _MailStop;
        [XmlIgnore] private PostalCode _PostalCode;
        [XmlIgnore] private SubPremiseType _SubPremise;
        [XmlIgnore] private SubPremiseLocation _SubPremiseLocation;
        [XmlIgnore] private List<SubPremiseName> _SubPremiseName;
        [XmlIgnore] private List<SubPremiseNumber> _SubPremiseNumber;
        [XmlIgnore] private List<SubPremiseNumberPrefix> _SubPremiseNumberPrefix;
        [XmlIgnore] private List<SubPremiseNumberSuffix> _SubPremiseNumberSuffix;
        [XmlIgnore] private string _Type;
        [XmlAnyElement] public XmlElement[] Any;

        [XmlAnyAttribute] public XmlAttribute[] AnyAttr;

        [XmlAttribute(AttributeName = "Type")]
        public string Type
        {
            get { return _Type; }
            set { _Type = value; }
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

        [XmlElement(Type = typeof (SubPremiseName), ElementName = "SubPremiseName", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<SubPremiseName> SubPremiseName
        {
            get
            {
                if (_SubPremiseName == null) _SubPremiseName = new List<SubPremiseName>();
                return _SubPremiseName;
            }
            set { _SubPremiseName = value; }
        }

        [XmlElement(Type = typeof (SubPremiseLocation), ElementName = "SubPremiseLocation", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public SubPremiseLocation SubPremiseLocation
        {
            get { return _SubPremiseLocation; }
            set { _SubPremiseLocation = value; }
        }

        [XmlElement(Type = typeof (SubPremiseNumber), ElementName = "SubPremiseNumber", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<SubPremiseNumber> SubPremiseNumber
        {
            get
            {
                if (_SubPremiseNumber == null) _SubPremiseNumber = new List<SubPremiseNumber>();
                return _SubPremiseNumber;
            }
            set { _SubPremiseNumber = value; }
        }

        [XmlElement(Type = typeof (SubPremiseNumberPrefix), ElementName = "SubPremiseNumberPrefix", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<SubPremiseNumberPrefix> SubPremiseNumberPrefix
        {
            get
            {
                if (_SubPremiseNumberPrefix == null) _SubPremiseNumberPrefix = new List<SubPremiseNumberPrefix>();
                return _SubPremiseNumberPrefix;
            }
            set { _SubPremiseNumberPrefix = value; }
        }

        [XmlElement(Type = typeof (SubPremiseNumberSuffix), ElementName = "SubPremiseNumberSuffix", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<SubPremiseNumberSuffix> SubPremiseNumberSuffix
        {
            get
            {
                if (_SubPremiseNumberSuffix == null) _SubPremiseNumberSuffix = new List<SubPremiseNumberSuffix>();
                return _SubPremiseNumberSuffix;
            }
            set { _SubPremiseNumberSuffix = value; }
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

        [XmlElement(Type = typeof (SubPremiseType), ElementName = "SubPremise", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public SubPremiseType SubPremise
        {
            get { return _SubPremise; }
            set { _SubPremise = value; }
        }

        public void MakeSchemaCompliant()
        {
            SubPremiseLocation.MakeSchemaCompliant();
        }
    }
}