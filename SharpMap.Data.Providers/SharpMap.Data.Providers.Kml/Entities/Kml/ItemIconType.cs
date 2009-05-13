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
    [XmlRoot("ItemIcon", Namespace = "http://www.opengis.net/kml/2.2", IsNullable = false)]
    public class ItemIconType : KmlObjectBase
    {
        private string hrefField;

        private KmlObjectBase[] itemIconObjectExtensionGroupField;
        private string[] itemIconSimpleExtensionGroupField;
        private string stateField;

        /// <remarks/>
        public string state
        {
            get { return stateField; }
            set { stateField = value; }
        }

        /// <remarks/>
        public string href
        {
            get { return hrefField; }
            set { hrefField = value; }
        }

        /// <remarks/>
        [XmlElement("ItemIconSimpleExtensionGroup")]
        public string[] ItemIconSimpleExtensionGroup
        {
            get { return itemIconSimpleExtensionGroupField; }
            set { itemIconSimpleExtensionGroupField = value; }
        }

        /// <remarks/>
        [XmlElement("ItemIconObjectExtensionGroup")]
        public KmlObjectBase[] ItemIconObjectExtensionGroup
        {
            get { return itemIconObjectExtensionGroupField; }
            set { itemIconObjectExtensionGroupField = value; }
        }
    }
}