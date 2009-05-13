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
    [XmlRoot("ViewVolume", Namespace = "http://www.opengis.net/kml/2.2", IsNullable = false)]
    public class ViewVolumeType : AbstractObjectType
    {
        private double bottomFovField;

        private bool bottomFovFieldSpecified;
        private double leftFovField;

        private bool leftFovFieldSpecified;
        private double nearField;

        private bool nearFieldSpecified;

        private double rightFovField;

        private bool rightFovFieldSpecified;

        private double topFovField;

        private bool topFovFieldSpecified;

        private AbstractObjectType[] viewVolumeObjectExtensionGroupField;
        private string[] viewVolumeSimpleExtensionGroupField;

        public ViewVolumeType()
        {
            leftFovField = 0;
            rightFovField = 0;
            bottomFovField = 0;
            topFovField = 0;
            nearField = 0;
        }

        /// <remarks/>
        public double leftFov
        {
            get { return leftFovField; }
            set { leftFovField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool leftFovSpecified
        {
            get { return leftFovFieldSpecified; }
            set { leftFovFieldSpecified = value; }
        }

        /// <remarks/>
        public double rightFov
        {
            get { return rightFovField; }
            set { rightFovField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool rightFovSpecified
        {
            get { return rightFovFieldSpecified; }
            set { rightFovFieldSpecified = value; }
        }

        /// <remarks/>
        public double bottomFov
        {
            get { return bottomFovField; }
            set { bottomFovField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool bottomFovSpecified
        {
            get { return bottomFovFieldSpecified; }
            set { bottomFovFieldSpecified = value; }
        }

        /// <remarks/>
        public double topFov
        {
            get { return topFovField; }
            set { topFovField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool topFovSpecified
        {
            get { return topFovFieldSpecified; }
            set { topFovFieldSpecified = value; }
        }

        /// <remarks/>
        public double near
        {
            get { return nearField; }
            set { nearField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool nearSpecified
        {
            get { return nearFieldSpecified; }
            set { nearFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement("ViewVolumeSimpleExtensionGroup")]
        public string[] ViewVolumeSimpleExtensionGroup
        {
            get { return viewVolumeSimpleExtensionGroupField; }
            set { viewVolumeSimpleExtensionGroupField = value; }
        }

        /// <remarks/>
        [XmlElement("ViewVolumeObjectExtensionGroup")]
        public AbstractObjectType[] ViewVolumeObjectExtensionGroup
        {
            get { return viewVolumeObjectExtensionGroupField; }
            set { viewVolumeObjectExtensionGroupField = value; }
        }
    }
}