using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Ogc.Kml
{
    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.3038")]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://www.opengis.net/kml/2.2")]
    [XmlRoot("PhotoOverlay", Namespace = "http://www.opengis.net/kml/2.2", IsNullable = false)]
    public class PhotoOverlayType : AbstractOverlayType
    {
        private ImagePyramidType imagePyramidField;
        private AbstractObjectType[] photoOverlayObjectExtensionGroupField;
        private string[] photoOverlaySimpleExtensionGroupField;

        private PointType pointField;
        private double rotationField;

        private bool rotationFieldSpecified;

        private shapeEnumType shapeField;

        private bool shapeFieldSpecified;
        private ViewVolumeType viewVolumeField;

        public PhotoOverlayType()
        {
            rotationField = 0;
            shapeField = shapeEnumType.rectangle;
        }

        /// <remarks/>
        public double rotation
        {
            get { return rotationField; }
            set { rotationField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool rotationSpecified
        {
            get { return rotationFieldSpecified; }
            set { rotationFieldSpecified = value; }
        }

        /// <remarks/>
        public ViewVolumeType ViewVolume
        {
            get { return viewVolumeField; }
            set { viewVolumeField = value; }
        }

        /// <remarks/>
        public ImagePyramidType ImagePyramid
        {
            get { return imagePyramidField; }
            set { imagePyramidField = value; }
        }

        /// <remarks/>
        public PointType Point
        {
            get { return pointField; }
            set { pointField = value; }
        }

        /// <remarks/>
        public shapeEnumType shape
        {
            get { return shapeField; }
            set { shapeField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool shapeSpecified
        {
            get { return shapeFieldSpecified; }
            set { shapeFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement("PhotoOverlaySimpleExtensionGroup")]
        public string[] PhotoOverlaySimpleExtensionGroup
        {
            get { return photoOverlaySimpleExtensionGroupField; }
            set { photoOverlaySimpleExtensionGroupField = value; }
        }

        /// <remarks/>
        [XmlElement("PhotoOverlayObjectExtensionGroup")]
        public AbstractObjectType[] PhotoOverlayObjectExtensionGroup
        {
            get { return photoOverlayObjectExtensionGroupField; }
            set { photoOverlayObjectExtensionGroupField = value; }
        }
    }
}