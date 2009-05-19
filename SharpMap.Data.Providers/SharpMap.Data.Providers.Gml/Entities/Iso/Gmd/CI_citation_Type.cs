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
    [Serializable, XmlType(TypeName = "CI_citation_Type", Namespace = "http://www.isotc211.org/2005/gmd")]
    public class CI_citation_Type : AbstractObjectType
    {
        [XmlIgnore] private List<CharacterStringPropertyType> _alternateTitle;
        [XmlIgnore] private List<CI_responsibleParty_PropertyType> _citedResponsibleParty;
        [XmlIgnore] private CharacterStringPropertyType _collectiveTitle;
        [XmlIgnore] private List<CI_date_PropertyType> _date;
        [XmlIgnore] private CharacterStringPropertyType _edition;
        [XmlIgnore] private DatePropertyType _editionDate;
        [XmlIgnore] private List<MD_identifier_PropertyType> _identifier;
        [XmlIgnore] private CharacterStringPropertyType _iSBN;
        [XmlIgnore] private CharacterStringPropertyType _iSSN;
        [XmlIgnore] private CharacterStringPropertyType _otherCitationDetails;
        [XmlIgnore] private List<CI_presentationFormCode_PropertyType> _presentationForm;
        [XmlIgnore] private CI_series_PropertyType _series;
        [XmlIgnore] private CharacterStringPropertyType _title;

        [XmlElement(Type = typeof (CharacterStringPropertyType), ElementName = "alternateTitle", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<CharacterStringPropertyType> AlternateTitle
        {
            get
            {
                if (_alternateTitle == null)
                {
                    _alternateTitle = new List<CharacterStringPropertyType>();
                }
                return _alternateTitle;
            }
            set { _alternateTitle = value; }
        }

        [XmlElement(Type = typeof (CI_responsibleParty_PropertyType), ElementName = "citedResponsibleParty",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<CI_responsibleParty_PropertyType> CitedResponsibleParty
        {
            get
            {
                if (_citedResponsibleParty == null)
                {
                    _citedResponsibleParty = new List<CI_responsibleParty_PropertyType>();
                }
                return _citedResponsibleParty;
            }
            set { _citedResponsibleParty = value; }
        }

        [XmlElement(Type = typeof (CharacterStringPropertyType), ElementName = "collectiveTitle", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public CharacterStringPropertyType CollectiveTitle
        {
            get { return _collectiveTitle; }
            set { _collectiveTitle = value; }
        }

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

        [XmlElement(Type = typeof (CharacterStringPropertyType), ElementName = "edition", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public CharacterStringPropertyType Edition
        {
            get { return _edition; }
            set { _edition = value; }
        }

        [XmlElement(Type = typeof (DatePropertyType), ElementName = "editionDate", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public DatePropertyType EditionDate
        {
            get { return _editionDate; }
            set { _editionDate = value; }
        }

        [XmlElement(Type = typeof (MD_identifier_PropertyType), ElementName = "identifier", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<MD_identifier_PropertyType> Identifier
        {
            get
            {
                if (_identifier == null)
                {
                    _identifier = new List<MD_identifier_PropertyType>();
                }
                return _identifier;
            }
            set { _identifier = value; }
        }

        [XmlElement(Type = typeof (CharacterStringPropertyType), ElementName = "ISBN", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public CharacterStringPropertyType ISBN
        {
            get { return _iSBN; }
            set { _iSBN = value; }
        }

        [XmlElement(Type = typeof (CharacterStringPropertyType), ElementName = "ISSN", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public CharacterStringPropertyType ISSN
        {
            get { return _iSSN; }
            set { _iSSN = value; }
        }

        [XmlElement(Type = typeof (CharacterStringPropertyType), ElementName = "otherCitationDetails",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public CharacterStringPropertyType OtherCitationDetails
        {
            get { return _otherCitationDetails; }
            set { _otherCitationDetails = value; }
        }

        [XmlElement(Type = typeof (CI_presentationFormCode_PropertyType), ElementName = "presentationForm",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<CI_presentationFormCode_PropertyType> PresentationForm
        {
            get
            {
                if (_presentationForm == null)
                {
                    _presentationForm = new List<CI_presentationFormCode_PropertyType>();
                }
                return _presentationForm;
            }
            set { _presentationForm = value; }
        }

        [XmlElement(Type = typeof (CI_series_PropertyType), ElementName = "series", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public CI_series_PropertyType Series
        {
            get { return _series; }
            set { _series = value; }
        }

        [XmlElement(Type = typeof (CharacterStringPropertyType), ElementName = "title", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public CharacterStringPropertyType Title
        {
            get { return _title; }
            set { _title = value; }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
            Title.MakeSchemaCompliant();
            foreach (CI_date_PropertyType _c in Date)
            {
                _c.MakeSchemaCompliant();
            }
        }
    }
}