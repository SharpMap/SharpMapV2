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
    [Serializable, XmlType(TypeName = "MD_legalConstraints_Type", Namespace = "http://www.isotc211.org/2005/gmd")]
    public class MD_legalConstraints_Type : MD_constraints_Type
    {
        [XmlIgnore] private List<MD_restrictionCode_PropertyType> _accessConstraints;
        [XmlIgnore] private List<CharacterStringPropertyType> _otherConstraints;
        [XmlIgnore] private List<MD_restrictionCode_PropertyType> _useConstraints;

        [XmlElement(Type = typeof (MD_restrictionCode_PropertyType), ElementName = "accessConstraints",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<MD_restrictionCode_PropertyType> AccessConstraints
        {
            get
            {
                if (_accessConstraints == null)
                {
                    _accessConstraints = new List<MD_restrictionCode_PropertyType>();
                }
                return _accessConstraints;
            }
            set { _accessConstraints = value; }
        }

        [XmlElement(Type = typeof (CharacterStringPropertyType), ElementName = "otherConstraints", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<CharacterStringPropertyType> OtherConstraints
        {
            get
            {
                if (_otherConstraints == null)
                {
                    _otherConstraints = new List<CharacterStringPropertyType>();
                }
                return _otherConstraints;
            }
            set { _otherConstraints = value; }
        }

        [XmlElement(Type = typeof (MD_restrictionCode_PropertyType), ElementName = "useConstraints", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<MD_restrictionCode_PropertyType> UseConstraints
        {
            get
            {
                if (_useConstraints == null)
                {
                    _useConstraints = new List<MD_restrictionCode_PropertyType>();
                }
                return _useConstraints;
            }
            set { _useConstraints = value; }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }
    }
}