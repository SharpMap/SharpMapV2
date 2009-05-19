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
    [Serializable, XmlType(TypeName = "MD_keywords_Type", Namespace = "http://www.isotc211.org/2005/gmd")]
    public class MD_keywords_Type : AbstractObjectType
    {
        [XmlIgnore] private List<CharacterStringPropertyType> _keyword;
        [XmlIgnore] private CI_citation_PropertyType _thesaurusName;
        [XmlIgnore] private MD_keywordTypeCode_PropertyType _type;

        [XmlElement(Type = typeof (CharacterStringPropertyType), ElementName = "keyword", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<CharacterStringPropertyType> Keyword
        {
            get
            {
                if (_keyword == null)
                {
                    _keyword = new List<CharacterStringPropertyType>();
                }
                return _keyword;
            }
            set { _keyword = value; }
        }

        [XmlElement(Type = typeof (CI_citation_PropertyType), ElementName = "thesaurusName", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public CI_citation_PropertyType ThesaurusName
        {
            get { return _thesaurusName; }
            set { _thesaurusName = value; }
        }

        [XmlElement(Type = typeof (MD_keywordTypeCode_PropertyType), ElementName = "type", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public MD_keywordTypeCode_PropertyType Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
            foreach (CharacterStringPropertyType _c in Keyword)
            {
                _c.MakeSchemaCompliant();
            }
        }
    }
}