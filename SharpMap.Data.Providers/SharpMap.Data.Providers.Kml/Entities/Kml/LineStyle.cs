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
    [XmlType(TypeName = "LineStyleType", Namespace = "http://www.opengis.net/kml/2.2")]
    [XmlRoot("LineStyle", Namespace = "http://www.opengis.net/kml/2.2", IsNullable = false)]
    public class LineStyle : ColorStyleBase
    {
        private KmlObjectBase[] lineStyleObjectExtensionGroupField;
        private string[] lineStyleSimpleExtensionGroupField;
        private double widthField;

        private bool widthFieldSpecified;

        public LineStyle()
        {
            widthField = 1;
        }

        /// <remarks/>
        public double width
        {
            get { return widthField; }
            set { widthField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool widthSpecified
        {
            get { return widthFieldSpecified; }
            set { widthFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement("LineStyleSimpleExtensionGroup")]
        public string[] LineStyleSimpleExtensionGroup
        {
            get { return lineStyleSimpleExtensionGroupField; }
            set { lineStyleSimpleExtensionGroupField = value; }
        }

        /// <remarks/>
        [XmlElement("LineStyleObjectExtensionGroup")]
        public KmlObjectBase[] LineStyleObjectExtensionGroup
        {
            get { return lineStyleObjectExtensionGroupField; }
            set { lineStyleObjectExtensionGroupField = value; }
        }
    }
}