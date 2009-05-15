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
    [XmlType(TypeName = "SchemaDataType", Namespace = Declarations.SchemaVersion), Serializable]
    public class SchemaDataType : AbstractObjectType
    {
        [XmlIgnore] private List<string> _schemaDataExtension;
        [XmlIgnore] private string _schemaUrl;

        [XmlIgnore] private List<SimpleData> _simpleData;

        [XmlAttribute(AttributeName = "schemaUrl", DataType = "anyURI")]
        public string schemaUrl
        {
            get { return _schemaUrl; }
            set { _schemaUrl = value; }
        }

        [XmlElement(Type = typeof (SimpleData), ElementName = "SimpleData", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<SimpleData> SimpleData
        {
            get
            {
                if (_simpleData == null) _simpleData = new List<SimpleData>();
                return _simpleData;
            }
            set { _simpleData = value; }
        }

        [XmlElement(Type = typeof (string), ElementName = "SchemaDataExtension", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<string> SchemaDataExtension
        {
            get
            {
                if (_schemaDataExtension == null) _schemaDataExtension = new List<string>();
                return _schemaDataExtension;
            }
            set { _schemaDataExtension = value; }
        }

        public new void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }
    }
}