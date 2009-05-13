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
    [XmlRoot("ImagePyramid", Namespace = "http://www.opengis.net/kml/2.2", IsNullable = false)]
    public class ImagePyramidType : AbstractObjectType
    {
        private gridOriginEnumType gridOriginField;

        private bool gridOriginFieldSpecified;

        private AbstractObjectType[] imagePyramidObjectExtensionGroupField;
        private string[] imagePyramidSimpleExtensionGroupField;
        private int maxHeightField;

        private bool maxHeightFieldSpecified;
        private int maxWidthField;

        private bool maxWidthFieldSpecified;
        private int tileSizeField;

        private bool tileSizeFieldSpecified;

        public ImagePyramidType()
        {
            tileSizeField = 256;
            maxWidthField = 0;
            maxHeightField = 0;
            gridOriginField = gridOriginEnumType.lowerLeft;
        }

        /// <remarks/>
        public int tileSize
        {
            get { return tileSizeField; }
            set { tileSizeField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool tileSizeSpecified
        {
            get { return tileSizeFieldSpecified; }
            set { tileSizeFieldSpecified = value; }
        }

        /// <remarks/>
        public int maxWidth
        {
            get { return maxWidthField; }
            set { maxWidthField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool maxWidthSpecified
        {
            get { return maxWidthFieldSpecified; }
            set { maxWidthFieldSpecified = value; }
        }

        /// <remarks/>
        public int maxHeight
        {
            get { return maxHeightField; }
            set { maxHeightField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool maxHeightSpecified
        {
            get { return maxHeightFieldSpecified; }
            set { maxHeightFieldSpecified = value; }
        }

        /// <remarks/>
        public gridOriginEnumType gridOrigin
        {
            get { return gridOriginField; }
            set { gridOriginField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool gridOriginSpecified
        {
            get { return gridOriginFieldSpecified; }
            set { gridOriginFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement("ImagePyramidSimpleExtensionGroup")]
        public string[] ImagePyramidSimpleExtensionGroup
        {
            get { return imagePyramidSimpleExtensionGroupField; }
            set { imagePyramidSimpleExtensionGroupField = value; }
        }

        /// <remarks/>
        [XmlElement("ImagePyramidObjectExtensionGroup")]
        public AbstractObjectType[] ImagePyramidObjectExtensionGroup
        {
            get { return imagePyramidObjectExtensionGroupField; }
            set { imagePyramidObjectExtensionGroupField = value; }
        }
    }
}