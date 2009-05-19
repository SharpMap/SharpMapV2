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
using SharpMap.Entities.Iso.Gss;

namespace SharpMap.Entities.Iso.Gmd
{
    [Serializable, XmlType(TypeName = "MD_georectified_Type", Namespace = "http://www.isotc211.org/2005/gmd")]
    public class MD_georectified_Type : MD_gridSpatialRepresentation_Type
    {
        [XmlIgnore] private GM_point_PropertyType _centerPoint;
        [XmlIgnore] private BooleanPropertyType _checkPointAvailability;
        [XmlIgnore] private CharacterStringPropertyType _checkPointDescription;
        [XmlIgnore] private List<GM_point_PropertyType> _cornerPoints;
        [XmlIgnore] private MD_pixelOrientationCode_PropertyType _pointInPixel;
        [XmlIgnore] private CharacterStringPropertyType _transformationDimensionDescription;
        [XmlIgnore] private List<CharacterStringPropertyType> _transformationDimensionMapping;

        [XmlElement(Type = typeof (GM_point_PropertyType), ElementName = "centerPoint", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public GM_point_PropertyType CenterPoint
        {
            get { return _centerPoint; }
            set { _centerPoint = value; }
        }

        [XmlElement(Type = typeof (BooleanPropertyType), ElementName = "checkPointAvailability", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public BooleanPropertyType CheckPointAvailability
        {
            get { return _checkPointAvailability; }
            set { _checkPointAvailability = value; }
        }

        [XmlElement(Type = typeof (CharacterStringPropertyType), ElementName = "checkPointDescription",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public CharacterStringPropertyType CheckPointDescription
        {
            get { return _checkPointDescription; }
            set { _checkPointDescription = value; }
        }

        [XmlElement(Type = typeof (GM_point_PropertyType), ElementName = "cornerPoints", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<GM_point_PropertyType> CornerPoints
        {
            get
            {
                if (_cornerPoints == null)
                {
                    _cornerPoints = new List<GM_point_PropertyType>();
                }
                return _cornerPoints;
            }
            set { _cornerPoints = value; }
        }

        [XmlElement(Type = typeof (MD_pixelOrientationCode_PropertyType), ElementName = "pointInPixel",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public MD_pixelOrientationCode_PropertyType PointInPixel
        {
            get { return _pointInPixel; }
            set { _pointInPixel = value; }
        }

        [XmlElement(Type = typeof (CharacterStringPropertyType), ElementName = "transformationDimensionDescription",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public CharacterStringPropertyType TransformationDimensionDescription
        {
            get { return _transformationDimensionDescription; }
            set { _transformationDimensionDescription = value; }
        }

        [XmlElement(Type = typeof (CharacterStringPropertyType), ElementName = "transformationDimensionMapping",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<CharacterStringPropertyType> TransformationDimensionMapping
        {
            get
            {
                if (_transformationDimensionMapping == null)
                {
                    _transformationDimensionMapping = new List<CharacterStringPropertyType>();
                }
                return _transformationDimensionMapping;
            }
            set { _transformationDimensionMapping = value; }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
            CheckPointAvailability.MakeSchemaCompliant();
            PointInPixel.MakeSchemaCompliant();
        }
    }
}