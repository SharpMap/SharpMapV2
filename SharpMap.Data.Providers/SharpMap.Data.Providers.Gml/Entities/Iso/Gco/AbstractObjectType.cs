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
using System.Xml.Serialization;

namespace SharpMap.Entities.Iso.Gco
{
    [Serializable, XmlType(TypeName = "AbstractObject_type", Namespace = "http://www.isotc211.org/2005/gco")]
    public abstract class AbstractObjectType
    {
        [XmlIgnore] private string _id;
        [XmlIgnore] private string _uuid;

        [XmlAttribute(AttributeName = "id", DataType = "ID")]
        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }

        [XmlAttribute(AttributeName = "uuid", DataType = "string")]
        public string Uuid
        {
            get { return _uuid; }
            set { _uuid = value; }
        }

        public virtual void MakeSchemaCompliant()
        {
        }
    }
}