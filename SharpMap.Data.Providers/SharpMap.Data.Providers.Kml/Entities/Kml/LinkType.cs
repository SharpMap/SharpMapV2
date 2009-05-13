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
    [XmlType(Namespace = "http://www.opengis.net/kml/2.2")]
    [XmlRoot("Icon", Namespace = "http://www.opengis.net/kml/2.2", IsNullable = false)]
    public class LinkType : BasicLink
    {
        private string httpQueryField;
        private KmlObjectBase[] linkObjectExtensionGroupField;
        private string[] linkSimpleExtensionGroupField;
        private double refreshIntervalField;

        private bool refreshIntervalFieldSpecified;
        private RefreshMode refreshModeField;

        private bool refreshModeFieldSpecified;
        private double viewBoundScaleField;

        private bool viewBoundScaleFieldSpecified;

        private string viewFormatField;

        private ViewRefreshMode viewRefreshModeField;

        private bool viewRefreshModeFieldSpecified;

        private double viewRefreshTimeField;

        private bool viewRefreshTimeFieldSpecified;

        public LinkType()
        {
            refreshModeField = RefreshMode.OnChange;
            refreshIntervalField = 4;
            viewRefreshModeField = ViewRefreshMode.Never;
            viewRefreshTimeField = 4;
            viewBoundScaleField = 1;
        }

        /// <remarks/>
        public RefreshMode refreshMode
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
        public ViewRefreshMode viewRefreshMode
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
        public KmlObjectBase[] LinkObjectExtensionGroup
        {
            get { return linkObjectExtensionGroupField; }
            set { linkObjectExtensionGroupField = value; }
        }
    }
}