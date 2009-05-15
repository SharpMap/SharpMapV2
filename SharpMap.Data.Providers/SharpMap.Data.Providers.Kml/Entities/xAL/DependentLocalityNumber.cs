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
using System.Xml;
using System.Xml.Serialization;

namespace SharpMap.Entities.xAL
{
    [XmlType(TypeName = "DependentLocalityNumber", Namespace = Declarations.SchemaVersion), Serializable]
    public class DependentLocalityNumber
    {
        [XmlIgnore] private string _Code;
        [XmlIgnore] private Occurrence _NameNumberOccurrence;

        [XmlIgnore] public bool _NameNumberOccurrenceSpecified;
        [XmlIgnore] private string _Value;
        [XmlAnyAttribute] public XmlAttribute[] AnyAttr;

        [XmlAttribute(AttributeName = "NameNumberOccurrence")]
        public Occurrence NameNumberOccurrence
        {
            get { return _NameNumberOccurrence; }
            set
            {
                _NameNumberOccurrence = value;
                _NameNumberOccurrenceSpecified = true;
            }
        }

        [XmlAttribute(AttributeName = "Code")]
        public string Code
        {
            get { return _Code; }
            set { _Code = value; }
        }

        [XmlText(DataType = "string")]
        public string Value
        {
            get { return _Value; }
            set { _Value = value; }
        }

        public void MakeSchemaCompliant()
        {
        }
    }
}