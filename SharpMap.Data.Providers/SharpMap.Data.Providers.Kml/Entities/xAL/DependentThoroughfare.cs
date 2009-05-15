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
    [XmlType(TypeName = "DependentThoroughfare", Namespace = Declarations.SchemaVersion), Serializable]
    public class DependentThoroughfare
    {
        [XmlIgnore] private List<AddressLine> _AddressLine;
        [XmlIgnore] private ThoroughfareLeadingTypeType _ThoroughfareLeadingType;
        [XmlIgnore] private List<ThoroughfareNameType> _ThoroughfareName;
        [XmlIgnore] private ThoroughfarePostDirectionType _ThoroughfarePostDirection;
        [XmlIgnore] private ThoroughfarePreDirectionType _ThoroughfarePreDirection;
        [XmlIgnore] private ThoroughfareTrailingTypeType _ThoroughfareTrailingType;
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

        [XmlElement(Type = typeof (ThoroughfarePreDirectionType), ElementName = "ThoroughfarePreDirection",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public ThoroughfarePreDirectionType ThoroughfarePreDirection
        {
            get { return _ThoroughfarePreDirection; }
            set { _ThoroughfarePreDirection = value; }
        }

        [XmlElement(Type = typeof (ThoroughfareLeadingTypeType), ElementName = "ThoroughfareLeadingType",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public ThoroughfareLeadingTypeType ThoroughfareLeadingType
        {
            get { return _ThoroughfareLeadingType; }
            set { _ThoroughfareLeadingType = value; }
        }

        [XmlElement(Type = typeof (ThoroughfareNameType), ElementName = "ThoroughfareName", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<ThoroughfareNameType> ThoroughfareName
        {
            get
            {
                if (_ThoroughfareName == null) _ThoroughfareName = new List<ThoroughfareNameType>();
                return _ThoroughfareName;
            }
            set { _ThoroughfareName = value; }
        }

        [XmlElement(Type = typeof (ThoroughfareTrailingTypeType), ElementName = "ThoroughfareTrailingType",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public ThoroughfareTrailingTypeType ThoroughfareTrailingType
        {
            get { return _ThoroughfareTrailingType; }
            set { _ThoroughfareTrailingType = value; }
        }

        [XmlElement(Type = typeof (ThoroughfarePostDirectionType), ElementName = "ThoroughfarePostDirection",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public ThoroughfarePostDirectionType ThoroughfarePostDirection
        {
            get
            {
                if (_ThoroughfarePostDirection == null)
                    _ThoroughfarePostDirection = new ThoroughfarePostDirectionType();
                return _ThoroughfarePostDirection;
            }
            set { _ThoroughfarePostDirection = value; }
        }

        public void MakeSchemaCompliant()
        {
        }
    }
}