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
        [XmlIgnore] private List<AddressLine> __AddressLine;
        [XmlIgnore] private string __Code;
        [XmlIgnore] private List<ThoroughfareNumber> __ThoroughfareNumber;
        [XmlIgnore] private List<ThoroughfareNumberPrefix> __ThoroughfareNumberPrefix;
        [XmlIgnore] private List<ThoroughfareNumberSuffix> __ThoroughfareNumberSuffix;
        [XmlIgnore] private string __Value;

        [XmlAnyAttribute] public XmlAttribute[] AnyAttr;

        [XmlAttribute(AttributeName = "Code")]
        public string Code
        {
            get { return __Code; }
            set { __Code = value; }
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

        [XmlText(DataType = "string")]
        public string Value
        {
            get { return __Value; }
            set { __Value = value; }
        }

        public void MakeSchemaCompliant()
        {
            foreach (ThoroughfareNumber _c in ThoroughfareNumber) _c.MakeSchemaCompliant();
        }
    }
}