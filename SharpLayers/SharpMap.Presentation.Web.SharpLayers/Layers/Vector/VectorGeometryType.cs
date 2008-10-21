/*
 *  The attached / following is part of SharpMap.Presentation.Web.SharpLayers
 *  SharpMap.Presentation.Web.SharpLayers is free software © 2008 Newgrove Consultants Limited, 
 *  www.newgrove.com; you can redistribute it and/or modify it under the terms 
 *  of the current GNU Lesser General Public License (LGPL) as published by and 
 *  available from the Free Software Foundation, Inc., 
 *  59 Temple Place, Suite 330, Boston, MA 02111-1307 USA: http://fsf.org/    
 *  This program is distributed without any warranty; 
 *  without even the implied warranty of merchantability or fitness for purpose.  
 *  See the GNU Lesser General Public License for the full details. 
 *  
 *  Author: John Diss 2008
 * 
 */
namespace SharpMap.Presentation.Web.SharpLayers.Layers.Vector
{
    public struct VectorGeometryType
    {
        private string _type;

        private VectorGeometryType(string type)
        {
            _type = type;
        }

        public static VectorGeometryType Point
        {
            get { return new VectorGeometryType("OpenLayers.Geometry.Point"); }
        }

        public static VectorGeometryType Collection
        {
            get { return new VectorGeometryType("OpenLayers.Geometry.Collection"); }
        }

        public static VectorGeometryType LineString
        {
            get { return new VectorGeometryType("OpenLayers.Geometry.LineString"); }
        }

        public static VectorGeometryType MultiLineString
        {
            get { return new VectorGeometryType("OpenLayers.Geometry.MultiLineString"); }
        }

        public static VectorGeometryType MultiPoint
        {
            get { return new VectorGeometryType("OpenLayers.Geometry.MultiPoint"); }
        }

        public static VectorGeometryType MultiPolygon
        {
            get { return new VectorGeometryType("OpenLayers.Geometry.MultiPolygon"); }
        }

        public static VectorGeometryType Polygon
        {
            get { return new VectorGeometryType("Openlayers.Geometry.Polygon"); }
        }

        public static VectorGeometryType Rectangle
        {
            get { return new VectorGeometryType("OpenLayers.Geometry.Rectangle"); }
        }
    }
}