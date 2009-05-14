using System;
using System.Xml.Serialization;

namespace SharpMap.Entities.Atom
{
    [XmlRoot(ElementName = "author", Namespace = Declarations.SchemaVersion, IsNullable = false), Serializable]
    public class Author : Person
    {
        public new void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }
    }
}