using System;
using System.Collections.Generic;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SharpMap.Entities.Atom
{
    [XmlType(TypeName = "atomPersonConstruct", Namespace = Declarations.SchemaVersion), Serializable]
    public class Person
    {
        [XmlIgnore] private List<string> __email;
        [XmlIgnore] private List<string> __name;

        [XmlIgnore] private List<string> __uri;

        [XmlElement(Type = typeof (string), ElementName = "name", IsNullable = false, Form = XmlSchemaForm.Qualified,
            DataType = "string", Namespace = Declarations.SchemaVersion)]
        public List<string> Name
        {
            get
            {
                if (__name == null) __name = new List<string>();
                return __name;
            }
            set { __name = value; }
        }

        [XmlElement(Type = typeof (string), ElementName = "uri", IsNullable = false, Form = XmlSchemaForm.Qualified,
            DataType = "string", Namespace = Declarations.SchemaVersion)]
        public List<string> Uri
        {
            get
            {
                if (__uri == null) __uri = new List<string>();
                return __uri;
            }
            set { __uri = value; }
        }

        [XmlElement(Type = typeof (string), ElementName = "email", IsNullable = false, Form = XmlSchemaForm.Qualified,
            DataType = "string", Namespace = Declarations.SchemaVersion)]
        public List<string> Email
        {
            get
            {
                if (__email == null) __email = new List<string>();
                return __email;
            }
            set { __email = value; }
        }

        public void MakeSchemaCompliant()
        {
        }
    }
}