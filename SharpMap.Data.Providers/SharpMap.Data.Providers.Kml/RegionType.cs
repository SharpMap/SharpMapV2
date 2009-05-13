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
    [XmlRoot("Region", Namespace = "http://www.opengis.net/kml/2.2", IsNullable = false)]
    public class RegionType : AbstractObjectType
    {
        private LatLonAltBoxType latLonAltBoxField;

        private LodType lodField;

        private AbstractObjectType[] regionObjectExtensionGroupField;
        private string[] regionSimpleExtensionGroupField;

        /// <remarks/>
        public LatLonAltBoxType LatLonAltBox
        {
            get { return latLonAltBoxField; }
            set { latLonAltBoxField = value; }
        }

        /// <remarks/>
        public LodType Lod
        {
            get { return lodField; }
            set { lodField = value; }
        }

        /// <remarks/>
        [XmlElement("RegionSimpleExtensionGroup")]
        public string[] RegionSimpleExtensionGroup
        {
            get { return regionSimpleExtensionGroupField; }
            set { regionSimpleExtensionGroupField = value; }
        }

        /// <remarks/>
        [XmlElement("RegionObjectExtensionGroup")]
        public AbstractObjectType[] RegionObjectExtensionGroup
        {
            get { return regionObjectExtensionGroupField; }
            set { regionObjectExtensionGroupField = value; }
        }
    }
}