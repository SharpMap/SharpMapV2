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