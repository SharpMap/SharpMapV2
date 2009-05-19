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
    [Serializable, XmlType(TypeName = "ClothoidType", Namespace = "http://www.opengis.net/gml/3.2")]
    public class ClothoidType : AbstractCurveSegmentType
    {
        [XmlIgnore] private double _endParameter;
        [XmlIgnore] private CurveInterpolationType _interpolation;
        [XmlIgnore] private RefLocation _refLocation;
        [XmlIgnore] private decimal _scaleFactor;
        [XmlIgnore] private double _startParameter;
        [XmlIgnore] public bool EndParameterSpecified;
        [XmlIgnore] public bool InterpolationSpecified;
        [XmlIgnore] public bool ScaleFactorSpecified;
        [XmlIgnore] public bool StartParameterSpecified;

        public ClothoidType()
        {
            Interpolation = CurveInterpolationType.Clothoid;
            ScaleFactorSpecified = true;
            StartParameterSpecified = true;
            EndParameterSpecified = true;
        }

        [XmlElement(ElementName = "endParameter", IsNullable = false, Form = XmlSchemaForm.Qualified,
            DataType = "double", Namespace = "http://www.opengis.net/gml/3.2")]
        public double EndParameter
        {
            get { return _endParameter; }
            set
            {
                _endParameter = value;
                EndParameterSpecified = true;
            }
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

        [XmlElement(Type = typeof (RefLocation), ElementName = "refLocation", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public RefLocation RefLocation
        {
            get { return _refLocation; }
            set { _refLocation = value; }
        }

        [XmlElement(ElementName = "scaleFactor", IsNullable = false, Form = XmlSchemaForm.Qualified,
            DataType = "decimal", Namespace = "http://www.opengis.net/gml/3.2")]
        public decimal ScaleFactor
        {
            get { return _scaleFactor; }
            set
            {
                _scaleFactor = value;
                ScaleFactorSpecified = true;
            }
        }

        [XmlElement(ElementName = "startParameter", IsNullable = false, Form = XmlSchemaForm.Qualified,
            DataType = "double", Namespace = "http://www.opengis.net/gml/3.2")]
        public double StartParameter
        {
            get { return _startParameter; }
            set
            {
                _startParameter = value;
                StartParameterSpecified = true;
            }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
            RefLocation.MakeSchemaCompliant();
        }
    }
}