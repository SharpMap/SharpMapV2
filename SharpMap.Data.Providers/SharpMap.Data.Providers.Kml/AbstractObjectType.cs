using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Ogc.Kml
{
    /// <remarks/>
    [XmlInclude(typeof (ImagePyramidType))]
    [XmlInclude(typeof (ViewVolumeType))]
    [XmlInclude(typeof (SchemaDataType))]
    [XmlInclude(typeof (AliasType))]
    [XmlInclude(typeof (ResourceMapType))]
    [XmlInclude(typeof (ScaleType))]
    [XmlInclude(typeof (OrientationType))]
    [XmlInclude(typeof (LocationType))]
    [XmlInclude(typeof (AbstractGeometryType))]
    [XmlInclude(typeof (ModelType))]
    [XmlInclude(typeof (PolygonType))]
    [XmlInclude(typeof (LinearRingType))]
    [XmlInclude(typeof (LineStringType))]
    [XmlInclude(typeof (PointType))]
    [XmlInclude(typeof (MultiGeometryType))]
    [XmlInclude(typeof (DataType))]
    [XmlInclude(typeof (LodType))]
    [XmlInclude(typeof (AbstractLatLonBoxType))]
    [XmlInclude(typeof (LatLonBoxType))]
    [XmlInclude(typeof (LatLonAltBoxType))]
    [XmlInclude(typeof (RegionType))]
    [XmlInclude(typeof (PairType))]
    [XmlInclude(typeof (ItemIconType))]
    [XmlInclude(typeof (BasicLinkType))]
    [XmlInclude(typeof (LinkType))]
    [XmlInclude(typeof (AbstractSubStyleType))]
    [XmlInclude(typeof (ListStyleType))]
    [XmlInclude(typeof (BalloonStyleType))]
    [XmlInclude(typeof (AbstractColorStyleType))]
    [XmlInclude(typeof (PolyStyleType))]
    [XmlInclude(typeof (LineStyleType))]
    [XmlInclude(typeof (LabelStyleType))]
    [XmlInclude(typeof (IconStyleType))]
    [XmlInclude(typeof (AbstractStyleSelectorType))]
    [XmlInclude(typeof (StyleMapType))]
    [XmlInclude(typeof (StyleType))]
    [XmlInclude(typeof (AbstractTimePrimitiveType))]
    [XmlInclude(typeof (TimeSpanType))]
    [XmlInclude(typeof (TimeStampType))]
    [XmlInclude(typeof (AbstractViewType))]
    [XmlInclude(typeof (CameraType))]
    [XmlInclude(typeof (LookAtType))]
    [XmlInclude(typeof (AbstractFeatureType))]
    [XmlInclude(typeof (AbstractOverlayType))]
    [XmlInclude(typeof (PhotoOverlayType))]
    [XmlInclude(typeof (ScreenOverlayType))]
    [XmlInclude(typeof (GroundOverlayType))]
    [XmlInclude(typeof (NetworkLinkType))]
    [XmlInclude(typeof (PlacemarkType))]
    [XmlInclude(typeof (AbstractContainerType))]
    [XmlInclude(typeof (FolderType))]
    [XmlInclude(typeof (DocumentType))]
    [GeneratedCode("xsd", "2.0.50727.3038")]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://www.opengis.net/kml/2.2")]
    public abstract class AbstractObjectType
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