// /*
//  *  The attached / following is part of SharpMap.Data.Providers.Kml
//  *  SharpMap.Data.Providers.Kml is free software © 2008 Newgrove Consultants Limited, 
//  *  www.newgrove.com; you can redistribute it and/or modify it under the terms 
//  *  of the current GNU Lesser General Public License (LGPL) as published by and 
//  *  available from the Free Software Foundation, Inc., 
//  *  59 Temple Place, Suite 330, Boston, MA 02111-1307 USA: http://fsf.org/    
//  *  This program is distributed without any warranty; 
//  *  without even the implied warranty of merchantability or fitness for purpose.  
//  *  See the GNU Lesser General Public License for the full details. 
//  *  
//  *  Author: John Diss 2009
//  * 
//  */
using System;
using System.Xml.Serialization;

namespace SharpMap.Entities.Ogc.Kml
{
    /// <remarks/>
    [Serializable]
    [XmlType(TypeName = "NetworkLinkControlType", Namespace = "http://www.opengis.net/kml/2.2")]
    [XmlRoot("NetworkLinkControl", Namespace = "http://www.opengis.net/kml/2.2", IsNullable = false)]
    public class NetworkLinkControl
    {
        private string cookieField;

        private string expiresField;

        private ViewBase itemField;
        private string linkDescriptionField;
        private string linkNameField;
        private Snippet linkSnippetField;
        private double maxSessionLengthField;

        private bool maxSessionLengthFieldSpecified;
        private string messageField;
        private double minRefreshPeriodField;

        private bool minRefreshPeriodFieldSpecified;

        private KmlObjectBase[] networkLinkControlObjectExtensionGroupField;
        private string[] networkLinkControlSimpleExtensionGroupField;
        private Update updateField;

        public NetworkLinkControl()
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
        public Snippet linkSnippet
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
        public Update Update
        {
            get { return updateField; }
            set { updateField = value; }
        }

        /// <remarks/>
        [XmlElement("Camera", typeof (Camera))]
        [XmlElement("LookAt", typeof (LookAt))]
        public ViewBase Item
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
        public KmlObjectBase[] NetworkLinkControlObjectExtensionGroup
        {
            get { return networkLinkControlObjectExtensionGroupField; }
            set { networkLinkControlObjectExtensionGroupField = value; }
        }
    }
}