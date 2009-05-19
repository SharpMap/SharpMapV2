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
    [Serializable, XmlType(TypeName = "CoordinateSystemAxisType", Namespace = "http://www.opengis.net/gml/3.2")]
    public class CoordinateSystemAxisType : IdentifiedObjectType
    {
        [XmlIgnore] private AxisAbbrev _axisAbbrev;
        [XmlIgnore] private AxisDirection _axisDirection;
        [XmlIgnore] private double _maximumValue;
        [XmlIgnore] private double _minimumValue;
        [XmlIgnore] private RangeMeaning _rangeMeaning;
        [XmlIgnore] private string _uom;
        [XmlIgnore] public bool MaximumValueSpecified;
        [XmlIgnore] public bool MinimumValueSpecified;

        [XmlElement(Type = typeof (AxisAbbrev), ElementName = "axisAbbrev", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public AxisAbbrev AxisAbbrev
        {
            get { return _axisAbbrev; }
            set { _axisAbbrev = value; }
        }

        [XmlElement(Type = typeof (AxisDirection), ElementName = "axisDirection", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public AxisDirection AxisDirection
        {
            get { return _axisDirection; }
            set { _axisDirection = value; }
        }

        [XmlElement(ElementName = "maximumValue", IsNullable = false, Form = XmlSchemaForm.Qualified,
            DataType = "double", Namespace = "http://www.opengis.net/gml/3.2")]
        public double MaximumValue
        {
            get { return _maximumValue; }
            set
            {
                _maximumValue = value;
                MaximumValueSpecified = true;
            }
        }

        [XmlElement(ElementName = "minimumValue", IsNullable = false, Form = XmlSchemaForm.Qualified,
            DataType = "double", Namespace = "http://www.opengis.net/gml/3.2")]
        public double MinimumValue
        {
            get { return _minimumValue; }
            set
            {
                _minimumValue = value;
                MinimumValueSpecified = true;
            }
        }

        [XmlElement(Type = typeof (RangeMeaning), ElementName = "rangeMeaning", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public RangeMeaning RangeMeaning
        {
            get { return _rangeMeaning; }
            set { _rangeMeaning = value; }
        }

        [XmlAttribute(AttributeName = "uom")]
        public string Uom
        {
            get { return _uom; }
            set { _uom = value; }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
            AxisAbbrev.MakeSchemaCompliant();
            AxisDirection.MakeSchemaCompliant();
        }
    }
}