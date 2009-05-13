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
    [XmlType(TypeName = "ChangeType", Namespace = "http://www.opengis.net/kml/2.2")]
    [XmlRoot("Change", Namespace = "http://www.opengis.net/kml/2.2", IsNullable = false)]
    public class Change
    {
        private ItemsChoiceType1[] itemsElementNameField;
        private KmlObjectBase[] itemsField;

        /// <remarks/>
        [XmlElement("Alias", typeof (Alias))]
        [XmlElement("Data", typeof (Data))]
        [XmlElement("Icon", typeof (LinkType))]
        [XmlElement("ImagePyramid", typeof (ImagePyramid))]
        [XmlElement("ItemIcon", typeof (ItemIconType))]
        [XmlElement("LatLonAltBox", typeof (LatLonAltBox))]
        [XmlElement("LatLonBox", typeof (LatLonBox))]
        [XmlElement("Link", typeof (LinkType))]
        [XmlElement("Location", typeof (Location))]
        [XmlElement("Lod", typeof (Lod))]
        [XmlElement("Orientation", typeof (Orientation))]
        [XmlElement("Pair", typeof (Pair))]
        [XmlElement("Region", typeof (Region))]
        [XmlElement("ResourceMap", typeof (ResourceMap))]
        [XmlElement("Scale", typeof (Scale))]
        [XmlElement("SchemaData", typeof (SchemaData))]
        [XmlElement("Url", typeof (LinkType))]
        [XmlElement("ViewVolume", typeof (ViewVolume))]
        [XmlChoiceIdentifier("ItemsElementName")]
        public KmlObjectBase[] Items
        {
            get { return itemsField; }
            set { itemsField = value; }
        }

        /// <remarks/>
        [XmlElement("ItemsElementName")]
        [XmlIgnore]
        public ItemsChoiceType1[] ItemsElementName
        {
            get { return itemsElementNameField; }
            set { itemsElementNameField = value; }
        }
    }
}