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
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SharpMap.Entities.Ogc.Gml
{
    [Serializable, XmlType(TypeName = "CompoundCRSType", Namespace = "http://www.opengis.net/gml/3.2")]
    public class CompoundCRSType : AbstractCRSType
    {
        [XmlIgnore] private AggregationType _aggregationType;
        [XmlIgnore] private List<ComponentReferenceSystemProperty> _componentReferenceSystem;
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

        [XmlElement(Type = typeof (ComponentReferenceSystemProperty), ElementName = "componentReferenceSystem",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public List<ComponentReferenceSystemProperty> ComponentReferenceSystem
        {
            get
            {
                if (_componentReferenceSystem == null)
                {
                    _componentReferenceSystem = new List<ComponentReferenceSystemProperty>();
                }
                return _componentReferenceSystem;
            }
            set { _componentReferenceSystem = value; }
        }

        [XmlIgnore]
        public int Count
        {
            get { return ComponentReferenceSystem.Count; }
        }

        [XmlIgnore]
        public ComponentReferenceSystemProperty this[int index]
        {
            get { return ComponentReferenceSystem[index]; }
        }

        public void Add(ComponentReferenceSystemProperty obj)
        {
            ComponentReferenceSystem.Add(obj);
        }

        public void Clear()
        {
            ComponentReferenceSystem.Clear();
        }

        [DispId(-4)]
        public IEnumerator GetEnumerator()
        {
            return ComponentReferenceSystem.GetEnumerator();
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
            foreach (ComponentReferenceSystemProperty _c in ComponentReferenceSystem)
            {
                _c.MakeSchemaCompliant();
            }
        }

        public bool Remove(ComponentReferenceSystemProperty obj)
        {
            return ComponentReferenceSystem.Remove(obj);
        }

        public ComponentReferenceSystemProperty Remove(int index)
        {
            ComponentReferenceSystemProperty obj = ComponentReferenceSystem[index];
            ComponentReferenceSystem.Remove(obj);
            return obj;
        }
    }
}