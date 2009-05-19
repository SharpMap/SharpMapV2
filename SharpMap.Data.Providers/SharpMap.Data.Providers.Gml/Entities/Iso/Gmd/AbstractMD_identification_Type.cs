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
    [Serializable, XmlType(TypeName = "AbstractMD_identification_Type", Namespace = "http://www.isotc211.org/2005/gmd")]
    public abstract class AbstractMD_identification_Type : AbstractObjectType
    {
        [XmlIgnore] private CharacterStringPropertyType _abstract;
        [XmlIgnore] private List<MD_aggregateInformation_PropertyType> _aggregationInfo;
        [XmlIgnore] private CI_citation_PropertyType _citation;
        [XmlIgnore] private List<CharacterStringPropertyType> _credit;
        [XmlIgnore] private List<MD_keywords_PropertyType> _descriptiveKeywords;
        [XmlIgnore] private List<MD_browseGraphic_PropertyType> _graphicOverview;
        [XmlIgnore] private List<CI_responsibleParty_PropertyType> _pointOfContact;
        [XmlIgnore] private CharacterStringPropertyType _purpose;
        [XmlIgnore] private List<MD_constraints_PropertyType> _resourceConstraints;
        [XmlIgnore] private List<MD_format_PropertyType> _resourceFormat;
        [XmlIgnore] private List<MD_maintenanceInformation_PropertyType> _resourceMaintenance;
        [XmlIgnore] private List<MD_usage_PropertyType> _resourceSpecificUsage;
        [XmlIgnore] private List<MD_progressCode_PropertyType> _status;

        [XmlElement(Type = typeof (CharacterStringPropertyType), ElementName = "abstract", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public CharacterStringPropertyType Abstract
        {
            get { return _abstract; }
            set { _abstract = value; }
        }

        [XmlElement(Type = typeof (MD_aggregateInformation_PropertyType), ElementName = "aggregationInfo",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<MD_aggregateInformation_PropertyType> AggregationInfo
        {
            get
            {
                if (_aggregationInfo == null)
                {
                    _aggregationInfo = new List<MD_aggregateInformation_PropertyType>();
                }
                return _aggregationInfo;
            }
            set { _aggregationInfo = value; }
        }

        [XmlElement(Type = typeof (CI_citation_PropertyType), ElementName = "citation", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public CI_citation_PropertyType Citation
        {
            get { return _citation; }
            set { _citation = value; }
        }

        [XmlElement(Type = typeof (CharacterStringPropertyType), ElementName = "credit", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<CharacterStringPropertyType> Credit
        {
            get
            {
                if (_credit == null)
                {
                    _credit = new List<CharacterStringPropertyType>();
                }
                return _credit;
            }
            set { _credit = value; }
        }

        [XmlElement(Type = typeof (MD_keywords_PropertyType), ElementName = "descriptiveKeywords", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<MD_keywords_PropertyType> DescriptiveKeywords
        {
            get
            {
                if (_descriptiveKeywords == null)
                {
                    _descriptiveKeywords = new List<MD_keywords_PropertyType>();
                }
                return _descriptiveKeywords;
            }
            set { _descriptiveKeywords = value; }
        }

        [XmlElement(Type = typeof (MD_browseGraphic_PropertyType), ElementName = "graphicOverview", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<MD_browseGraphic_PropertyType> GraphicOverview
        {
            get
            {
                if (_graphicOverview == null)
                {
                    _graphicOverview = new List<MD_browseGraphic_PropertyType>();
                }
                return _graphicOverview;
            }
            set { _graphicOverview = value; }
        }

        [XmlElement(Type = typeof (CI_responsibleParty_PropertyType), ElementName = "pointOfContact", IsNullable = false
            , Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<CI_responsibleParty_PropertyType> PointOfContact
        {
            get
            {
                if (_pointOfContact == null)
                {
                    _pointOfContact = new List<CI_responsibleParty_PropertyType>();
                }
                return _pointOfContact;
            }
            set { _pointOfContact = value; }
        }

        [XmlElement(Type = typeof (CharacterStringPropertyType), ElementName = "purpose", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public CharacterStringPropertyType Purpose
        {
            get { return _purpose; }
            set { _purpose = value; }
        }

        [XmlElement(Type = typeof (MD_constraints_PropertyType), ElementName = "resourceConstraints", IsNullable = false
            , Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<MD_constraints_PropertyType> ResourceConstraints
        {
            get
            {
                if (_resourceConstraints == null)
                {
                    _resourceConstraints = new List<MD_constraints_PropertyType>();
                }
                return _resourceConstraints;
            }
            set { _resourceConstraints = value; }
        }

        [XmlElement(Type = typeof (MD_format_PropertyType), ElementName = "resourceFormat", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<MD_format_PropertyType> ResourceFormat
        {
            get
            {
                if (_resourceFormat == null)
                {
                    _resourceFormat = new List<MD_format_PropertyType>();
                }
                return _resourceFormat;
            }
            set { _resourceFormat = value; }
        }

        [XmlElement(Type = typeof (MD_maintenanceInformation_PropertyType), ElementName = "resourceMaintenance",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<MD_maintenanceInformation_PropertyType> ResourceMaintenance
        {
            get
            {
                if (_resourceMaintenance == null)
                {
                    _resourceMaintenance = new List<MD_maintenanceInformation_PropertyType>();
                }
                return _resourceMaintenance;
            }
            set { _resourceMaintenance = value; }
        }

        [XmlElement(Type = typeof (MD_usage_PropertyType), ElementName = "resourceSpecificUsage", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<MD_usage_PropertyType> ResourceSpecificUsage
        {
            get
            {
                if (_resourceSpecificUsage == null)
                {
                    _resourceSpecificUsage = new List<MD_usage_PropertyType>();
                }
                return _resourceSpecificUsage;
            }
            set { _resourceSpecificUsage = value; }
        }

        [XmlElement(Type = typeof (MD_progressCode_PropertyType), ElementName = "status", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<MD_progressCode_PropertyType> Status
        {
            get
            {
                if (_status == null)
                {
                    _status = new List<MD_progressCode_PropertyType>();
                }
                return _status;
            }
            set { _status = value; }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
            Citation.MakeSchemaCompliant();
            Abstract.MakeSchemaCompliant();
        }
    }
}