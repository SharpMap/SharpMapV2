// /*
//  *  The attached / following is part of SharpMap.Data.Providers.Gml
//  *  SharpMap.Data.Providers.Gml is free software � 2008 Newgrove Consultants Limited, 
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
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SharpMap.Entities.Ogc.Gml
{
    [Serializable, XmlType(TypeName = "AbstractMetaDataType", Namespace = Declarations.SchemaVersion)]
    public abstract class AbstractMetaDataType
    {
        [XmlIgnore] private string _id;
        [XmlIgnore] private List<String> _value = new List<string>();

        [XmlAttribute(AttributeName = "id", DataType = "ID")]
        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }

        [XmlText(DataType = "string")]
        public List<string> Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public virtual void MakeSchemaCompliant()
        {
        }
    }
}