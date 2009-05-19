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
    [Serializable, XmlType(TypeName = "TinType", Namespace = "http://www.opengis.net/gml/3.2")]
    public class TinType : SurfaceType
    {
        [XmlIgnore] private List<LineStringSegmentArrayPropertyType> _breakLines;
        [XmlIgnore] private ControlPoint _controlPoint;
        [XmlIgnore] private LengthType _maxLength;
        [XmlIgnore] private List<LineStringSegmentArrayPropertyType> _stopLines;

        [XmlElement(Type = typeof (LineStringSegmentArrayPropertyType), ElementName = "breakLines", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public List<LineStringSegmentArrayPropertyType> BreakLines
        {
            get
            {
                if (_breakLines == null)
                {
                    _breakLines = new List<LineStringSegmentArrayPropertyType>();
                }
                return _breakLines;
            }
            set { _breakLines = value; }
        }

        [XmlElement(Type = typeof (ControlPoint), ElementName = "controlPoint", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public ControlPoint ControlPoint
        {
            get { return _controlPoint; }
            set { _controlPoint = value; }
        }

        [XmlElement(Type = typeof (LengthType), ElementName = "maxLength", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public LengthType MaxLength
        {
            get { return _maxLength; }
            set { _maxLength = value; }
        }

        [XmlElement(Type = typeof (LineStringSegmentArrayPropertyType), ElementName = "stopLines", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public List<LineStringSegmentArrayPropertyType> StopLines
        {
            get
            {
                if (_stopLines == null)
                {
                    _stopLines = new List<LineStringSegmentArrayPropertyType>();
                }
                return _stopLines;
            }
            set { _stopLines = value; }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
            MaxLength.MakeSchemaCompliant();
            ControlPoint.MakeSchemaCompliant();
        }
    }
}