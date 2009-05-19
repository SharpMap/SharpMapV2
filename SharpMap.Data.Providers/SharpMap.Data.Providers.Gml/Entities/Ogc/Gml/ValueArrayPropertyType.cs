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
    [Serializable, XmlInclude(typeof (AbstractGeometricAggregateType)),
     XmlInclude(typeof (AbstractGeometricPrimitiveType)), XmlInclude(typeof (AbstractTimeComplexType)),
     XmlInclude(typeof (AbstractTimePrimitiveType)),
     XmlType(TypeName = "ValueArrayPropertyType", Namespace = Declarations.SchemaVersion),
     XmlInclude(typeof (GeometricComplexType)), XmlInclude(typeof (GridType))]
    public class ValueArrayPropertyType
    {
        [XmlIgnore] private AbstractGeometry _abstractGeometry;
        [XmlIgnore] private AbstractTimeObject _abstractTimeObject;
        [XmlIgnore] private object _abstractValue;
        [XmlIgnore] private string _null;
        [XmlIgnore] private bool _owns;
        [XmlIgnore] public bool OwnsSpecified;

        public ValueArrayPropertyType()
        {
            Owns = false;
            Null = string.Empty;
        }

        [XmlElement(Type = typeof (AbstractGeometry), ElementName = "AbstractGeometry", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public AbstractGeometry AbstractGeometry
        {
            get { return _abstractGeometry; }
            set { _abstractGeometry = value; }
        }

        [XmlElement(Type = typeof (AbstractTimeObject), ElementName = "AbstractTimeObject", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public AbstractTimeObject AbstractTimeObject
        {
            get { return _abstractTimeObject; }
            set { _abstractTimeObject = value; }
        }

        [XmlElement(ElementName = "AbstractValue", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = Declarations.SchemaVersion)]
        public object AbstractValue
        {
            get { return _abstractValue; }
            set { _abstractValue = value; }
        }

        [XmlElement(ElementName = "Null", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "anyURI",
            Namespace = Declarations.SchemaVersion)]
        public string Null
        {
            get { return _null; }
            set { _null = value; }
        }

        [XmlAttribute(AttributeName = "owns", DataType = "boolean")]
        public bool Owns
        {
            get { return _owns; }
            set
            {
                _owns = value;
                OwnsSpecified = true;
            }
        }

        public virtual void MakeSchemaCompliant()
        {
            AbstractGeometry.MakeSchemaCompliant();
            AbstractTimeObject.MakeSchemaCompliant();
        }
    }
}