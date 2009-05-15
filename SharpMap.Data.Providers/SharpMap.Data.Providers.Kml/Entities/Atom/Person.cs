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

namespace SharpMap.Entities.Atom
{
    [XmlType(TypeName = "atomPersonConstruct", Namespace = Declarations.SchemaVersion), Serializable]
    public class Person
    {
        [XmlIgnore] private List<string> _email;
        [XmlIgnore] private List<string> _name;

        [XmlIgnore] private List<string> _uri;

        [XmlElement(Type = typeof (string), ElementName = "name", IsNullable = false, Form = XmlSchemaForm.Qualified,
            DataType = "string", Namespace = Declarations.SchemaVersion)]
        public List<string> Name
        {
            get
            {
                if (_name == null) _name = new List<string>();
                return _name;
            }
            set { _name = value; }
        }

        [XmlElement(Type = typeof (string), ElementName = "uri", IsNullable = false, Form = XmlSchemaForm.Qualified,
            DataType = "string", Namespace = Declarations.SchemaVersion)]
        public List<string> Uri
        {
            get
            {
                if (_uri == null) _uri = new List<string>();
                return _uri;
            }
            set { _uri = value; }
        }

        [XmlElement(Type = typeof (string), ElementName = "email", IsNullable = false, Form = XmlSchemaForm.Qualified,
            DataType = "string", Namespace = Declarations.SchemaVersion)]
        public List<string> Email
        {
            get
            {
                if (_email == null) _email = new List<string>();
                return _email;
            }
            set { _email = value; }
        }

        public void MakeSchemaCompliant()
        {
        }
    }
}