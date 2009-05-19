// /*
//  *  The attached / following is part of SharpMap.Data.Providers.Gml
//  *  SharpMap.Data.Providers.Gml is free software © 2008 Newgrove Consultants Limited, 
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
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SharpMap.Entities.Iso.Gmd
{
    [Serializable, XmlType(TypeName = "URL_propertyType", Namespace = "http://www.isotc211.org/2005/gmd")]
    public class URLPropertyType
    {
        [XmlIgnore] private string _nilReason;
        [XmlIgnore] private string _uRL;

        public URLPropertyType()
        {
            URL = string.Empty;
        }

        [XmlAttribute(AttributeName = "nilReason", DataType = "anyURI")]
        public string NilReason
        {
            get { return _nilReason; }
            set { _nilReason = value; }
        }

        [XmlElement(ElementName = "URL", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "anyURI",
            Namespace = "http://www.isotc211.org/2005/gmd")]
        public string URL
        {
            get { return _uRL; }
            set { _uRL = value; }
        }

        public virtual void MakeSchemaCompliant()
        {
        }
    }
}