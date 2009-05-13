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
    /// <remarks/>
    [Serializable]
    [XmlType(TypeName = "MultiGeometryType", Namespace = "http://www.opengis.net/kml/2.2")]
    [XmlRoot("MultiGeometry", Namespace = "http://www.opengis.net/kml/2.2", IsNullable = false)]
    public class MultiGeometry : GeometryBase
    {
        private GeometryBase[] itemsField;

        private KmlObjectBase[] multiGeometryObjectExtensionGroupField;
        private string[] multiGeometrySimpleExtensionGroupField;

        /// <remarks/>
        [XmlElement("LineString", typeof (LineString))]
        [XmlElement("LinearRing", typeof (LinearRing))]
        [XmlElement("Model", typeof (Model))]
        [XmlElement("MultiGeometry", typeof (MultiGeometry))]
        [XmlElement("Point", typeof (Point))]
        [XmlElement("Polygon", typeof (Polygon))]
        public GeometryBase[] Items
        {
            get { return itemsField; }
            set { itemsField = value; }
        }

        /// <remarks/>
        [XmlElement("MultiGeometrySimpleExtensionGroup")]
        public string[] MultiGeometrySimpleExtensionGroup
        {
            get { return multiGeometrySimpleExtensionGroupField; }
            set { multiGeometrySimpleExtensionGroupField = value; }
        }

        /// <remarks/>
        [XmlElement("MultiGeometryObjectExtensionGroup")]
        public KmlObjectBase[] MultiGeometryObjectExtensionGroup
        {
            get { return multiGeometryObjectExtensionGroupField; }
            set { multiGeometryObjectExtensionGroupField = value; }
        }
    }
}