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
    [Serializable, XmlType(TypeName = "MD_usage_Type", Namespace = "http://www.isotc211.org/2005/gmd")]
    public class MD_usage_Type : AbstractObjectType
    {
        [XmlIgnore] private CharacterStringPropertyType _specificUsage;
        [XmlIgnore] private DateTimePropertyType _usageDateTime;
        [XmlIgnore] private List<CI_responsibleParty_PropertyType> _userContactInfo;
        [XmlIgnore] private CharacterStringPropertyType _userDeterminedLimitations;

        [XmlElement(Type = typeof (CharacterStringPropertyType), ElementName = "specificUsage", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public CharacterStringPropertyType SpecificUsage
        {
            get { return _specificUsage; }
            set { _specificUsage = value; }
        }

        [XmlElement(Type = typeof (DateTimePropertyType), ElementName = "usageDateTime", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public DateTimePropertyType UsageDateTime
        {
            get { return _usageDateTime; }
            set { _usageDateTime = value; }
        }

        [XmlElement(Type = typeof (CI_responsibleParty_PropertyType), ElementName = "userContactInfo",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<CI_responsibleParty_PropertyType> UserContactInfo
        {
            get
            {
                if (_userContactInfo == null)
                {
                    _userContactInfo = new List<CI_responsibleParty_PropertyType>();
                }
                return _userContactInfo;
            }
            set { _userContactInfo = value; }
        }

        [XmlElement(Type = typeof (CharacterStringPropertyType), ElementName = "userDeterminedLimitations",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public CharacterStringPropertyType UserDeterminedLimitations
        {
            get { return _userDeterminedLimitations; }
            set { _userDeterminedLimitations = value; }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
            SpecificUsage.MakeSchemaCompliant();
            foreach (CI_responsibleParty_PropertyType _c in UserContactInfo)
            {
                _c.MakeSchemaCompliant();
            }
        }
    }
}