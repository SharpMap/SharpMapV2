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
    [Serializable, XmlType(TypeName = "ArcByBulgeType", Namespace = "http://www.opengis.net/gml/3.2")]
    public class ArcByBulgeType : ArcStringByBulgeType
    {
        [XmlIgnore] private double _bulge;
        [XmlIgnore] private Coordinates _coordinates;
        [XmlIgnore] private VectorType _normal;
        [XmlIgnore] private string _numArc;
        [XmlIgnore] private List<PointProperty> _pointProperty;
        [XmlIgnore] private List<PointRep> _pointRep;
        [XmlIgnore] private List<Pos> _pos;
        [XmlIgnore] private PosList _posList;
        [XmlIgnore] public bool BulgeSpecified;

        public ArcByBulgeType()
        {
            NumArc = "1";
            BulgeSpecified = true;
        }

        [XmlElement(ElementName = "bulge", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "double",
            Namespace = "http://www.opengis.net/gml/3.2")]
        public double Bulge
        {
            get { return _bulge; }
            set
            {
                _bulge = value;
                BulgeSpecified = true;
            }
        }

        [XmlElement(Type = typeof (Coordinates), ElementName = "coordinates", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public Coordinates Coordinates
        {
            get { return _coordinates; }
            set { _coordinates = value; }
        }

        [XmlElement(Type = typeof (VectorType), ElementName = "normal", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public VectorType Normal
        {
            get { return _normal; }
            set { _normal = value; }
        }

        [XmlAttribute(AttributeName = "numArc", DataType = "integer")]
        public string NumArc
        {
            get { return _numArc; }
            set { _numArc = value; }
        }

        [XmlElement(Type = typeof (PointProperty), ElementName = "pointProperty", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
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
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
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
            Namespace = "http://www.opengis.net/gml/3.2")]
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
            , Namespace = "http://www.opengis.net/gml/3.2")]
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
            Normal.MakeSchemaCompliant();
        }
    }
}