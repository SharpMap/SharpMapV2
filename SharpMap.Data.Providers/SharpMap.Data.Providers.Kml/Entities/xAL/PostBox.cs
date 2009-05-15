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
        [XmlIgnore] private List<AddressLine> _AddressLine;
        [XmlIgnore] private FirmType _Firm;
        [XmlIgnore] private string _Indicator;
        [XmlIgnore] private PostalCode _PostalCode;
        [XmlIgnore] private PostBoxNumber _PostBoxNumber;
        [XmlIgnore] private PostBoxNumberExtension _PostBoxNumberExtension;
        [XmlIgnore] private PostBoxNumberPrefix _PostBoxNumberPrefix;
        [XmlIgnore] private PostBoxNumberSuffix _PostBoxNumberSuffix;
        [XmlIgnore] private string _Type;
        [XmlAnyElement] public XmlElement[] Any;
        [XmlAnyAttribute] public XmlAttribute[] AnyAttr;

        [XmlAttribute(AttributeName = "Type")]
        public string Type
        {
            get { return _Type; }
            set { _Type = value; }
        }

        [XmlAttribute(AttributeName = "Indicator")]
        public string Indicator
        {
            get { return _Indicator; }
            set { _Indicator = value; }
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

        [XmlElement(Type = typeof (PostBoxNumber), ElementName = "PostBoxNumber", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public PostBoxNumber PostBoxNumber
        {
            get { return _PostBoxNumber; }
            set { _PostBoxNumber = value; }
        }

        [XmlElement(Type = typeof (PostBoxNumberPrefix), ElementName = "PostBoxNumberPrefix", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public PostBoxNumberPrefix PostBoxNumberPrefix
        {
            get { return _PostBoxNumberPrefix; }
            set { _PostBoxNumberPrefix = value; }
        }

        [XmlElement(Type = typeof (PostBoxNumberSuffix), ElementName = "PostBoxNumberSuffix", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public PostBoxNumberSuffix PostBoxNumberSuffix
        {
            get { return _PostBoxNumberSuffix; }
            set { _PostBoxNumberSuffix = value; }
        }

        [XmlElement(Type = typeof (PostBoxNumberExtension), ElementName = "PostBoxNumberExtension", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public PostBoxNumberExtension PostBoxNumberExtension
        {
            get { return _PostBoxNumberExtension; }
            set { _PostBoxNumberExtension = value; }
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
            PostBoxNumber.MakeSchemaCompliant();
        }
    }
}