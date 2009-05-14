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
using System.Collections.Generic;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SharpMap.Entities.Ogc.Kml
{
    [XmlType(TypeName = "DataType", Namespace = Declarations.SchemaVersion), Serializable]
    public class DataType : AbstractObjectType
    {
        [XmlIgnore] private List<string> __DataExtension;
        [XmlIgnore] private string __displayName;
        [XmlIgnore] private string __name;
        [XmlIgnore] private string __value;

        public DataType()
        {
            @value = string.Empty;
        }

        [XmlAttribute(AttributeName = "name", DataType = "string")]
        public string name
        {
            get { return __name; }
            set { __name = value; }
        }

        [XmlElement(ElementName = "displayName", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "string"
            , Namespace = Declarations.SchemaVersion)]
        public string displayName
        {
            get { return __displayName; }
            set { __displayName = value; }
        }

        [XmlElement(ElementName = "value", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "string",
            Namespace = Declarations.SchemaVersion)]
        public string @value
        {
            get { return __value; }
            set { __value = value; }
        }

        [XmlElement(Type = typeof (string), ElementName = "DataExtension", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<string> DataExtension
        {
            get
            {
                if (__DataExtension == null) __DataExtension = new List<string>();
                return __DataExtension;
            }
            set { __DataExtension = value; }
        }

        public new void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }
    }
}