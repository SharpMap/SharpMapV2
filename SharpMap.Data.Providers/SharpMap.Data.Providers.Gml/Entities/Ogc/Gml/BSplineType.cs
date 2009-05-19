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

namespace SharpMap.Entities.Ogc.Gml
{
    [Serializable, XmlType(TypeName = "BSplineType", Namespace = Declarations.SchemaVersion)]
    public class BSplineType : AbstractCurveSegmentType
    {
        [XmlIgnore] private Coordinates _coordinates;
        [XmlIgnore] private string _degree;
        [XmlIgnore] private CurveInterpolationType _interpolation;
        [XmlIgnore] private Boolean _isPolynomial;
        [XmlIgnore] private List<KnotPropertyType> _knot;
        [XmlIgnore] private KnotTypesType _knotType;
        [XmlIgnore] private List<PointProperty> _pointProperty;
        [XmlIgnore] private List<PointRep> _pointRep;
        [XmlIgnore] private List<Pos> _pos;
        [XmlIgnore] private PosList _posList;
        [XmlIgnore] public bool InterpolationSpecified;
        [XmlIgnore] public bool IsPolynomialSpecified;
        [XmlIgnore] public bool KnotTypeSpecified;

        public BSplineType()
        {
            Interpolation = CurveInterpolationType.PolynomialSpline;
            Degree = string.Empty;
        }

        [XmlElement(Type = typeof (Coordinates), ElementName = "coordinates", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public Coordinates Coordinates
        {
            get { return _coordinates; }
            set { _coordinates = value; }
        }

        [XmlElement(ElementName = "degree", IsNullable = false, Form = XmlSchemaForm.Qualified,
            DataType = "nonNegativeInteger", Namespace = Declarations.SchemaVersion)]
        public string Degree
        {
            get { return _degree; }
            set { _degree = value; }
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

        [XmlAttribute(AttributeName = "isPolynomial", DataType = "boolean")]
        public Boolean IsPolynomial
        {
            get { return _isPolynomial; }
            set
            {
                _isPolynomial = value;
                IsPolynomialSpecified = true;
            }
        }

        [XmlElement(Type = typeof (KnotPropertyType), ElementName = "knot", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<KnotPropertyType> Knot
        {
            get
            {
                if (_knot == null)
                {
                    _knot = new List<KnotPropertyType>();
                }
                return _knot;
            }
            set { _knot = value; }
        }

        [XmlAttribute(AttributeName = "knotType")]
        public KnotTypesType KnotType
        {
            get { return _knotType; }
            set
            {
                _knotType = value;
                KnotTypeSpecified = true;
            }
        }

        [XmlElement(Type = typeof (PointProperty), ElementName = "pointProperty", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<PointProperty> PointProperty
        {
            get
            {
                if (_pointProperty == null)
                {
                    _pointProperty = new List<PointProperty>();
                }
                return _pointProperty;
            }
            set { _pointProperty = value; }
        }

        [XmlElement(Type = typeof (PointRep), ElementName = "pointRep", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<PointRep> PointRep
        {
            get
            {
                if (_pointRep == null)
                {
                    _pointRep = new List<PointRep>();
                }
                return _pointRep;
            }
            set { _pointRep = value; }
        }

        [XmlElement(Type = typeof (Pos), ElementName = "pos", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = Declarations.SchemaVersion)]
        public List<Pos> Pos
        {
            get
            {
                if (_pos == null)
                {
                    _pos = new List<Pos>();
                }
                return _pos;
            }
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
            foreach (Pos _c in Pos)
            {
                _c.MakeSchemaCompliant();
            }
            foreach (PointProperty _c in PointProperty)
            {
                _c.MakeSchemaCompliant();
            }
            foreach (PointRep _c in PointRep)
            {
                _c.MakeSchemaCompliant();
            }
            PosList.MakeSchemaCompliant();
            Coordinates.MakeSchemaCompliant();
            foreach (KnotPropertyType _c in Knot)
            {
                _c.MakeSchemaCompliant();
            }
        }
    }
}