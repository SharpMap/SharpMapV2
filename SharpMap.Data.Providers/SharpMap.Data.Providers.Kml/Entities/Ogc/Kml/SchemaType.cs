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
    [XmlType(TypeName = "SchemaType", Namespace = Declarations.SchemaVersion), Serializable]
    public class SchemaType
    {
        [XmlIgnore] private string _id;
        [XmlIgnore] private string _name;
        [XmlIgnore] private List<string> _SchemaExtension;
        [XmlIgnore] private List<SimpleField> _SimpleField;

        [XmlAttribute(AttributeName = "name", DataType = "string")]
        public string name
        {
            get { return _name; }
            set { _name = value; }
        }

        [XmlAttribute(AttributeName = "id", DataType = "ID")]
        public string id
        {
            get { return _id; }
            set { _id = value; }
        }

        [XmlElement(Type = typeof (SimpleField), ElementName = "SimpleField", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<SimpleField> SimpleField
        {
            get
            {
                if (_SimpleField == null) _SimpleField = new List<SimpleField>();
                return _SimpleField;
            }
            set { _SimpleField = value; }
        }

        [XmlElement(Type = typeof (string), ElementName = "SchemaExtension", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<string> SchemaExtension
        {
            get
            {
                if (_SchemaExtension == null) _SchemaExtension = new List<string>();
                return _SchemaExtension;
            }
            set { _SchemaExtension = value; }
        }

        public void MakeSchemaCompliant()
        {
        }
    }
}