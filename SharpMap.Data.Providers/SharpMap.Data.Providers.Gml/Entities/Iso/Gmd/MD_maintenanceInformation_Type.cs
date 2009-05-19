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
using SharpMap.Entities.Iso.Gts;

namespace SharpMap.Entities.Iso.Gmd
{
    [Serializable, XmlType(TypeName = "MD_maintenanceInformation_Type", Namespace = "http://www.isotc211.org/2005/gmd")]
    public class MD_maintenanceInformation_Type : AbstractObjectType
    {
        [XmlIgnore] private List<CI_responsibleParty_PropertyType> _contact;
        [XmlIgnore] private DatePropertyType _dateOfNextUpdate;
        [XmlIgnore] private MD_maintenanceFrequencyCode_PropertyType _maintenanceAndUpdateFrequency;
        [XmlIgnore] private List<CharacterStringPropertyType> _maintenanceNote;
        [XmlIgnore] private List<MD_scopeCode_PropertyType> _updateScope;
        [XmlIgnore] private List<MD_scopeDescription_PropertyType> _updateScopeDescription;
        [XmlIgnore] private TM_periodDuration_PropertyType _userDefinedMaintenanceFrequency;

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

        [XmlElement(Type = typeof (DatePropertyType), ElementName = "dateOfNextUpdate", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public DatePropertyType DateOfNextUpdate
        {
            get { return _dateOfNextUpdate; }
            set { _dateOfNextUpdate = value; }
        }

        [XmlElement(Type = typeof (MD_maintenanceFrequencyCode_PropertyType),
            ElementName = "maintenanceAndUpdateFrequency", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = "http://www.isotc211.org/2005/gmd")]
        public MD_maintenanceFrequencyCode_PropertyType MaintenanceAndUpdateFrequency
        {
            get { return _maintenanceAndUpdateFrequency; }
            set { _maintenanceAndUpdateFrequency = value; }
        }

        [XmlElement(Type = typeof (CharacterStringPropertyType), ElementName = "maintenanceNote", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<CharacterStringPropertyType> MaintenanceNote
        {
            get
            {
                if (_maintenanceNote == null)
                {
                    _maintenanceNote = new List<CharacterStringPropertyType>();
                }
                return _maintenanceNote;
            }
            set { _maintenanceNote = value; }
        }

        [XmlElement(Type = typeof (MD_scopeCode_PropertyType), ElementName = "updateScope", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<MD_scopeCode_PropertyType> UpdateScope
        {
            get
            {
                if (_updateScope == null)
                {
                    _updateScope = new List<MD_scopeCode_PropertyType>();
                }
                return _updateScope;
            }
            set { _updateScope = value; }
        }

        [XmlElement(Type = typeof (MD_scopeDescription_PropertyType), ElementName = "updateScopeDescription",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<MD_scopeDescription_PropertyType> UpdateScopeDescription
        {
            get
            {
                if (_updateScopeDescription == null)
                {
                    _updateScopeDescription = new List<MD_scopeDescription_PropertyType>();
                }
                return _updateScopeDescription;
            }
            set { _updateScopeDescription = value; }
        }

        [XmlElement(Type = typeof (TM_periodDuration_PropertyType), ElementName = "userDefinedMaintenanceFrequency",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public TM_periodDuration_PropertyType UserDefinedMaintenanceFrequency
        {
            get { return _userDefinedMaintenanceFrequency; }
            set { _userDefinedMaintenanceFrequency = value; }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
            MaintenanceAndUpdateFrequency.MakeSchemaCompliant();
        }
    }
}