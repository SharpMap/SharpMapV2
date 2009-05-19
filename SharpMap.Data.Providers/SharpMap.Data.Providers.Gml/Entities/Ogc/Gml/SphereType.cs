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
using System.Xml.Serialization;

namespace SharpMap.Entities.Ogc.Gml
{
    [Serializable, XmlType(TypeName = "SphereType", Namespace = Declarations.SchemaVersion)]
    public class SphereType : AbstractGriddedSurfaceType
    {
        [XmlIgnore] private CurveInterpolationType _horizontalCurveType;
        [XmlIgnore] private CurveInterpolationType _verticalCurveType;
        [XmlIgnore] public bool HorizontalCurveTypeSpecified;
        [XmlIgnore] public bool VerticalCurveTypeSpecified;

        public SphereType()
        {
            HorizontalCurveType = CurveInterpolationType.CircularArc3Points;
            VerticalCurveType = CurveInterpolationType.CircularArc3Points;
        }

        [XmlAttribute(AttributeName = "horizontalCurveType")]
        public CurveInterpolationType HorizontalCurveType
        {
            get { return _horizontalCurveType; }
            set
            {
                _horizontalCurveType = value;
                HorizontalCurveTypeSpecified = true;
            }
        }

        [XmlAttribute(AttributeName = "verticalCurveType")]
        public CurveInterpolationType VerticalCurveType
        {
            get { return _verticalCurveType; }
            set
            {
                _verticalCurveType = value;
                VerticalCurveTypeSpecified = true;
            }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }
    }
}