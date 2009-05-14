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
        [XmlIgnore] private double __x;

        [XmlIgnore] public bool __xSpecified;

        [XmlIgnore] private Unit _xUnits;

        [XmlIgnore] public bool __xunitsSpecified;
        [XmlIgnore] private double __y;

        [XmlIgnore] public bool __ySpecified;

        [XmlIgnore] private Unit __yunits;

        [XmlIgnore] public bool __yunitsSpecified;

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
            get { return __x; }
            set
            {
                __x = value;
                __xSpecified = true;
            }
        }

        [XmlAttribute(AttributeName = "y", DataType = "double")]
        public double Y
        {
            get { return __y; }
            set
            {
                __y = value;
                __ySpecified = true;
            }
        }

        [XmlAttribute(AttributeName = "xunits")]
        public Unit XUnits
        {
            get { return _xUnits; }
            set
            {
                _xUnits = value;
                __xunitsSpecified = true;
            }
        }

        [XmlAttribute(AttributeName = "yunits")]
        public Unit YUnits
        {
            get { return __yunits; }
            set
            {
                __yunits = value;
                __yunitsSpecified = true;
            }
        }

        public virtual void MakeSchemaCompliant()
        {
        }
    }
}