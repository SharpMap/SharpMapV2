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
    [XmlRoot("Icon", Namespace = "http://www.opengis.net/kml/2.2", IsNullable = false)]
    public class LinkType : BasicLinkType
    {
        private string httpQueryField;
        private AbstractObjectType[] linkObjectExtensionGroupField;
        private string[] linkSimpleExtensionGroupField;
        private double refreshIntervalField;

        private bool refreshIntervalFieldSpecified;
        private refreshModeEnumType refreshModeField;

        private bool refreshModeFieldSpecified;
        private double viewBoundScaleField;

        private bool viewBoundScaleFieldSpecified;

        private string viewFormatField;

        private viewRefreshModeEnumType viewRefreshModeField;

        private bool viewRefreshModeFieldSpecified;

        private double viewRefreshTimeField;

        private bool viewRefreshTimeFieldSpecified;

        public LinkType()
        {
            refreshModeField = refreshModeEnumType.onChange;
            refreshIntervalField = 4;
            viewRefreshModeField = viewRefreshModeEnumType.never;
            viewRefreshTimeField = 4;
            viewBoundScaleField = 1;
        }

        /// <remarks/>
        public refreshModeEnumType refreshMode
        {
            get { return refreshModeField; }
            set { refreshModeField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool refreshModeSpecified
        {
            get { return refreshModeFieldSpecified; }
            set { refreshModeFieldSpecified = value; }
        }

        /// <remarks/>
        public double refreshInterval
        {
            get { return refreshIntervalField; }
            set { refreshIntervalField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool refreshIntervalSpecified
        {
            get { return refreshIntervalFieldSpecified; }
            set { refreshIntervalFieldSpecified = value; }
        }

        /// <remarks/>
        public viewRefreshModeEnumType viewRefreshMode
        {
            get { return viewRefreshModeField; }
            set { viewRefreshModeField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool viewRefreshModeSpecified
        {
            get { return viewRefreshModeFieldSpecified; }
            set { viewRefreshModeFieldSpecified = value; }
        }

        /// <remarks/>
        public double viewRefreshTime
        {
            get { return viewRefreshTimeField; }
            set { viewRefreshTimeField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool viewRefreshTimeSpecified
        {
            get { return viewRefreshTimeFieldSpecified; }
            set { viewRefreshTimeFieldSpecified = value; }
        }

        /// <remarks/>
        public double viewBoundScale
        {
            get { return viewBoundScaleField; }
            set { viewBoundScaleField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool viewBoundScaleSpecified
        {
            get { return viewBoundScaleFieldSpecified; }
            set { viewBoundScaleFieldSpecified = value; }
        }

        /// <remarks/>
        public string viewFormat
        {
            get { return viewFormatField; }
            set { viewFormatField = value; }
        }

        /// <remarks/>
        public string httpQuery
        {
            get { return httpQueryField; }
            set { httpQueryField = value; }
        }

        /// <remarks/>
        [XmlElement("LinkSimpleExtensionGroup")]
        public string[] LinkSimpleExtensionGroup
        {
            get { return linkSimpleExtensionGroupField; }
            set { linkSimpleExtensionGroupField = value; }
        }

        /// <remarks/>
        [XmlElement("LinkObjectExtensionGroup")]
        public AbstractObjectType[] LinkObjectExtensionGroup
        {
            get { return linkObjectExtensionGroupField; }
            set { linkObjectExtensionGroupField = value; }
        }
    }
}