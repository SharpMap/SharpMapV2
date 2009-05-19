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
    [Serializable, XmlType(TypeName = "LI_lineage_Type", Namespace = "http://www.isotc211.org/2005/gmd")]
    public class LI_lineage_Type : AbstractObjectType
    {
        [XmlIgnore] private List<LI_processStep_PropertyType> _processStep;
        [XmlIgnore] private List<LI_source_PropertyType> _source;
        [XmlIgnore] private CharacterStringPropertyType _statement;

        [XmlElement(Type = typeof (LI_processStep_PropertyType), ElementName = "processStep", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<LI_processStep_PropertyType> ProcessStep
        {
            get
            {
                if (_processStep == null)
                {
                    _processStep = new List<LI_processStep_PropertyType>();
                }
                return _processStep;
            }
            set { _processStep = value; }
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

        [XmlElement(Type = typeof (CharacterStringPropertyType), ElementName = "statement", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public CharacterStringPropertyType Statement
        {
            get { return _statement; }
            set { _statement = value; }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }
    }
}