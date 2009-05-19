// /*
//  *  The attached / following is part of SharpMap.Data.Providers.Gml
//  *  SharpMap.Data.Providers.Gml is free software © 2008 Newgrove Consultants Limited, 
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
using System.Xml.Schema;
using System.Xml.Serialization;
using SharpMap.Entities.Iso.Gco;

namespace SharpMap.Entities.Iso.Gmd
{
    [Serializable, XmlType(TypeName = "CI_address_Type", Namespace = "http://www.isotc211.org/2005/gmd")]
    public class CI_address_Type : AbstractObjectType
    {
        [XmlIgnore] private CharacterStringPropertyType _administrativeArea;
        [XmlIgnore] private CharacterStringPropertyType _city;
        [XmlIgnore] private CharacterStringPropertyType _country;
        [XmlIgnore] private List<CharacterStringPropertyType> _deliveryPoint;
        [XmlIgnore] private List<CharacterStringPropertyType> _electronicMailAddress;
        [XmlIgnore] private CharacterStringPropertyType _postalCode;

        [XmlElement(Type = typeof (CharacterStringPropertyType), ElementName = "administrativeArea", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public CharacterStringPropertyType AdministrativeArea
        {
            get { return _administrativeArea; }
            set { _administrativeArea = value; }
        }

        [XmlElement(Type = typeof (CharacterStringPropertyType), ElementName = "city", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public CharacterStringPropertyType City
        {
            get { return _city; }
            set { _city = value; }
        }

        [XmlElement(Type = typeof (CharacterStringPropertyType), ElementName = "country", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public CharacterStringPropertyType Country
        {
            get { return _country; }
            set { _country = value; }
        }

        [XmlElement(Type = typeof (CharacterStringPropertyType), ElementName = "deliveryPoint", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<CharacterStringPropertyType> DeliveryPoint
        {
            get
            {
                if (_deliveryPoint == null)
                {
                    _deliveryPoint = new List<CharacterStringPropertyType>();
                }
                return _deliveryPoint;
            }
            set { _deliveryPoint = value; }
        }

        [XmlElement(Type = typeof (CharacterStringPropertyType), ElementName = "electronicMailAddress",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<CharacterStringPropertyType> ElectronicMailAddress
        {
            get
            {
                if (_electronicMailAddress == null)
                {
                    _electronicMailAddress = new List<CharacterStringPropertyType>();
                }
                return _electronicMailAddress;
            }
            set { _electronicMailAddress = value; }
        }

        [XmlElement(Type = typeof (CharacterStringPropertyType), ElementName = "postalCode", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public CharacterStringPropertyType PostalCode
        {
            get { return _postalCode; }
            set { _postalCode = value; }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }
    }
}