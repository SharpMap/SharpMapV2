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
    [XmlType(TypeName = "StyleType", Namespace = "http://www.opengis.net/kml/2.2")]
    [XmlRoot("Style", Namespace = "http://www.opengis.net/kml/2.2", IsNullable = false)]
    public class Style : StyleSelectorBase
    {
        private BalloonStyle balloonStyleField;
        private IconStyle iconStyleField;

        private LabelStyle labelStyleField;

        private LineStyle lineStyleField;

        private ListStyle listStyleField;
        private PolyStyle polyStyleField;

        private KmlObjectBase[] styleObjectExtensionGroupField;
        private string[] styleSimpleExtensionGroupField;

        /// <remarks/>
        public IconStyle IconStyle
        {
            get { return iconStyleField; }
            set { iconStyleField = value; }
        }

        /// <remarks/>
        public LabelStyle LabelStyle
        {
            get { return labelStyleField; }
            set { labelStyleField = value; }
        }

        /// <remarks/>
        public LineStyle LineStyle
        {
            get { return lineStyleField; }
            set { lineStyleField = value; }
        }

        /// <remarks/>
        public PolyStyle PolyStyle
        {
            get { return polyStyleField; }
            set { polyStyleField = value; }
        }

        /// <remarks/>
        public BalloonStyle BalloonStyle
        {
            get { return balloonStyleField; }
            set { balloonStyleField = value; }
        }

        /// <remarks/>
        public ListStyle ListStyle
        {
            get { return listStyleField; }
            set { listStyleField = value; }
        }

        /// <remarks/>
        [XmlElement("StyleSimpleExtensionGroup")]
        public string[] StyleSimpleExtensionGroup
        {
            get { return styleSimpleExtensionGroupField; }
            set { styleSimpleExtensionGroupField = value; }
        }

        /// <remarks/>
        [XmlElement("StyleObjectExtensionGroup")]
        public KmlObjectBase[] StyleObjectExtensionGroup
        {
            get { return styleObjectExtensionGroupField; }
            set { styleObjectExtensionGroupField = value; }
        }
    }
}