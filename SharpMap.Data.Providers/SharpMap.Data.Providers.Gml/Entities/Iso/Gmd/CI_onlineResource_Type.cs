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
    [Serializable, XmlType(TypeName = "CI_onlineResource_Type", Namespace = "http://www.isotc211.org/2005/gmd")]
    public class CI_onlineResource_Type : AbstractObjectType
    {
        [XmlIgnore] private CharacterStringPropertyType _applicationProfile;
        [XmlIgnore] private CharacterStringPropertyType _description;
        [XmlIgnore] private CI_onLineFunctionCode_PropertyType _function;
        [XmlIgnore] private URLPropertyType _linkage;
        [XmlIgnore] private CharacterStringPropertyType _name;
        [XmlIgnore] private CharacterStringPropertyType _protocol;

        [XmlElement(Type = typeof (CharacterStringPropertyType), ElementName = "applicationProfile", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public CharacterStringPropertyType ApplicationProfile
        {
            get { return _applicationProfile; }
            set { _applicationProfile = value; }
        }

        [XmlElement(Type = typeof (CharacterStringPropertyType), ElementName = "description", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public CharacterStringPropertyType Description
        {
            get { return _description; }
            set { _description = value; }
        }

        [XmlElement(Type = typeof (CI_onLineFunctionCode_PropertyType), ElementName = "function", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public CI_onLineFunctionCode_PropertyType Function
        {
            get { return _function; }
            set { _function = value; }
        }

        [XmlElement(Type = typeof (URLPropertyType), ElementName = "linkage", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public URLPropertyType Linkage
        {
            get { return _linkage; }
            set { _linkage = value; }
        }

        [XmlElement(Type = typeof (CharacterStringPropertyType), ElementName = "name", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public CharacterStringPropertyType Name
        {
            get { return _name; }
            set { _name = value; }
        }

        [XmlElement(Type = typeof (CharacterStringPropertyType), ElementName = "protocol", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public CharacterStringPropertyType Protocol
        {
            get { return _protocol; }
            set { _protocol = value; }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
            Linkage.MakeSchemaCompliant();
        }
    }
}