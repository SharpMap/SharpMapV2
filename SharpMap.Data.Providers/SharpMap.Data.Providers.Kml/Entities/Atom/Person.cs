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

namespace SharpMap.Entities.Atom
{
    /// <remarks/>
    [Serializable]
    [XmlType(TypeName = "atomPersonConstruct", Namespace = "http://www.w3.org/2005/Atom")]
    [XmlRoot("author", Namespace = "http://www.w3.org/2005/Atom", IsNullable = false)]
    public class Person
    {
        private ItemType[] itemsElementNameField;
        private string[] itemsField;

        /// <remarks/>
        [XmlElement("email", typeof (string))]
        [XmlElement("name", typeof (string))]
        [XmlElement("uri", typeof (string))]
        [XmlChoiceIdentifier("ItemsElementName")]
        public string[] Items
        {
            get { return itemsField; }
            set { itemsField = value; }
        }

        /// <remarks/>
        [XmlElement("ItemsElementName")]
        [XmlIgnore]
        public ItemType[] ItemsElementName
        {
            get { return itemsElementNameField; }
            set { itemsElementNameField = value; }
        }
    }
}