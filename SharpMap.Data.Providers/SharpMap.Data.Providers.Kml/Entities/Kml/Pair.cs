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
    [XmlType(TypeName = "PairType", Namespace = "http://www.opengis.net/kml/2.2")]
    [XmlRoot("Pair", Namespace = "http://www.opengis.net/kml/2.2", IsNullable = false)]
    public class Pair : KmlObjectBase
    {
        private StyleSelectorBase itemField;
        private StyleState keyField;

        private bool keyFieldSpecified;

        private KmlObjectBase[] pairObjectExtensionGroupField;
        private string[] pairSimpleExtensionGroupField;
        private string styleUrlField;

        public Pair()
        {
            keyField = StyleState.Normal;
        }

        /// <remarks/>
        public StyleState key
        {
            get { return keyField; }
            set { keyField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool keySpecified
        {
            get { return keyFieldSpecified; }
            set { keyFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(DataType = "anyURI")]
        public string styleUrl
        {
            get { return styleUrlField; }
            set { styleUrlField = value; }
        }

        /// <remarks/>
        [XmlElement("Style", typeof (Style))]
        [XmlElement("StyleMap", typeof (StyleMap))]
        public StyleSelectorBase Item
        {
            get { return itemField; }
            set { itemField = value; }
        }

        /// <remarks/>
        [XmlElement("PairSimpleExtensionGroup")]
        public string[] PairSimpleExtensionGroup
        {
            get { return pairSimpleExtensionGroupField; }
            set { pairSimpleExtensionGroupField = value; }
        }

        /// <remarks/>
        [XmlElement("PairObjectExtensionGroup")]
        public KmlObjectBase[] PairObjectExtensionGroup
        {
            get { return pairObjectExtensionGroupField; }
            set { pairObjectExtensionGroupField = value; }
        }
    }
}