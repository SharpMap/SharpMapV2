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
    [Serializable, XmlType(TypeName = "ConcatenatedOperationType", Namespace = "http://www.opengis.net/gml/3.2")]
    public class ConcatenatedOperationType : AbstractCoordinateOperationType
    {
        [XmlIgnore] private AggregationType _aggregationType;
        [XmlIgnore] private List<CoordOperationProperty> _coordOperation;
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

        [XmlElement(Type = typeof (CoordOperationProperty), ElementName = "coordOperation", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public List<CoordOperationProperty> CoordOperation
        {
            get
            {
                if (_coordOperation == null)
                {
                    _coordOperation = new List<CoordOperationProperty>();
                }
                return _coordOperation;
            }
            set { _coordOperation = value; }
        }

        [XmlIgnore]
        public int Count
        {
            get { return CoordOperation.Count; }
        }

        [XmlIgnore]
        public CoordOperationProperty this[int index]
        {
            get { return CoordOperation[index]; }
        }

        public void Add(CoordOperationProperty obj)
        {
            CoordOperation.Add(obj);
        }

        public void Clear()
        {
            CoordOperation.Clear();
        }

        [DispId(-4)]
        public IEnumerator GetEnumerator()
        {
            return CoordOperation.GetEnumerator();
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
            foreach (CoordOperationProperty _c in CoordOperation)
            {
                _c.MakeSchemaCompliant();
            }
        }

        public bool Remove(CoordOperationProperty obj)
        {
            return CoordOperation.Remove(obj);
        }

        public CoordOperationProperty Remove(int index)
        {
            CoordOperationProperty obj = CoordOperation[index];
            CoordOperation.Remove(obj);
            return obj;
        }
    }
}