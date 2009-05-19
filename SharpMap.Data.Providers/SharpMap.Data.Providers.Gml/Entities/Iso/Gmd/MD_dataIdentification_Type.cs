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
    [Serializable, XmlType(TypeName = "MD_dataIdentification_Type", Namespace = "http://www.isotc211.org/2005/gmd")]
    public class MD_dataIdentification_Type : AbstractMD_identification_Type
    {
        [XmlIgnore] private List<MD_characterSetCode_PropertyType> _characterSet;
        [XmlIgnore] private CharacterStringPropertyType _environmentDescription;
        [XmlIgnore] private List<EX_extent_PropertyType> _extent;
        [XmlIgnore] private List<CharacterStringPropertyType> _language;
        [XmlIgnore] private List<MD_spatialRepresentationTypeCode_PropertyType> _spatialRepresentationType;
        [XmlIgnore] private List<MD_resolution_PropertyType> _spatialResolution;
        [XmlIgnore] private CharacterStringPropertyType _supplementalInformation;
        [XmlIgnore] private List<MD_topicCategoryCode_PropertyType> _topicCategory;

        [XmlElement(Type = typeof (MD_characterSetCode_PropertyType), ElementName = "characterSet", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<MD_characterSetCode_PropertyType> CharacterSet
        {
            get
            {
                if (_characterSet == null)
                {
                    _characterSet = new List<MD_characterSetCode_PropertyType>();
                }
                return _characterSet;
            }
            set { _characterSet = value; }
        }

        [XmlElement(Type = typeof (CharacterStringPropertyType), ElementName = "environmentDescription",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public CharacterStringPropertyType EnvironmentDescription
        {
            get { return _environmentDescription; }
            set { _environmentDescription = value; }
        }

        [XmlElement(Type = typeof (EX_extent_PropertyType), ElementName = "extent", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<EX_extent_PropertyType> Extent
        {
            get
            {
                if (_extent == null)
                {
                    _extent = new List<EX_extent_PropertyType>();
                }
                return _extent;
            }
            set { _extent = value; }
        }

        [XmlElement(Type = typeof (CharacterStringPropertyType), ElementName = "language", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<CharacterStringPropertyType> Language
        {
            get
            {
                if (_language == null)
                {
                    _language = new List<CharacterStringPropertyType>();
                }
                return _language;
            }
            set { _language = value; }
        }

        [XmlElement(Type = typeof (MD_spatialRepresentationTypeCode_PropertyType),
            ElementName = "spatialRepresentationType", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<MD_spatialRepresentationTypeCode_PropertyType> SpatialRepresentationType
        {
            get
            {
                if (_spatialRepresentationType == null)
                {
                    _spatialRepresentationType = new List<MD_spatialRepresentationTypeCode_PropertyType>();
                }
                return _spatialRepresentationType;
            }
            set { _spatialRepresentationType = value; }
        }

        [XmlElement(Type = typeof (MD_resolution_PropertyType), ElementName = "spatialResolution", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<MD_resolution_PropertyType> SpatialResolution
        {
            get
            {
                if (_spatialResolution == null)
                {
                    _spatialResolution = new List<MD_resolution_PropertyType>();
                }
                return _spatialResolution;
            }
            set { _spatialResolution = value; }
        }

        [XmlElement(Type = typeof (CharacterStringPropertyType), ElementName = "supplementalInformation",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public CharacterStringPropertyType SupplementalInformation
        {
            get { return _supplementalInformation; }
            set { _supplementalInformation = value; }
        }

        [XmlElement(Type = typeof (MD_topicCategoryCode_PropertyType), ElementName = "topicCategory", IsNullable = false
            , Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<MD_topicCategoryCode_PropertyType> TopicCategory
        {
            get
            {
                if (_topicCategory == null)
                {
                    _topicCategory = new List<MD_topicCategoryCode_PropertyType>();
                }
                return _topicCategory;
            }
            set { _topicCategory = value; }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
            foreach (CharacterStringPropertyType _c in Language)
            {
                _c.MakeSchemaCompliant();
            }
        }
    }
}