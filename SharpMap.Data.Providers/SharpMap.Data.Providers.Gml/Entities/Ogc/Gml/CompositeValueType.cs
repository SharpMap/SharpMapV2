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
using System.Collections.Generic;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SharpMap.Entities.Ogc.Gml
{
    [Serializable, XmlType(TypeName = "CompositeValueType", Namespace = Declarations.SchemaVersion)]
    public class CompositeValueType : AbstractGMLType
    {
        [XmlIgnore] private AggregationType _aggregationType;
        [XmlIgnore] private List<ValueComponent> _valueComponent;
        [XmlIgnore] private ValueComponents _valueComponents;
        [XmlIgnore] public bool AggregationTypeSpecified;

        [XmlAttribute(AttributeName = "aggregationType")]
        public AggregationType AggregationType
        {
            get { return _aggregationType; }
            set
            {
                _aggregationType = value;
                AggregationTypeSpecified = true;
            }
        }

        [XmlElement(Type = typeof (ValueComponent), ElementName = "valueComponent", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<ValueComponent> ValueComponent
        {
            get
            {
                if (_valueComponent == null)
                {
                    _valueComponent = new List<ValueComponent>();
                }
                return _valueComponent;
            }
            set { _valueComponent = value; }
        }

        [XmlElement(Type = typeof (ValueComponents), ElementName = "valueComponents", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public ValueComponents ValueComponents
        {
            get { return _valueComponents; }
            set { _valueComponents = value; }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }
    }
}