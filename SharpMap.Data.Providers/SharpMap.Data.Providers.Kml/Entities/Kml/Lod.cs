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
    [XmlType(TypeName = "LodType", Namespace = "http://www.opengis.net/kml/2.2")]
    [XmlRoot("Lod", Namespace = "http://www.opengis.net/kml/2.2", IsNullable = false)]
    public class Lod : KmlObjectBase
    {
        private KmlObjectBase[] lodObjectExtensionGroupField;
        private string[] lodSimpleExtensionGroupField;
        private double maxFadeExtentField;

        private bool maxFadeExtentFieldSpecified;
        private double maxLodPixelsField;

        private bool maxLodPixelsFieldSpecified;

        private double minFadeExtentField;

        private bool minFadeExtentFieldSpecified;
        private double minLodPixelsField;

        private bool minLodPixelsFieldSpecified;

        public Lod()
        {
            minLodPixelsField = 0;
            maxLodPixelsField = -1;
            minFadeExtentField = 0;
            maxFadeExtentField = 0;
        }

        /// <remarks/>
        public double minLodPixels
        {
            get { return minLodPixelsField; }
            set { minLodPixelsField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool minLodPixelsSpecified
        {
            get { return minLodPixelsFieldSpecified; }
            set { minLodPixelsFieldSpecified = value; }
        }

        /// <remarks/>
        public double maxLodPixels
        {
            get { return maxLodPixelsField; }
            set { maxLodPixelsField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool maxLodPixelsSpecified
        {
            get { return maxLodPixelsFieldSpecified; }
            set { maxLodPixelsFieldSpecified = value; }
        }

        /// <remarks/>
        public double minFadeExtent
        {
            get { return minFadeExtentField; }
            set { minFadeExtentField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool minFadeExtentSpecified
        {
            get { return minFadeExtentFieldSpecified; }
            set { minFadeExtentFieldSpecified = value; }
        }

        /// <remarks/>
        public double maxFadeExtent
        {
            get { return maxFadeExtentField; }
            set { maxFadeExtentField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool maxFadeExtentSpecified
        {
            get { return maxFadeExtentFieldSpecified; }
            set { maxFadeExtentFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement("LodSimpleExtensionGroup")]
        public string[] LodSimpleExtensionGroup
        {
            get { return lodSimpleExtensionGroupField; }
            set { lodSimpleExtensionGroupField = value; }
        }

        /// <remarks/>
        [XmlElement("LodObjectExtensionGroup")]
        public KmlObjectBase[] LodObjectExtensionGroup
        {
            get { return lodObjectExtensionGroupField; }
            set { lodObjectExtensionGroupField = value; }
        }
    }
}