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
    [Serializable, XmlType(TypeName = "ArcByCenterPointType", Namespace = "http://www.opengis.net/gml/3.2")]
    public class ArcByCenterPointType : AbstractCurveSegmentType
    {
        [XmlIgnore] private Coordinates _coordinates;
        [XmlIgnore] private AngleType _endAngle;
        [XmlIgnore] private CurveInterpolationType _interpolation;
        [XmlIgnore] private string _numArc;
        [XmlIgnore] private PointProperty _pointProperty;
        [XmlIgnore] private PointRep _pointRep;
        [XmlIgnore] private Pos _pos;
        [XmlIgnore] private PosList _posList;
        [XmlIgnore] private LengthType _radius;
        [XmlIgnore] private AngleType _startAngle;
        [XmlIgnore] public bool InterpolationSpecified;

        public ArcByCenterPointType()
        {
            Interpolation = CurveInterpolationType.CircularArcCenterPointWithRadius;
            NumArc = "1";
        }

        [XmlElement(Type = typeof (Coordinates), ElementName = "coordinates", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public Coordinates Coordinates
        {
            get { return _coordinates; }
            set { _coordinates = value; }
        }

        [XmlElement(Type = typeof (AngleType), ElementName = "endAngle", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public AngleType EndAngle
        {
            get { return _endAngle; }
            set { _endAngle = value; }
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

        [XmlAttribute(AttributeName = "numArc", DataType = "integer")]
        public string NumArc
        {
            get { return _numArc; }
            set { _numArc = value; }
        }

        [XmlElement(Type = typeof (PointProperty), ElementName = "pointProperty", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public PointProperty PointProperty
        {
            get { return _pointProperty; }
            set { _pointProperty = value; }
        }

        [XmlElement(Type = typeof (PointRep), ElementName = "pointRep", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public PointRep PointRep
        {
            get { return _pointRep; }
            set { _pointRep = value; }
        }

        [XmlElement(Type = typeof (Pos), ElementName = "pos", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = "http://www.opengis.net/gml/3.2")]
        public Pos Pos
        {
            get { return _pos; }
            set { _pos = value; }
        }

        [XmlElement(Type = typeof (PosList), ElementName = "posList", IsNullable = false, Form = XmlSchemaForm.Qualified
            , Namespace = "http://www.opengis.net/gml/3.2")]
        public PosList PosList
        {
            get { return _posList; }
            set { _posList = value; }
        }

        [XmlElement(Type = typeof (LengthType), ElementName = "radius", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public LengthType Radius
        {
            get { return _radius; }
            set { _radius = value; }
        }

        [XmlElement(Type = typeof (AngleType), ElementName = "startAngle", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public AngleType StartAngle
        {
            get { return _startAngle; }
            set { _startAngle = value; }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
            Pos.MakeSchemaCompliant();
            PointProperty.MakeSchemaCompliant();
            PointRep.MakeSchemaCompliant();
            PosList.MakeSchemaCompliant();
            Coordinates.MakeSchemaCompliant();
            Radius.MakeSchemaCompliant();
        }
    }
}