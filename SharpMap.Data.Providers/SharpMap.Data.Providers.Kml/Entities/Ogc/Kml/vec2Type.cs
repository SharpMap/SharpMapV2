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
    public class vec2Type
    {
        [XmlIgnore] private double __x;

        [XmlIgnore] public bool __xSpecified;

        [XmlIgnore] private unitsEnumType __xunits;

        [XmlIgnore] public bool __xunitsSpecified;
        [XmlIgnore] private double __y;

        [XmlIgnore] public bool __ySpecified;

        [XmlIgnore] private unitsEnumType __yunits;

        [XmlIgnore] public bool __yunitsSpecified;

        public vec2Type()
        {
            x = 1.0;
            y = 1.0;
            xunits = unitsEnumType.fraction;
            yunits = unitsEnumType.fraction;
        }

        [XmlAttribute(AttributeName = "x", DataType = "double")]
        public double x
        {
            get { return __x; }
            set
            {
                __x = value;
                __xSpecified = true;
            }
        }

        [XmlAttribute(AttributeName = "y", DataType = "double")]
        public double y
        {
            get { return __y; }
            set
            {
                __y = value;
                __ySpecified = true;
            }
        }

        [XmlAttribute(AttributeName = "xunits")]
        public unitsEnumType xunits
        {
            get { return __xunits; }
            set
            {
                __xunits = value;
                __xunitsSpecified = true;
            }
        }

        [XmlAttribute(AttributeName = "yunits")]
        public unitsEnumType yunits
        {
            get { return __yunits; }
            set
            {
                __yunits = value;
                __yunitsSpecified = true;
            }
        }

        public void MakeSchemaCompliant()
        {
        }
    }
}