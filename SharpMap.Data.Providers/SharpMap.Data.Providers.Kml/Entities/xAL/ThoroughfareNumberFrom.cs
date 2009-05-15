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
    [XmlType(TypeName = "ThoroughfareNumberFrom", Namespace = Declarations.SchemaVersion), Serializable]
    public class ThoroughfareNumberFrom
    {
        [XmlIgnore] private List<AddressLine> _AddressLine;
        [XmlIgnore] private string _Code;
        [XmlIgnore] private List<ThoroughfareNumber> _ThoroughfareNumber;
        [XmlIgnore] private List<ThoroughfareNumberPrefix> _ThoroughfareNumberPrefix;
        [XmlIgnore] private List<ThoroughfareNumberSuffix> _ThoroughfareNumberSuffix;
        [XmlIgnore] private string _Value;

        [XmlAnyAttribute] public XmlAttribute[] AnyAttr;

        [XmlAttribute(AttributeName = "Code")]
        public string Code
        {
            get { return _Code; }
            set { _Code = value; }
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

        [XmlText(DataType = "string")]
        public string Value
        {
            get { return _Value; }
            set { _Value = value; }
        }

        public void MakeSchemaCompliant()
        {
            foreach (ThoroughfareNumber _c in ThoroughfareNumber) _c.MakeSchemaCompliant();
        }
    }
}