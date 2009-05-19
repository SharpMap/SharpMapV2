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
     XmlType(TypeName = "MD_gridSpatialRepresentation_Type", Namespace = "http://www.isotc211.org/2005/gmd")]
    public class MD_gridSpatialRepresentation_Type : AbstractMD_spatialRepresentation_Type
    {
        [XmlIgnore] private List<MD_dimension_PropertyType> _axisDimensionProperties;
        [XmlIgnore] private MD_cellGeometryCode_PropertyType _cellGeometry;
        [XmlIgnore] private IntegerPropertyType _numberOfDimensions;
        [XmlIgnore] private BooleanPropertyType _transformationParameterAvailability;

        [XmlElement(Type = typeof (MD_dimension_PropertyType), ElementName = "axisDimensionProperties",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<MD_dimension_PropertyType> AxisDimensionProperties
        {
            get
            {
                if (_axisDimensionProperties == null)
                {
                    _axisDimensionProperties = new List<MD_dimension_PropertyType>();
                }
                return _axisDimensionProperties;
            }
            set { _axisDimensionProperties = value; }
        }

        [XmlElement(Type = typeof (MD_cellGeometryCode_PropertyType), ElementName = "cellGeometry", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public MD_cellGeometryCode_PropertyType CellGeometry
        {
            get { return _cellGeometry; }
            set { _cellGeometry = value; }
        }

        [XmlElement(Type = typeof (IntegerPropertyType), ElementName = "numberOfDimensions", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public IntegerPropertyType NumberOfDimensions
        {
            get { return _numberOfDimensions; }
            set { _numberOfDimensions = value; }
        }

        [XmlElement(Type = typeof (BooleanPropertyType), ElementName = "transformationParameterAvailability",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public BooleanPropertyType TransformationParameterAvailability
        {
            get { return _transformationParameterAvailability; }
            set { _transformationParameterAvailability = value; }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
            NumberOfDimensions.MakeSchemaCompliant();
            CellGeometry.MakeSchemaCompliant();
            TransformationParameterAvailability.MakeSchemaCompliant();
        }
    }
}