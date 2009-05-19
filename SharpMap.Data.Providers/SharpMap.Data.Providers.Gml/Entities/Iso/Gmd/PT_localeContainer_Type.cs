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
    [Serializable, XmlType(TypeName = "PT_localeContainer_Type", Namespace = "http://www.isotc211.org/2005/gmd")]
    public class PT_localeContainer_Type
    {
        [XmlIgnore] private List<CI_date_PropertyType> _date;
        [XmlIgnore] private CharacterStringPropertyType _description;
        [XmlIgnore] private PT_locale_PropertyType _locale;
        [XmlIgnore] private List<LocalisedCharacterStringPropertyType> _localisedString;
        [XmlIgnore] private List<CI_responsibleParty_PropertyType> _responsibleParty;

        [XmlElement(Type = typeof (CI_date_PropertyType), ElementName = "date", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<CI_date_PropertyType> Date
        {
            get
            {
                if (_date == null)
                {
                    _date = new List<CI_date_PropertyType>();
                }
                return _date;
            }
            set { _date = value; }
        }

        [XmlElement(Type = typeof (CharacterStringPropertyType), ElementName = "description", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public CharacterStringPropertyType Description
        {
            get { return _description; }
            set { _description = value; }
        }

        [XmlElement(Type = typeof (PT_locale_PropertyType), ElementName = "locale", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public PT_locale_PropertyType Locale
        {
            get { return _locale; }
            set { _locale = value; }
        }

        [XmlElement(Type = typeof (LocalisedCharacterStringPropertyType), ElementName = "localisedString",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<LocalisedCharacterStringPropertyType> LocalisedString
        {
            get
            {
                if (_localisedString == null)
                {
                    _localisedString = new List<LocalisedCharacterStringPropertyType>();
                }
                return _localisedString;
            }
            set { _localisedString = value; }
        }

        [XmlElement(Type = typeof (CI_responsibleParty_PropertyType), ElementName = "responsibleParty",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<CI_responsibleParty_PropertyType> ResponsibleParty
        {
            get
            {
                if (_responsibleParty == null)
                {
                    _responsibleParty = new List<CI_responsibleParty_PropertyType>();
                }
                return _responsibleParty;
            }
            set { _responsibleParty = value; }
        }

        public virtual void MakeSchemaCompliant()
        {
            Description.MakeSchemaCompliant();
            Locale.MakeSchemaCompliant();
            foreach (CI_date_PropertyType _c in Date)
            {
                _c.MakeSchemaCompliant();
            }
            foreach (CI_responsibleParty_PropertyType _c in ResponsibleParty)
            {
                _c.MakeSchemaCompliant();
            }
            foreach (LocalisedCharacterStringPropertyType _c in LocalisedString)
            {
                _c.MakeSchemaCompliant();
            }
        }
    }
}