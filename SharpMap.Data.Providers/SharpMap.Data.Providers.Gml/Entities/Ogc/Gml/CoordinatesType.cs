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

namespace SharpMap.Entities.Ogc.Gml
{
    [Serializable, XmlType(TypeName = "CoordinatesType", Namespace = Declarations.SchemaVersion)]
    public class CoordinatesType
    {
        [XmlIgnore] private string _cs;
        [XmlIgnore] private string _decimal;
        [XmlIgnore] private string _ts;
        [XmlIgnore] private string _value;

        public CoordinatesType()
        {
            Decimal = ".";
            Cs = ",";
            Ts = " ";
        }

        [XmlAttribute(AttributeName = "cs", DataType = "string")]
        public string Cs
        {
            get { return _cs; }
            set { _cs = value; }
        }

        [XmlAttribute(AttributeName = "decimal", DataType = "string")]
        public string Decimal
        {
            get { return _decimal; }
            set { _decimal = value; }
        }

        [XmlAttribute(AttributeName = "ts", DataType = "string")]
        public string Ts
        {
            get { return _ts; }
            set { _ts = value; }
        }

        [XmlText(DataType = "string")]
        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public virtual void MakeSchemaCompliant()
        {
        }
    }
}