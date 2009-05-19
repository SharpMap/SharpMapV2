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
using System.Xml.Schema;
using System.Xml.Serialization;
using SharpMap.Entities.Iso.Gco;

namespace SharpMap.Entities.Iso.Gmd
{
    [Serializable, XmlType(TypeName = "MD_dimension_Type", Namespace = "http://www.isotc211.org/2005/gmd")]
    public class MD_dimension_Type : AbstractObjectType
    {
        [XmlIgnore] private MD_dimensionNameTypeCode_PropertyType _dimensionName;
        [XmlIgnore] private IntegerPropertyType _dimensionSize;
        [XmlIgnore] private MeasurePropertyType _resolution;

        [XmlElement(Type = typeof (MD_dimensionNameTypeCode_PropertyType), ElementName = "dimensionName",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public MD_dimensionNameTypeCode_PropertyType DimensionName
        {
            get { return _dimensionName; }
            set { _dimensionName = value; }
        }

        [XmlElement(Type = typeof (IntegerPropertyType), ElementName = "dimensionSize", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public IntegerPropertyType DimensionSize
        {
            get { return _dimensionSize; }
            set { _dimensionSize = value; }
        }

        [XmlElement(Type = typeof (MeasurePropertyType), ElementName = "resolution", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public MeasurePropertyType Resolution
        {
            get { return _resolution; }
            set { _resolution = value; }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
            DimensionName.MakeSchemaCompliant();
            DimensionSize.MakeSchemaCompliant();
        }
    }
}