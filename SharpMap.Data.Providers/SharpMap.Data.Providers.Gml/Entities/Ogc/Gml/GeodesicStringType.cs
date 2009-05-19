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

namespace SharpMap.Entities.Ogc.Gml
{
    [Serializable, XmlType(TypeName = "GeodesicStringType", Namespace = Declarations.SchemaVersion)]
    public class GeodesicStringType : AbstractCurveSegmentType
    {
        [XmlIgnore] private CurveInterpolationType _interpolation;
        [XmlIgnore] private PointProperty _pointProperty;
        [XmlIgnore] private Pos _pos;
        [XmlIgnore] private PosList _posList;
        [XmlIgnore] public bool InterpolationSpecified;

        public GeodesicStringType()
        {
            Interpolation = CurveInterpolationType.Geodesic;
        }

        [XmlAttribute(AttributeName = "interpolation")]
        public CurveInterpolationType Interpolation
        {
            get { return _interpolation; }
            set
            {
                _interpolation = value;
                InterpolationSpecified = true;
            }
        }

        [XmlElement(Type = typeof (PointProperty), ElementName = "pointProperty", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public PointProperty PointProperty
        {
            get { return _pointProperty; }
            set { _pointProperty = value; }
        }

        [XmlElement(Type = typeof (Pos), ElementName = "pos", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = Declarations.SchemaVersion)]
        public Pos Pos
        {
            get { return _pos; }
            set { _pos = value; }
        }

        [XmlElement(Type = typeof (PosList), ElementName = "posList", IsNullable = false, Form = XmlSchemaForm.Qualified
            , Namespace = Declarations.SchemaVersion)]
        public PosList PosList
        {
            get { return _posList; }
            set { _posList = value; }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
            PosList.MakeSchemaCompliant();
            Pos.MakeSchemaCompliant();
            PointProperty.MakeSchemaCompliant();
        }
    }
}