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
    [XmlRoot(ElementName = "PostBox", Namespace = Declarations.SchemaVersion, IsNullable = false), Serializable]
    public class PostBox
    {
        [XmlIgnore] private List<AddressLine> __AddressLine;
        [XmlIgnore] private FirmType __Firm;
        [XmlIgnore] private string __Indicator;
        [XmlIgnore] private PostalCode __PostalCode;
        [XmlIgnore] private PostBoxNumber __PostBoxNumber;
        [XmlIgnore] private PostBoxNumberExtension __PostBoxNumberExtension;
        [XmlIgnore] private PostBoxNumberPrefix __PostBoxNumberPrefix;
        [XmlIgnore] private PostBoxNumberSuffix __PostBoxNumberSuffix;
        [XmlIgnore] private string __Type;
        [XmlAnyElement] public XmlElement[] Any;
        [XmlAnyAttribute] public XmlAttribute[] AnyAttr;

        [XmlAttribute(AttributeName = "Type")]
        public string Type
        {
            get { return __Type; }
            set { __Type = value; }
        }

        [XmlAttribute(AttributeName = "Indicator")]
        public string Indicator
        {
            get { return __Indicator; }
            set { __Indicator = value; }
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

        [XmlElement(Type = typeof (PostBoxNumber), ElementName = "PostBoxNumber", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public PostBoxNumber PostBoxNumber
        {
            get
            {
                if (__PostBoxNumber == null) __PostBoxNumber = new PostBoxNumber();
                return __PostBoxNumber;
            }
            set { __PostBoxNumber = value; }
        }

        [XmlElement(Type = typeof (PostBoxNumberPrefix), ElementName = "PostBoxNumberPrefix", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public PostBoxNumberPrefix PostBoxNumberPrefix
        {
            get
            {
                if (__PostBoxNumberPrefix == null) __PostBoxNumberPrefix = new PostBoxNumberPrefix();
                return __PostBoxNumberPrefix;
            }
            set { __PostBoxNumberPrefix = value; }
        }

        [XmlElement(Type = typeof (PostBoxNumberSuffix), ElementName = "PostBoxNumberSuffix", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public PostBoxNumberSuffix PostBoxNumberSuffix
        {
            get
            {
                if (__PostBoxNumberSuffix == null) __PostBoxNumberSuffix = new PostBoxNumberSuffix();
                return __PostBoxNumberSuffix;
            }
            set { __PostBoxNumberSuffix = value; }
        }

        [XmlElement(Type = typeof (PostBoxNumberExtension), ElementName = "PostBoxNumberExtension", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public PostBoxNumberExtension PostBoxNumberExtension
        {
            get
            {
                if (__PostBoxNumberExtension == null) __PostBoxNumberExtension = new PostBoxNumberExtension();
                return __PostBoxNumberExtension;
            }
            set { __PostBoxNumberExtension = value; }
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

        public void MakeSchemaCompliant()
        {
            PostBoxNumber.MakeSchemaCompliant();
        }
    }
}