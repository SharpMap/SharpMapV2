// /*
//  *  The attached / following is part of SharpMap.Data.Providers.Kml
//  *  SharpMap.Data.Providers.Kml is free software © 2008 Newgrove Consultants Limited, 
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

namespace SharpMap.Entities.Ogc.Kml
{
    [XmlType(TypeName = "vec2Type", Namespace = Declarations.SchemaVersion), Serializable]
    public class Vector2
    {
        [XmlIgnore] private double _x;

        [XmlIgnore] public bool _xSpecified;

        [XmlIgnore] public bool _xunitsSpecified;
        [XmlIgnore] private double _y;

        [XmlIgnore] public bool _ySpecified;

        [XmlIgnore] private Unit _yunits;

        [XmlIgnore] public bool _yunitsSpecified;
        [XmlIgnore] private Unit _xUnits;

        public Vector2()
        {
            X = 1.0;
            Y = 1.0;
            XUnits = Unit.Fraction;
            YUnits = Unit.Fraction;
        }

        [XmlAttribute(AttributeName = "x", DataType = "double")]
        public double X
        {
            get { return _x; }
            set
            {
                _x = value;
                _xSpecified = true;
            }
        }

        [XmlAttribute(AttributeName = "y", DataType = "double")]
        public double Y
        {
            get { return _y; }
            set
            {
                _y = value;
                _ySpecified = true;
            }
        }

        [XmlAttribute(AttributeName = "xunits")]
        public Unit XUnits
        {
            get { return _xUnits; }
            set
            {
                _xUnits = value;
                _xunitsSpecified = true;
            }
        }

        [XmlAttribute(AttributeName = "yunits")]
        public Unit YUnits
        {
            get { return _yunits; }
            set
            {
                _yunits = value;
                _yunitsSpecified = true;
            }
        }

        public virtual void MakeSchemaCompliant()
        {
        }
    }
}