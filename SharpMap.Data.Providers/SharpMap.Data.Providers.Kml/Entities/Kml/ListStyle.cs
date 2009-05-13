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
    [XmlType(TypeName = "ListStyleType", Namespace = "http://www.opengis.net/kml/2.2")]
    [XmlRoot("ListStyle", Namespace = "http://www.opengis.net/kml/2.2", IsNullable = false)]
    public class ListStyle : SubStyleBase
    {
        private byte[] bgColorField;

        private ItemIconType[] itemIconField;
        private ListItemType listItemTypeField;

        private bool listItemTypeFieldSpecified;
        private KmlObjectBase[] listStyleObjectExtensionGroupField;
        private string[] listStyleSimpleExtensionGroupField;

        private int maxSnippetLinesField;

        private bool maxSnippetLinesFieldSpecified;

        public ListStyle()
        {
            listItemTypeField = ListItemType.Check;
            maxSnippetLinesField = 2;
        }

        /// <remarks/>
        public ListItemType listItemType
        {
            get { return listItemTypeField; }
            set { listItemTypeField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool listItemTypeSpecified
        {
            get { return listItemTypeFieldSpecified; }
            set { listItemTypeFieldSpecified = value; }
        }

        /// <remarks/>
        // CODEGEN Warning: 'default' attribute on items of type 'hexBinary' is not supported in this version of the .Net Framework.  Ignoring default='ffffffff' attribute.
        [XmlElement(DataType = "hexBinary")]
        public byte[] bgColor
        {
            get { return bgColorField; }
            set { bgColorField = value; }
        }

        /// <remarks/>
        [XmlElement("ItemIcon")]
        public ItemIconType[] ItemIcon
        {
            get { return itemIconField; }
            set { itemIconField = value; }
        }

        /// <remarks/>
        public int maxSnippetLines
        {
            get { return maxSnippetLinesField; }
            set { maxSnippetLinesField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool maxSnippetLinesSpecified
        {
            get { return maxSnippetLinesFieldSpecified; }
            set { maxSnippetLinesFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement("ListStyleSimpleExtensionGroup")]
        public string[] ListStyleSimpleExtensionGroup
        {
            get { return listStyleSimpleExtensionGroupField; }
            set { listStyleSimpleExtensionGroupField = value; }
        }

        /// <remarks/>
        [XmlElement("ListStyleObjectExtensionGroup")]
        public KmlObjectBase[] ListStyleObjectExtensionGroup
        {
            get { return listStyleObjectExtensionGroupField; }
            set { listStyleObjectExtensionGroupField = value; }
        }
    }
}