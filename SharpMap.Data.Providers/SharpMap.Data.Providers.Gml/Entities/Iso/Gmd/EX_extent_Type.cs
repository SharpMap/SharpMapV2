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
    [Serializable, XmlType(TypeName = "EX_extent_Type", Namespace = "http://www.isotc211.org/2005/gmd")]
    public class EX_extent_Type : AbstractObjectType
    {
        [XmlIgnore] private CharacterStringPropertyType _description;
        [XmlIgnore] private List<EX_geographicExtent_PropertyType> _geographicElement;
        [XmlIgnore] private List<EX_temporalExtent_PropertyType> _temporalElement;
        [XmlIgnore] private List<EX_verticalExtent_PropertyType> _verticalElement;

        [XmlElement(Type = typeof (CharacterStringPropertyType), ElementName = "description", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public CharacterStringPropertyType Description
        {
            get { return _description; }
            set { _description = value; }
        }

        [XmlElement(Type = typeof (EX_geographicExtent_PropertyType), ElementName = "geographicElement",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<EX_geographicExtent_PropertyType> GeographicElement
        {
            get
            {
                if (_geographicElement == null)
                {
                    _geographicElement = new List<EX_geographicExtent_PropertyType>();
                }
                return _geographicElement;
            }
            set { _geographicElement = value; }
        }

        [XmlElement(Type = typeof (EX_temporalExtent_PropertyType), ElementName = "temporalElement", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<EX_temporalExtent_PropertyType> TemporalElement
        {
            get
            {
                if (_temporalElement == null)
                {
                    _temporalElement = new List<EX_temporalExtent_PropertyType>();
                }
                return _temporalElement;
            }
            set { _temporalElement = value; }
        }

        [XmlElement(Type = typeof (EX_verticalExtent_PropertyType), ElementName = "verticalElement", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<EX_verticalExtent_PropertyType> VerticalElement
        {
            get
            {
                if (_verticalElement == null)
                {
                    _verticalElement = new List<EX_verticalExtent_PropertyType>();
                }
                return _verticalElement;
            }
            set { _verticalElement = value; }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }
    }
}