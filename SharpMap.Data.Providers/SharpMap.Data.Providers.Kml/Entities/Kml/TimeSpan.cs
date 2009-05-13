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
    [XmlType(TypeName = "TimeSpanType", Namespace = "http://www.opengis.net/kml/2.2")]
    [XmlRoot("TimeSpan", Namespace = "http://www.opengis.net/kml/2.2", IsNullable = false)]
    public class TimeSpan : TimePrimitiveBase
    {
        private string beginField;

        private string endField;

        private KmlObjectBase[] timeSpanObjectExtensionGroupField;
        private string[] timeSpanSimpleExtensionGroupField;

        /// <remarks/>
        public string begin
        {
            get { return beginField; }
            set { beginField = value; }
        }

        /// <remarks/>
        public string end
        {
            get { return endField; }
            set { endField = value; }
        }

        /// <remarks/>
        [XmlElement("TimeSpanSimpleExtensionGroup")]
        public string[] TimeSpanSimpleExtensionGroup
        {
            get { return timeSpanSimpleExtensionGroupField; }
            set { timeSpanSimpleExtensionGroupField = value; }
        }

        /// <remarks/>
        [XmlElement("TimeSpanObjectExtensionGroup")]
        public KmlObjectBase[] TimeSpanObjectExtensionGroup
        {
            get { return timeSpanObjectExtensionGroupField; }
            set { timeSpanObjectExtensionGroupField = value; }
        }
    }
}