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
    [Serializable, XmlType(TypeName = "MD_georeferenceable_Type", Namespace = "http://www.isotc211.org/2005/gmd")]
    public class MD_georeferenceable_Type : MD_gridSpatialRepresentation_Type
    {
        [XmlIgnore] private BooleanPropertyType _controlPointAvailability;
        [XmlIgnore] private RecordPropertyType _georeferencedParameters;
        [XmlIgnore] private BooleanPropertyType _orientationParameterAvailability;
        [XmlIgnore] private CharacterStringPropertyType _orientationParameterDescription;
        [XmlIgnore] private List<CI_citation_PropertyType> _parameterCitation;

        [XmlElement(Type = typeof (BooleanPropertyType), ElementName = "controlPointAvailability", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public BooleanPropertyType ControlPointAvailability
        {
            get { return _controlPointAvailability; }
            set { _controlPointAvailability = value; }
        }

        [XmlElement(Type = typeof (RecordPropertyType), ElementName = "georeferencedParameters", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public RecordPropertyType GeoreferencedParameters
        {
            get { return _georeferencedParameters; }
            set { _georeferencedParameters = value; }
        }

        [XmlElement(Type = typeof (BooleanPropertyType), ElementName = "orientationParameterAvailability",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public BooleanPropertyType OrientationParameterAvailability
        {
            get { return _orientationParameterAvailability; }
            set { _orientationParameterAvailability = value; }
        }

        [XmlElement(Type = typeof (CharacterStringPropertyType), ElementName = "orientationParameterDescription",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public CharacterStringPropertyType OrientationParameterDescription
        {
            get { return _orientationParameterDescription; }
            set { _orientationParameterDescription = value; }
        }

        [XmlElement(Type = typeof (CI_citation_PropertyType), ElementName = "parameterCitation", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<CI_citation_PropertyType> ParameterCitation
        {
            get
            {
                if (_parameterCitation == null)
                {
                    _parameterCitation = new List<CI_citation_PropertyType>();
                }
                return _parameterCitation;
            }
            set { _parameterCitation = value; }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
            ControlPointAvailability.MakeSchemaCompliant();
            OrientationParameterAvailability.MakeSchemaCompliant();
            GeoreferencedParameters.MakeSchemaCompliant();
        }
    }
}