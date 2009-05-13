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
    [XmlType(TypeName = "ScaleType", Namespace = "http://www.opengis.net/kml/2.2")]
    [XmlRoot("Scale", Namespace = "http://www.opengis.net/kml/2.2", IsNullable = false)]
    public class Scale : KmlObjectBase
    {
        private KmlObjectBase[] scaleObjectExtensionGroupField;
        private string[] scaleSimpleExtensionGroupField;
        private double xField;

        private bool xFieldSpecified;

        private double yField;

        private bool yFieldSpecified;

        private double zField;

        private bool zFieldSpecified;

        public Scale()
        {
            xField = 1;
            yField = 1;
            zField = 1;
        }

        /// <remarks/>
        public double x
        {
            get { return xField; }
            set { xField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool xSpecified
        {
            get { return xFieldSpecified; }
            set { xFieldSpecified = value; }
        }

        /// <remarks/>
        public double y
        {
            get { return yField; }
            set { yField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool ySpecified
        {
            get { return yFieldSpecified; }
            set { yFieldSpecified = value; }
        }

        /// <remarks/>
        public double z
        {
            get { return zField; }
            set { zField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool zSpecified
        {
            get { return zFieldSpecified; }
            set { zFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement("ScaleSimpleExtensionGroup")]
        public string[] ScaleSimpleExtensionGroup
        {
            get { return scaleSimpleExtensionGroupField; }
            set { scaleSimpleExtensionGroupField = value; }
        }

        /// <remarks/>
        [XmlElement("ScaleObjectExtensionGroup")]
        public KmlObjectBase[] ScaleObjectExtensionGroup
        {
            get { return scaleObjectExtensionGroupField; }
            set { scaleObjectExtensionGroupField = value; }
        }
    }
}