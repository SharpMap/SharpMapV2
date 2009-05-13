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
    [XmlRoot("LatLonBox", Namespace = "http://www.opengis.net/kml/2.2", IsNullable = false)]
    public class LatLonBoxType : AbstractLatLonBoxType
    {
        private AbstractObjectType[] latLonBoxObjectExtensionGroupField;
        private string[] latLonBoxSimpleExtensionGroupField;
        private double rotationField;

        private bool rotationFieldSpecified;

        public LatLonBoxType()
        {
            rotationField = 0;
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
        [XmlElement("LatLonBoxSimpleExtensionGroup")]
        public string[] LatLonBoxSimpleExtensionGroup
        {
            get { return latLonBoxSimpleExtensionGroupField; }
            set { latLonBoxSimpleExtensionGroupField = value; }
        }

        /// <remarks/>
        [XmlElement("LatLonBoxObjectExtensionGroup")]
        public AbstractObjectType[] LatLonBoxObjectExtensionGroup
        {
            get { return latLonBoxObjectExtensionGroupField; }
            set { latLonBoxObjectExtensionGroupField = value; }
        }
    }
}