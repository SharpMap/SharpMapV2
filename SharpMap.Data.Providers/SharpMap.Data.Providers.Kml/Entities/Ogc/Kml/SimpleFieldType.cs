// /*
//  *  The attached / following is part of SharpMap.Data.Providers.Kml
//  *  SharpMap.Data.Providers.Kml is free software � 2008 Newgrove Consultants Limited, 
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
    [XmlType(TypeName = "SimpleFieldType", Namespace = Declarations.SchemaVersion), Serializable]
    public class SimpleFieldType
    {
        [XmlIgnore] private string _displayName;
        [XmlIgnore] private string _name;
        [XmlIgnore] private List<string> _simpleFieldExtension;
        [XmlIgnore] private string _type;

        [XmlAttribute(AttributeName = "type", DataType = "string")]
        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }

        [XmlAttribute(AttributeName = "name", DataType = "string")]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        [XmlElement(ElementName = "displayName", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "string"
            , Namespace = Declarations.SchemaVersion)]
        public string DisplayName
        {
            get { return _displayName; }
            set { _displayName = value; }
        }

        [XmlElement(Type = typeof (string), ElementName = "SimpleFieldExtension", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<string> SimpleFieldExtension
        {
            get
            {
                if (_simpleFieldExtension == null) _simpleFieldExtension = new List<string>();
                return _simpleFieldExtension;
            }
            set { _simpleFieldExtension = value; }
        }

        public void MakeSchemaCompliant()
        {
        }
    }
}