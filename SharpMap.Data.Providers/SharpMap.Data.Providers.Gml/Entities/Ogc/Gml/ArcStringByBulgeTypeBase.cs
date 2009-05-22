using System.Collections.Generic;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SharpMap.Entities.Ogc.Gml
{
    public class ArcStringByBulgeTypeBase : AbstractCurveSegmentType
    {
        [XmlIgnore]
        private Coordinates _coordinates;

        [XmlIgnore]
        private CurveInterpolationType _interpolation;

        [XmlIgnore]
        private string _numArc;

        [XmlIgnore]
        private List<PointProperty> _pointProperty;

        [XmlIgnore]
        private List<PointRep> _pointRep;

        [XmlIgnore]
        private List<Pos> _pos;

        [XmlIgnore]
        private PosList _posList;

        [XmlIgnore]
        public bool InterpolationSpecified;

        public ArcStringByBulgeTypeBase()
        {
            Interpolation = CurveInterpolationType.CircularArc2PointWithBulge;
        }

        [XmlElement(Type = typeof(Coordinates), ElementName = "coordinates", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public Coordinates Coordinates
        {
            get { return _coordinates; }
            set { _coordinates = value; }
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

        [XmlElement(Type = typeof(PointProperty), ElementName = "pointProperty", IsNullable = false,
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

        [XmlElement(Type = typeof(PointRep), ElementName = "pointRep", IsNullable = false,
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

        [XmlElement(Type = typeof(Pos), ElementName = "pos", IsNullable = false, Form = XmlSchemaForm.Qualified,
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

        [XmlElement(Type = typeof(PosList), ElementName = "posList", IsNullable = false, Form = XmlSchemaForm.Qualified
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

        }
    }
}