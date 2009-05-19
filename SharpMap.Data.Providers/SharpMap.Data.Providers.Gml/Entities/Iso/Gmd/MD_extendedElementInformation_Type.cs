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
     XmlType(TypeName = "MD_extendedElementInformation_Type", Namespace = "http://www.isotc211.org/2005/gmd")]
    public class MD_extendedElementInformation_Type : AbstractObjectType
    {
        [XmlIgnore] private CharacterStringPropertyType _condition;
        [XmlIgnore] private MD_datatypeCode_PropertyType _dataType;
        [XmlIgnore] private CharacterStringPropertyType _definition;
        [XmlIgnore] private IntegerPropertyType _domainCode;
        [XmlIgnore] private CharacterStringPropertyType _domainValue;
        [XmlIgnore] private CharacterStringPropertyType _maximumOccurrence;
        [XmlIgnore] private CharacterStringPropertyType _name;
        [XmlIgnore] private MD_obligationCode_PropertyType _obligation;
        [XmlIgnore] private List<CharacterStringPropertyType> _parentEntity;
        [XmlIgnore] private List<CharacterStringPropertyType> _rationale;
        [XmlIgnore] private CharacterStringPropertyType _rule;
        [XmlIgnore] private CharacterStringPropertyType _shortName;
        [XmlIgnore] private List<CI_responsibleParty_PropertyType> _source;

        [XmlElement(Type = typeof (CharacterStringPropertyType), ElementName = "condition", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public CharacterStringPropertyType Condition
        {
            get { return _condition; }
            set { _condition = value; }
        }

        [XmlElement(Type = typeof (MD_datatypeCode_PropertyType), ElementName = "dataType", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public MD_datatypeCode_PropertyType DataType
        {
            get { return _dataType; }
            set { _dataType = value; }
        }

        [XmlElement(Type = typeof (CharacterStringPropertyType), ElementName = "definition", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public CharacterStringPropertyType Definition
        {
            get { return _definition; }
            set { _definition = value; }
        }

        [XmlElement(Type = typeof (IntegerPropertyType), ElementName = "domainCode", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public IntegerPropertyType DomainCode
        {
            get { return _domainCode; }
            set { _domainCode = value; }
        }

        [XmlElement(Type = typeof (CharacterStringPropertyType), ElementName = "domainValue", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public CharacterStringPropertyType DomainValue
        {
            get { return _domainValue; }
            set { _domainValue = value; }
        }

        [XmlElement(Type = typeof (CharacterStringPropertyType), ElementName = "maximumOccurrence", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public CharacterStringPropertyType MaximumOccurrence
        {
            get { return _maximumOccurrence; }
            set { _maximumOccurrence = value; }
        }

        [XmlElement(Type = typeof (CharacterStringPropertyType), ElementName = "name", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public CharacterStringPropertyType Name
        {
            get { return _name; }
            set { _name = value; }
        }

        [XmlElement(Type = typeof (MD_obligationCode_PropertyType), ElementName = "obligation", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public MD_obligationCode_PropertyType Obligation
        {
            get { return _obligation; }
            set { _obligation = value; }
        }

        [XmlElement(Type = typeof (CharacterStringPropertyType), ElementName = "parentEntity", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<CharacterStringPropertyType> ParentEntity
        {
            get
            {
                if (_parentEntity == null)
                {
                    _parentEntity = new List<CharacterStringPropertyType>();
                }
                return _parentEntity;
            }
            set { _parentEntity = value; }
        }

        [XmlElement(Type = typeof (CharacterStringPropertyType), ElementName = "rationale", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<CharacterStringPropertyType> Rationale
        {
            get
            {
                if (_rationale == null)
                {
                    _rationale = new List<CharacterStringPropertyType>();
                }
                return _rationale;
            }
            set { _rationale = value; }
        }

        [XmlElement(Type = typeof (CharacterStringPropertyType), ElementName = "rule", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public CharacterStringPropertyType Rule
        {
            get { return _rule; }
            set { _rule = value; }
        }

        [XmlElement(Type = typeof (CharacterStringPropertyType), ElementName = "shortName", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public CharacterStringPropertyType ShortName
        {
            get { return _shortName; }
            set { _shortName = value; }
        }

        [XmlElement(Type = typeof (CI_responsibleParty_PropertyType), ElementName = "source", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<CI_responsibleParty_PropertyType> Source
        {
            get
            {
                if (_source == null)
                {
                    _source = new List<CI_responsibleParty_PropertyType>();
                }
                return _source;
            }
            set { _source = value; }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
            Name.MakeSchemaCompliant();
            Definition.MakeSchemaCompliant();
            DataType.MakeSchemaCompliant();
            foreach (CharacterStringPropertyType _c in ParentEntity)
            {
                _c.MakeSchemaCompliant();
            }
            Rule.MakeSchemaCompliant();
            foreach (CI_responsibleParty_PropertyType _c in Source)
            {
                _c.MakeSchemaCompliant();
            }
        }
    }
}