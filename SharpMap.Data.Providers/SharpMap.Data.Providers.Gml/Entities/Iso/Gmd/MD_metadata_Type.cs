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
    [Serializable, XmlType(TypeName = "MD_metadata_Type", Namespace = "http://www.isotc211.org/2005/gmd")]
    public class MD_metadata_Type : AbstractObjectType
    {
        [XmlIgnore] private List<MD_applicationSchemaInformation_PropertyType> _applicationSchemaInfo;
        [XmlIgnore] private MD_characterSetCode_PropertyType _characterSet;
        [XmlIgnore] private List<CI_responsibleParty_PropertyType> _contact;
        [XmlIgnore] private List<MD_contentInformation_PropertyType> _contentInfo;
        [XmlIgnore] private List<DQ_dataQuality_PropertyType> _dataQualityInfo;
        [XmlIgnore] private CharacterStringPropertyType _dataSetURI;
        [XmlIgnore] private DatePropertyType _dateStamp;
        [XmlIgnore] private List<DS_dataSet_PropertyType> _describes;
        [XmlIgnore] private MD_distribution_PropertyType _distributionInfo;
        [XmlIgnore] private List<ObjectReferencePropertyType> _featureAttribute;
        [XmlIgnore] private List<ObjectReferencePropertyType> _featureType;
        [XmlIgnore] private CharacterStringPropertyType _fileIdentifier;
        [XmlIgnore] private List<MD_scopeCode_PropertyType> _hierarchyLevel;
        [XmlIgnore] private List<CharacterStringPropertyType> _hierarchyLevelName;
        [XmlIgnore] private List<MD_identification_PropertyType> _identificationInfo;
        [XmlIgnore] private CharacterStringPropertyType _language;
        [XmlIgnore] private List<PT_locale_PropertyType> _locale;
        [XmlIgnore] private List<MD_constraints_PropertyType> _metadataConstraints;
        [XmlIgnore] private List<MD_metadataExtensionInformation_PropertyType> _metadataExtensionInfo;
        [XmlIgnore] private MD_maintenanceInformation_PropertyType _metadataMaintenance;
        [XmlIgnore] private CharacterStringPropertyType _metadataStandardName;
        [XmlIgnore] private CharacterStringPropertyType _metadataStandardVersion;
        [XmlIgnore] private CharacterStringPropertyType _parentIdentifier;
        [XmlIgnore] private List<MD_portrayalCatalogueReference_PropertyType> _portrayalCatalogueInfo;
        [XmlIgnore] private List<ObjectReferencePropertyType> _propertyType;
        [XmlIgnore] private List<MD_referenceSystem_PropertyType> _referenceSystemInfo;
        [XmlIgnore] private List<DS_aggregate_PropertyType> _series;
        [XmlIgnore] private List<MD_spatialRepresentation_PropertyType> _spatialRepresentationInfo;

        [XmlElement(Type = typeof (MD_applicationSchemaInformation_PropertyType), ElementName = "applicationSchemaInfo",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<MD_applicationSchemaInformation_PropertyType> ApplicationSchemaInfo
        {
            get
            {
                if (_applicationSchemaInfo == null)
                {
                    _applicationSchemaInfo = new List<MD_applicationSchemaInformation_PropertyType>();
                }
                return _applicationSchemaInfo;
            }
            set { _applicationSchemaInfo = value; }
        }

        [XmlElement(Type = typeof (MD_characterSetCode_PropertyType), ElementName = "characterSet", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public MD_characterSetCode_PropertyType CharacterSet
        {
            get { return _characterSet; }
            set { _characterSet = value; }
        }

        [XmlElement(Type = typeof (CI_responsibleParty_PropertyType), ElementName = "contact", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<CI_responsibleParty_PropertyType> Contact
        {
            get
            {
                if (_contact == null)
                {
                    _contact = new List<CI_responsibleParty_PropertyType>();
                }
                return _contact;
            }
            set { _contact = value; }
        }

        [XmlElement(Type = typeof (MD_contentInformation_PropertyType), ElementName = "contentInfo", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<MD_contentInformation_PropertyType> ContentInfo
        {
            get
            {
                if (_contentInfo == null)
                {
                    _contentInfo = new List<MD_contentInformation_PropertyType>();
                }
                return _contentInfo;
            }
            set { _contentInfo = value; }
        }

        [XmlElement(Type = typeof (DQ_dataQuality_PropertyType), ElementName = "dataQualityInfo", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<DQ_dataQuality_PropertyType> DataQualityInfo
        {
            get
            {
                if (_dataQualityInfo == null)
                {
                    _dataQualityInfo = new List<DQ_dataQuality_PropertyType>();
                }
                return _dataQualityInfo;
            }
            set { _dataQualityInfo = value; }
        }

        [XmlElement(Type = typeof (CharacterStringPropertyType), ElementName = "dataSetURI", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public CharacterStringPropertyType DataSetURI
        {
            get { return _dataSetURI; }
            set { _dataSetURI = value; }
        }

        [XmlElement(Type = typeof (DatePropertyType), ElementName = "dateStamp", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public DatePropertyType DateStamp
        {
            get { return _dateStamp; }
            set { _dateStamp = value; }
        }

        [XmlElement(Type = typeof (DS_dataSet_PropertyType), ElementName = "describes", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<DS_dataSet_PropertyType> Describes
        {
            get
            {
                if (_describes == null)
                {
                    _describes = new List<DS_dataSet_PropertyType>();
                }
                return _describes;
            }
            set { _describes = value; }
        }

        [XmlElement(Type = typeof (MD_distribution_PropertyType), ElementName = "distributionInfo", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public MD_distribution_PropertyType DistributionInfo
        {
            get { return _distributionInfo; }
            set { _distributionInfo = value; }
        }

        [XmlElement(Type = typeof (ObjectReferencePropertyType), ElementName = "featureAttribute", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<ObjectReferencePropertyType> FeatureAttribute
        {
            get
            {
                if (_featureAttribute == null)
                {
                    _featureAttribute = new List<ObjectReferencePropertyType>();
                }
                return _featureAttribute;
            }
            set { _featureAttribute = value; }
        }

        [XmlElement(Type = typeof (ObjectReferencePropertyType), ElementName = "featureType", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<ObjectReferencePropertyType> FeatureType
        {
            get
            {
                if (_featureType == null)
                {
                    _featureType = new List<ObjectReferencePropertyType>();
                }
                return _featureType;
            }
            set { _featureType = value; }
        }

        [XmlElement(Type = typeof (CharacterStringPropertyType), ElementName = "fileIdentifier", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public CharacterStringPropertyType FileIdentifier
        {
            get { return _fileIdentifier; }
            set { _fileIdentifier = value; }
        }

        [XmlElement(Type = typeof (MD_scopeCode_PropertyType), ElementName = "hierarchyLevel", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<MD_scopeCode_PropertyType> HierarchyLevel
        {
            get
            {
                if (_hierarchyLevel == null)
                {
                    _hierarchyLevel = new List<MD_scopeCode_PropertyType>();
                }
                return _hierarchyLevel;
            }
            set { _hierarchyLevel = value; }
        }

        [XmlElement(Type = typeof (CharacterStringPropertyType), ElementName = "hierarchyLevelName", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<CharacterStringPropertyType> HierarchyLevelName
        {
            get
            {
                if (_hierarchyLevelName == null)
                {
                    _hierarchyLevelName = new List<CharacterStringPropertyType>();
                }
                return _hierarchyLevelName;
            }
            set { _hierarchyLevelName = value; }
        }

        [XmlElement(Type = typeof (MD_identification_PropertyType), ElementName = "identificationInfo",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<MD_identification_PropertyType> IdentificationInfo
        {
            get
            {
                if (_identificationInfo == null)
                {
                    _identificationInfo = new List<MD_identification_PropertyType>();
                }
                return _identificationInfo;
            }
            set { _identificationInfo = value; }
        }

        [XmlElement(Type = typeof (CharacterStringPropertyType), ElementName = "language", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public CharacterStringPropertyType Language
        {
            get { return _language; }
            set { _language = value; }
        }

        [XmlElement(Type = typeof (PT_locale_PropertyType), ElementName = "locale", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<PT_locale_PropertyType> Locale
        {
            get
            {
                if (_locale == null)
                {
                    _locale = new List<PT_locale_PropertyType>();
                }
                return _locale;
            }
            set { _locale = value; }
        }

        [XmlElement(Type = typeof (MD_constraints_PropertyType), ElementName = "metadataConstraints", IsNullable = false
            , Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<MD_constraints_PropertyType> MetadataConstraints
        {
            get
            {
                if (_metadataConstraints == null)
                {
                    _metadataConstraints = new List<MD_constraints_PropertyType>();
                }
                return _metadataConstraints;
            }
            set { _metadataConstraints = value; }
        }

        [XmlElement(Type = typeof (MD_metadataExtensionInformation_PropertyType), ElementName = "metadataExtensionInfo",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<MD_metadataExtensionInformation_PropertyType> MetadataExtensionInfo
        {
            get
            {
                if (_metadataExtensionInfo == null)
                {
                    _metadataExtensionInfo = new List<MD_metadataExtensionInformation_PropertyType>();
                }
                return _metadataExtensionInfo;
            }
            set { _metadataExtensionInfo = value; }
        }

        [XmlElement(Type = typeof (MD_maintenanceInformation_PropertyType), ElementName = "metadataMaintenance",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public MD_maintenanceInformation_PropertyType MetadataMaintenance
        {
            get { return _metadataMaintenance; }
            set { _metadataMaintenance = value; }
        }

        [XmlElement(Type = typeof (CharacterStringPropertyType), ElementName = "metadataStandardName",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public CharacterStringPropertyType MetadataStandardName
        {
            get { return _metadataStandardName; }
            set { _metadataStandardName = value; }
        }

        [XmlElement(Type = typeof (CharacterStringPropertyType), ElementName = "metadataStandardVersion",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public CharacterStringPropertyType MetadataStandardVersion
        {
            get { return _metadataStandardVersion; }
            set { _metadataStandardVersion = value; }
        }

        [XmlElement(Type = typeof (CharacterStringPropertyType), ElementName = "parentIdentifier", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public CharacterStringPropertyType ParentIdentifier
        {
            get { return _parentIdentifier; }
            set { _parentIdentifier = value; }
        }

        [XmlElement(Type = typeof (MD_portrayalCatalogueReference_PropertyType), ElementName = "portrayalCatalogueInfo",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<MD_portrayalCatalogueReference_PropertyType> PortrayalCatalogueInfo
        {
            get
            {
                if (_portrayalCatalogueInfo == null)
                {
                    _portrayalCatalogueInfo = new List<MD_portrayalCatalogueReference_PropertyType>();
                }
                return _portrayalCatalogueInfo;
            }
            set { _portrayalCatalogueInfo = value; }
        }

        [XmlElement(Type = typeof (ObjectReferencePropertyType), ElementName = "propertyType", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<ObjectReferencePropertyType> PropertyType
        {
            get
            {
                if (_propertyType == null)
                {
                    _propertyType = new List<ObjectReferencePropertyType>();
                }
                return _propertyType;
            }
            set { _propertyType = value; }
        }

        [XmlElement(Type = typeof (MD_referenceSystem_PropertyType), ElementName = "referenceSystemInfo",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<MD_referenceSystem_PropertyType> ReferenceSystemInfo
        {
            get
            {
                if (_referenceSystemInfo == null)
                {
                    _referenceSystemInfo = new List<MD_referenceSystem_PropertyType>();
                }
                return _referenceSystemInfo;
            }
            set { _referenceSystemInfo = value; }
        }

        [XmlElement(Type = typeof (DS_aggregate_PropertyType), ElementName = "series", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<DS_aggregate_PropertyType> Series
        {
            get
            {
                if (_series == null)
                {
                    _series = new List<DS_aggregate_PropertyType>();
                }
                return _series;
            }
            set { _series = value; }
        }

        [XmlElement(Type = typeof (MD_spatialRepresentation_PropertyType), ElementName = "spatialRepresentationInfo",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<MD_spatialRepresentation_PropertyType> SpatialRepresentationInfo
        {
            get
            {
                if (_spatialRepresentationInfo == null)
                {
                    _spatialRepresentationInfo = new List<MD_spatialRepresentation_PropertyType>();
                }
                return _spatialRepresentationInfo;
            }
            set { _spatialRepresentationInfo = value; }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
            foreach (CI_responsibleParty_PropertyType _c in Contact)
            {
                _c.MakeSchemaCompliant();
            }
            DateStamp.MakeSchemaCompliant();
            foreach (MD_identification_PropertyType _c in IdentificationInfo)
            {
                _c.MakeSchemaCompliant();
            }
        }
    }
}