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
    [Serializable, XmlType(TypeName = "PT_locale_Type", Namespace = "http://www.isotc211.org/2005/gmd")]
    public class PT_locale_Type : AbstractObjectType
    {
        [XmlIgnore] private MD_characterSetCode_PropertyType _characterEncoding;
        [XmlIgnore] private CountryPropertyType _country;
        [XmlIgnore] private LanguageCodePropertyType _languageCode;

        [XmlElement(Type = typeof (MD_characterSetCode_PropertyType), ElementName = "characterEncoding",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public MD_characterSetCode_PropertyType CharacterEncoding
        {
            get { return _characterEncoding; }
            set { _characterEncoding = value; }
        }

        [XmlElement(Type = typeof (CountryPropertyType), ElementName = "country", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public CountryPropertyType Country
        {
            get { return _country; }
            set { _country = value; }
        }

        [XmlElement(Type = typeof (LanguageCodePropertyType), ElementName = "languageCode", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public LanguageCodePropertyType LanguageCode
        {
            get { return _languageCode; }
            set { _languageCode = value; }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
            LanguageCode.MakeSchemaCompliant();
            CharacterEncoding.MakeSchemaCompliant();
        }
    }
}