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

namespace SharpMap.Entities.Iso.Gmd
{
    [Serializable, XmlType(TypeName = "EX_spatialTemporalExtent_Type", Namespace = "http://www.isotc211.org/2005/gmd")]
    public class EX_spatialTemporalExtent_Type : EX_temporalExtent_Type
    {
        [XmlIgnore] private List<EX_geographicExtent_PropertyType> _spatialExtent;

        [XmlIgnore]
        public int Count
        {
            get { return SpatialExtent.Count; }
        }

        [XmlIgnore]
        public EX_geographicExtent_PropertyType this[int index]
        {
            get { return SpatialExtent[index]; }
        }

        [XmlElement(Type = typeof (EX_geographicExtent_PropertyType), ElementName = "spatialExtent", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<EX_geographicExtent_PropertyType> SpatialExtent
        {
            get
            {
                if (_spatialExtent == null)
                {
                    _spatialExtent = new List<EX_geographicExtent_PropertyType>();
                }
                return _spatialExtent;
            }
            set { _spatialExtent = value; }
        }

        public void Add(EX_geographicExtent_PropertyType obj)
        {
            SpatialExtent.Add(obj);
        }

        public void Clear()
        {
            SpatialExtent.Clear();
        }

        [DispId(-4)]
        public IEnumerator GetEnumerator()
        {
            return SpatialExtent.GetEnumerator();
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
            foreach (EX_geographicExtent_PropertyType _c in SpatialExtent)
            {
                _c.MakeSchemaCompliant();
            }
        }

        public bool Remove(EX_geographicExtent_PropertyType obj)
        {
            return SpatialExtent.Remove(obj);
        }

        public EX_geographicExtent_PropertyType Remove(int index)
        {
            EX_geographicExtent_PropertyType obj = SpatialExtent[index];
            SpatialExtent.Remove(obj);
            return obj;
        }
    }
}