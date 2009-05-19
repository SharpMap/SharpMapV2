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
    [Serializable,
     XmlType(TypeName = "MD_metadataExtensionInformation_Type", Namespace = "http://www.isotc211.org/2005/gmd")]
    public class MD_metadataExtensionInformation_Type : AbstractObjectType
    {
        [XmlIgnore] private List<MD_extendedElementInformation_PropertyType> _extendedElementInformation;
        [XmlIgnore] private CI_onlineResource_PropertyType _extensionOnLineResource;

        [XmlElement(Type = typeof (MD_extendedElementInformation_PropertyType),
            ElementName = "extendedElementInformation", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<MD_extendedElementInformation_PropertyType> ExtendedElementInformation
        {
            get
            {
                if (_extendedElementInformation == null)
                {
                    _extendedElementInformation = new List<MD_extendedElementInformation_PropertyType>();
                }
                return _extendedElementInformation;
            }
            set { _extendedElementInformation = value; }
        }

        [XmlElement(Type = typeof (CI_onlineResource_PropertyType), ElementName = "extensionOnLineResource",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public CI_onlineResource_PropertyType ExtensionOnLineResource
        {
            get { return _extensionOnLineResource; }
            set { _extensionOnLineResource = value; }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }
    }
}