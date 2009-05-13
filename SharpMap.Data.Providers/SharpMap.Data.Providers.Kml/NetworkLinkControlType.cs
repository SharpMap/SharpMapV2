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
    [XmlRoot("NetworkLinkControl", Namespace = "http://www.opengis.net/kml/2.2", IsNullable = false)]
    public class NetworkLinkControlType
    {
        private string cookieField;

        private string expiresField;

        private AbstractViewType itemField;
        private string linkDescriptionField;
        private string linkNameField;
        private SnippetType linkSnippetField;
        private double maxSessionLengthField;

        private bool maxSessionLengthFieldSpecified;
        private string messageField;
        private double minRefreshPeriodField;

        private bool minRefreshPeriodFieldSpecified;

        private AbstractObjectType[] networkLinkControlObjectExtensionGroupField;
        private string[] networkLinkControlSimpleExtensionGroupField;
        private UpdateType updateField;

        public NetworkLinkControlType()
        {
            minRefreshPeriodField = 0;
            maxSessionLengthField = -1;
        }

        /// <remarks/>
        public double minRefreshPeriod
        {
            get { return minRefreshPeriodField; }
            set { minRefreshPeriodField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool minRefreshPeriodSpecified
        {
            get { return minRefreshPeriodFieldSpecified; }
            set { minRefreshPeriodFieldSpecified = value; }
        }

        /// <remarks/>
        public double maxSessionLength
        {
            get { return maxSessionLengthField; }
            set { maxSessionLengthField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool maxSessionLengthSpecified
        {
            get { return maxSessionLengthFieldSpecified; }
            set { maxSessionLengthFieldSpecified = value; }
        }

        /// <remarks/>
        public string cookie
        {
            get { return cookieField; }
            set { cookieField = value; }
        }

        /// <remarks/>
        public string message
        {
            get { return messageField; }
            set { messageField = value; }
        }

        /// <remarks/>
        public string linkName
        {
            get { return linkNameField; }
            set { linkNameField = value; }
        }

        /// <remarks/>
        public string linkDescription
        {
            get { return linkDescriptionField; }
            set { linkDescriptionField = value; }
        }

        /// <remarks/>
        public SnippetType linkSnippet
        {
            get { return linkSnippetField; }
            set { linkSnippetField = value; }
        }

        /// <remarks/>
        public string expires
        {
            get { return expiresField; }
            set { expiresField = value; }
        }

        /// <remarks/>
        public UpdateType Update
        {
            get { return updateField; }
            set { updateField = value; }
        }

        /// <remarks/>
        [XmlElement("Camera", typeof (CameraType))]
        [XmlElement("LookAt", typeof (LookAtType))]
        public AbstractViewType Item
        {
            get { return itemField; }
            set { itemField = value; }
        }

        /// <remarks/>
        [XmlElement("NetworkLinkControlSimpleExtensionGroup")]
        public string[] NetworkLinkControlSimpleExtensionGroup
        {
            get { return networkLinkControlSimpleExtensionGroupField; }
            set { networkLinkControlSimpleExtensionGroupField = value; }
        }

        /// <remarks/>
        [XmlElement("NetworkLinkControlObjectExtensionGroup")]
        public AbstractObjectType[] NetworkLinkControlObjectExtensionGroup
        {
            get { return networkLinkControlObjectExtensionGroupField; }
            set { networkLinkControlObjectExtensionGroupField = value; }
        }
    }
}