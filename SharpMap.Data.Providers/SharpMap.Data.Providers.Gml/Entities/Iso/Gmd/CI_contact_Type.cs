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
using System.Xml.Schema;
using System.Xml.Serialization;
using SharpMap.Entities.Iso.Gco;

namespace SharpMap.Entities.Iso.Gmd
{
    [Serializable, XmlType(TypeName = "CI_contact_Type", Namespace = "http://www.isotc211.org/2005/gmd")]
    public class CI_contact_Type : AbstractObjectType
    {
        [XmlIgnore] private CI_address_PropertyType _address;
        [XmlIgnore] private CharacterStringPropertyType _contactInstructions;
        [XmlIgnore] private CharacterStringPropertyType _hoursOfService;
        [XmlIgnore] private CI_onlineResource_PropertyType _onlineResource;
        [XmlIgnore] private CI_telephone_PropertyType _phone;

        [XmlElement(Type = typeof (CI_address_PropertyType), ElementName = "address", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public CI_address_PropertyType Address
        {
            get { return _address; }
            set { _address = value; }
        }

        [XmlElement(Type = typeof (CharacterStringPropertyType), ElementName = "contactInstructions", IsNullable = false
            , Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public CharacterStringPropertyType ContactInstructions
        {
            get { return _contactInstructions; }
            set { _contactInstructions = value; }
        }

        [XmlElement(Type = typeof (CharacterStringPropertyType), ElementName = "hoursOfService", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public CharacterStringPropertyType HoursOfService
        {
            get { return _hoursOfService; }
            set { _hoursOfService = value; }
        }

        [XmlElement(Type = typeof (CI_onlineResource_PropertyType), ElementName = "onlineResource", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public CI_onlineResource_PropertyType OnlineResource
        {
            get { return _onlineResource; }
            set { _onlineResource = value; }
        }

        [XmlElement(Type = typeof (CI_telephone_PropertyType), ElementName = "phone", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public CI_telephone_PropertyType Phone
        {
            get { return _phone; }
            set { _phone = value; }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }
    }
}