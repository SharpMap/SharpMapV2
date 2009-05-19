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
    [Serializable, XmlType(TypeName = "LI_processStep_Type", Namespace = "http://www.isotc211.org/2005/gmd")]
    public class LI_processStep_Type : AbstractObjectType
    {
        [XmlIgnore] private DateTimePropertyType _dateTime;
        [XmlIgnore] private CharacterStringPropertyType _description;
        [XmlIgnore] private List<CI_responsibleParty_PropertyType> _processor;
        [XmlIgnore] private CharacterStringPropertyType _rationale;
        [XmlIgnore] private List<LI_source_PropertyType> _source;

        [XmlElement(Type = typeof (DateTimePropertyType), ElementName = "dateTime", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public DateTimePropertyType DateTime
        {
            get { return _dateTime; }
            set { _dateTime = value; }
        }

        [XmlElement(Type = typeof (CharacterStringPropertyType), ElementName = "description", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public CharacterStringPropertyType Description
        {
            get { return _description; }
            set { _description = value; }
        }

        [XmlElement(Type = typeof (CI_responsibleParty_PropertyType), ElementName = "processor", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<CI_responsibleParty_PropertyType> Processor
        {
            get
            {
                if (_processor == null)
                {
                    _processor = new List<CI_responsibleParty_PropertyType>();
                }
                return _processor;
            }
            set { _processor = value; }
        }

        [XmlElement(Type = typeof (CharacterStringPropertyType), ElementName = "rationale", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public CharacterStringPropertyType Rationale
        {
            get { return _rationale; }
            set { _rationale = value; }
        }

        [XmlElement(Type = typeof (LI_source_PropertyType), ElementName = "source", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<LI_source_PropertyType> Source
        {
            get
            {
                if (_source == null)
                {
                    _source = new List<LI_source_PropertyType>();
                }
                return _source;
            }
            set { _source = value; }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
            Description.MakeSchemaCompliant();
        }
    }
}