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
    [XmlRoot("Lod", Namespace = "http://www.opengis.net/kml/2.2", IsNullable = false)]
    public class LodType : AbstractObjectType
    {
        private AbstractObjectType[] lodObjectExtensionGroupField;
        private string[] lodSimpleExtensionGroupField;
        private double maxFadeExtentField;

        private bool maxFadeExtentFieldSpecified;
        private double maxLodPixelsField;

        private bool maxLodPixelsFieldSpecified;

        private double minFadeExtentField;

        private bool minFadeExtentFieldSpecified;
        private double minLodPixelsField;

        private bool minLodPixelsFieldSpecified;

        public LodType()
        {
            minLodPixelsField = 0;
            maxLodPixelsField = -1;
            minFadeExtentField = 0;
            maxFadeExtentField = 0;
        }

        /// <remarks/>
        public double minLodPixels
        {
            get { return minLodPixelsField; }
            set { minLodPixelsField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool minLodPixelsSpecified
        {
            get { return minLodPixelsFieldSpecified; }
            set { minLodPixelsFieldSpecified = value; }
        }

        /// <remarks/>
        public double maxLodPixels
        {
            get { return maxLodPixelsField; }
            set { maxLodPixelsField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool maxLodPixelsSpecified
        {
            get { return maxLodPixelsFieldSpecified; }
            set { maxLodPixelsFieldSpecified = value; }
        }

        /// <remarks/>
        public double minFadeExtent
        {
            get { return minFadeExtentField; }
            set { minFadeExtentField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool minFadeExtentSpecified
        {
            get { return minFadeExtentFieldSpecified; }
            set { minFadeExtentFieldSpecified = value; }
        }

        /// <remarks/>
        public double maxFadeExtent
        {
            get { return maxFadeExtentField; }
            set { maxFadeExtentField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool maxFadeExtentSpecified
        {
            get { return maxFadeExtentFieldSpecified; }
            set { maxFadeExtentFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement("LodSimpleExtensionGroup")]
        public string[] LodSimpleExtensionGroup
        {
            get { return lodSimpleExtensionGroupField; }
            set { lodSimpleExtensionGroupField = value; }
        }

        /// <remarks/>
        [XmlElement("LodObjectExtensionGroup")]
        public AbstractObjectType[] LodObjectExtensionGroup
        {
            get { return lodObjectExtensionGroupField; }
            set { lodObjectExtensionGroupField = value; }
        }
    }
}