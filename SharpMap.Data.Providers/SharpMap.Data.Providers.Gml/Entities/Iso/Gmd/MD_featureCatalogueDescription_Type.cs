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
     XmlType(TypeName = "MD_featureCatalogueDescription_Type", Namespace = "http://www.isotc211.org/2005/gmd")]
    public class MD_featureCatalogueDescription_Type : AbstractMD_contentInformation_Type
    {
        [XmlIgnore] private BooleanPropertyType _complianceCode;
        [XmlIgnore] private List<CI_citation_PropertyType> _featureCatalogueCitation;
        [XmlIgnore] private List<GenericNamePropertyType> _featureTypes;
        [XmlIgnore] private BooleanPropertyType _includedWithDataset;
        [XmlIgnore] private List<CharacterStringPropertyType> _language;

        [XmlElement(Type = typeof (BooleanPropertyType), ElementName = "complianceCode", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public BooleanPropertyType ComplianceCode
        {
            get { return _complianceCode; }
            set { _complianceCode = value; }
        }

        [XmlElement(Type = typeof (CI_citation_PropertyType), ElementName = "featureCatalogueCitation",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<CI_citation_PropertyType> FeatureCatalogueCitation
        {
            get
            {
                if (_featureCatalogueCitation == null)
                {
                    _featureCatalogueCitation = new List<CI_citation_PropertyType>();
                }
                return _featureCatalogueCitation;
            }
            set { _featureCatalogueCitation = value; }
        }

        [XmlElement(Type = typeof (GenericNamePropertyType), ElementName = "featureTypes", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<GenericNamePropertyType> FeatureTypes
        {
            get
            {
                if (_featureTypes == null)
                {
                    _featureTypes = new List<GenericNamePropertyType>();
                }
                return _featureTypes;
            }
            set { _featureTypes = value; }
        }

        [XmlElement(Type = typeof (BooleanPropertyType), ElementName = "includedWithDataset", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public BooleanPropertyType IncludedWithDataset
        {
            get { return _includedWithDataset; }
            set { _includedWithDataset = value; }
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

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
            IncludedWithDataset.MakeSchemaCompliant();
            foreach (CI_citation_PropertyType _c in FeatureCatalogueCitation)
            {
                _c.MakeSchemaCompliant();
            }
        }
    }
}