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
    [Serializable, XmlType(TypeName = "GridType", Namespace = Declarations.SchemaVersion)]
    public class GridType : AbstractGeometryType
    {
        [XmlIgnore] private string _axisLabels;
        [XmlIgnore] private List<string> _axisName;
        [XmlIgnore] private string _dimension;
        [XmlIgnore] private GridLimitsType _limits;

        public GridType()
        {
            Dimension = string.Empty;
        }

        [XmlElement(ElementName = "axisLabels", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = Declarations.SchemaVersion)]
        public string AxisLabels
        {
            get { return _axisLabels; }
            set { _axisLabels = value; }
        }

        [XmlElement(Type = typeof (string), ElementName = "axisName", IsNullable = false, Form = XmlSchemaForm.Qualified
            , DataType = "string", Namespace = Declarations.SchemaVersion)]
        public List<string> AxisName
        {
            get
            {
                if (_axisName == null)
                {
                    _axisName = new List<string>();
                }
                return _axisName;
            }
            set { _axisName = value; }
        }

        [XmlAttribute(AttributeName = "dimension", DataType = "positiveInteger")]
        public string Dimension
        {
            get { return _dimension; }
            set { _dimension = value; }
        }

        [XmlElement(Type = typeof (GridLimitsType), ElementName = "limits", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public GridLimitsType Limits
        {
            get { return _limits; }
            set { _limits = value; }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
            Limits.MakeSchemaCompliant();
        }
    }
}