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
    [XmlType(TypeName = "link", AnonymousType = true, Namespace = "http://www.w3.org/2005/Atom")]
    [XmlRoot(Namespace = "http://www.w3.org/2005/Atom", IsNullable = false)]
    public class Link
    {
        private string hrefField;

        private string hreflangField;

        private string lengthField;
        private string relField;
        private string titleField;
        private string typeField;

        /// <remarks/>
        [XmlAttribute("href")]
        public string Href
        {
            get { return hrefField; }
            set { hrefField = value; }
        }

        /// <remarks/>
        [XmlAttribute("rel")]
        public string Rel
        {
            get { return relField; }
            set { relField = value; }
        }

        /// <remarks/>
        [XmlAttribute("type")]
        public string Type
        {
            get { return typeField; }
            set { typeField = value; }
        }

        /// <remarks/>
        [XmlAttribute("hreflang")]
        public string HrefLang
        {
            get { return hreflangField; }
            set { hreflangField = value; }
        }

        /// <remarks/>
        [XmlAttribute("title")]
        public string Title
        {
            get { return titleField; }
            set { titleField = value; }
        }

        /// <remarks/>
        [XmlAttribute("length")]
        public string Length
        {
            get { return lengthField; }
            set { lengthField = value; }
        }
    }
}