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
using System.Xml.Serialization;

namespace SharpMap.Entities.Ogc.Gml
{
    [Serializable, XmlType(TypeName = "DirectPositionListType", Namespace = "http://www.opengis.net/gml/3.2")]
    public class DirectPositionListType
    {
        [XmlIgnore] private string _axisLabels;
        [XmlIgnore] private string _count;
        [XmlIgnore] private string _srsDimension;
        [XmlIgnore] private string _srsName;
        [XmlIgnore] private string _uomLabels;
        [XmlIgnore] private string _value;

        [XmlAttribute(AttributeName = "axisLabels")]
        public string AxisLabels
        {
            get { return _axisLabels; }
            set { _axisLabels = value; }
        }

        [XmlAttribute(AttributeName = "count", DataType = "positiveInteger")]
        public string Count
        {
            get { return _count; }
            set { _count = value; }
        }

        [XmlAttribute(AttributeName = "srsDimension", DataType = "positiveInteger")]
        public string SrsDimension
        {
            get { return _srsDimension; }
            set { _srsDimension = value; }
        }

        [XmlAttribute(AttributeName = "srsName", DataType = "anyURI")]
        public string SrsName
        {
            get { return _srsName; }
            set { _srsName = value; }
        }

        [XmlAttribute(AttributeName = "uomLabels")]
        public string UomLabels
        {
            get { return _uomLabels; }
            set { _uomLabels = value; }
        }

        [XmlText(DataType = "string")]
        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public virtual void MakeSchemaCompliant()
        {
        }
    }
}