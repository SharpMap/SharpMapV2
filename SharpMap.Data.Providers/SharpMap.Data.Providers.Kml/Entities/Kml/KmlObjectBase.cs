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
    [XmlInclude(typeof (ImagePyramid))]
    [XmlInclude(typeof (ViewVolume))]
    [XmlInclude(typeof (SchemaData))]
    [XmlInclude(typeof (Alias))]
    [XmlInclude(typeof (ResourceMap))]
    [XmlInclude(typeof (Scale))]
    [XmlInclude(typeof (Orientation))]
    [XmlInclude(typeof (Location))]
    [XmlInclude(typeof (GeometryBase))]
    [XmlInclude(typeof (Model))]
    [XmlInclude(typeof (Polygon))]
    [XmlInclude(typeof (LinearRing))]
    [XmlInclude(typeof (LineString))]
    [XmlInclude(typeof (Point))]
    [XmlInclude(typeof (MultiGeometry))]
    [XmlInclude(typeof (Data))]
    [XmlInclude(typeof (Lod))]
    [XmlInclude(typeof (LatLonBoxBase))]
    [XmlInclude(typeof (LatLonBox))]
    [XmlInclude(typeof (LatLonAltBox))]
    [XmlInclude(typeof (Region))]
    [XmlInclude(typeof (Pair))]
    [XmlInclude(typeof (ItemIconType))]
    [XmlInclude(typeof (BasicLink))]
    [XmlInclude(typeof (LinkType))]
    [XmlInclude(typeof (SubStyleBase))]
    [XmlInclude(typeof (ListStyle))]
    [XmlInclude(typeof (BalloonStyle))]
    [XmlInclude(typeof (ColorStyleBase))]
    [XmlInclude(typeof (PolyStyle))]
    [XmlInclude(typeof (LineStyle))]
    [XmlInclude(typeof (LabelStyle))]
    [XmlInclude(typeof (IconStyle))]
    [XmlInclude(typeof (StyleSelectorBase))]
    [XmlInclude(typeof (StyleMap))]
    [XmlInclude(typeof (Style))]
    [XmlInclude(typeof (TimePrimitiveBase))]
    [XmlInclude(typeof (TimeSpan))]
    [XmlInclude(typeof (TimeStamp))]
    [XmlInclude(typeof (ViewBase))]
    [XmlInclude(typeof (Camera))]
    [XmlInclude(typeof (LookAt))]
    [XmlInclude(typeof (FeatureBase))]
    [XmlInclude(typeof (OverlayBase))]
    [XmlInclude(typeof (PhotoOverlay))]
    [XmlInclude(typeof (ScreenOverlay))]
    [XmlInclude(typeof (GroundOverlay))]
    [XmlInclude(typeof (NetworkLink))]
    [XmlInclude(typeof (Placemark))]
    [XmlInclude(typeof (ContainerBase))]
    [XmlInclude(typeof (Folder))]
    [XmlInclude(typeof (Document))]
    [Serializable]
    [XmlType(TypeName = "AbstractObjectType", Namespace = "http://www.opengis.net/kml/2.2")]
    public abstract class KmlObjectBase
    {
        private string idField;
        private string[] objectSimpleExtensionGroupField;

        private string targetIdField;

        /// <remarks/>
        [XmlElement("ObjectSimpleExtensionGroup")]
        public string[] ObjectSimpleExtensionGroup
        {
            get { return objectSimpleExtensionGroupField; }
            set { objectSimpleExtensionGroupField = value; }
        }

        /// <remarks/>
        [XmlAttribute(DataType = "ID")]
        public string id
        {
            get { return idField; }
            set { idField = value; }
        }

        /// <remarks/>
        [XmlAttribute(DataType = "NCName")]
        public string targetId
        {
            get { return targetIdField; }
            set { targetIdField = value; }
        }
    }
}