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
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SharpMap.Entities.Ogc.Gml
{
    [Serializable, XmlType(TypeName = "KnotType", Namespace = Declarations.SchemaVersion)]
    public class KnotType
    {
        [XmlIgnore] private string _multiplicity;
        [XmlIgnore] private Value _value;
        [XmlIgnore] private double _weight;
        [XmlIgnore] public bool WeightSpecified;

        public KnotType()
        {
            Multiplicity = string.Empty;
            WeightSpecified = true;
        }

        [XmlElement(ElementName = "multiplicity", IsNullable = false, Form = XmlSchemaForm.Qualified,
            DataType = "nonNegativeInteger", Namespace = Declarations.SchemaVersion)]
        public string Multiplicity
        {
            get { return _multiplicity; }
            set { _multiplicity = value; }
        }

        [XmlElement(Type = typeof (Value), ElementName = "value", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = Declarations.SchemaVersion)]
        public Value Value
        {
            get { return _value; }
            set { _value = value; }
        }

        [XmlElement(ElementName = "weight", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "double",
            Namespace = Declarations.SchemaVersion)]
        public double Weight
        {
            get { return _weight; }
            set
            {
                _weight = value;
                WeightSpecified = true;
            }
        }

        public virtual void MakeSchemaCompliant()
        {
            Value.MakeSchemaCompliant();
        }
    }
}