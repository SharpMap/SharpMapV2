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
    [Serializable, XmlType(TypeName = "FeatureArrayPropertyType", Namespace = "http://www.opengis.net/gml/3.2"),
     XmlInclude(typeof (ObservationType)), XmlInclude(typeof (DynamicFeatureType)),
     XmlInclude(typeof (AbstractCoverageType)), XmlInclude(typeof (AbstractFeatureCollectionType)),
     XmlInclude(typeof (BoundedFeatureType))]
    public class FeatureArrayPropertyType
    {
        [XmlIgnore] private List<AbstractFeature> _abstractFeature;

        [XmlElement(Type = typeof (AbstractFeature), ElementName = "AbstractFeature", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public List<AbstractFeature> AbstractFeature
        {
            get
            {
                if (_abstractFeature == null)
                {
                    _abstractFeature = new List<AbstractFeature>();
                }
                return _abstractFeature;
            }
            set { _abstractFeature = value; }
        }

        [XmlIgnore]
        public int Count
        {
            get { return AbstractFeature.Count; }
        }

        [XmlIgnore]
        public AbstractFeature this[int index]
        {
            get { return AbstractFeature[index]; }
        }

        public void Add(AbstractFeature obj)
        {
            AbstractFeature.Add(obj);
        }

        public void Clear()
        {
            AbstractFeature.Clear();
        }

        [DispId(-4)]
        public IEnumerator GetEnumerator()
        {
            return AbstractFeature.GetEnumerator();
        }

        public virtual void MakeSchemaCompliant()
        {
            foreach (AbstractFeature _c in AbstractFeature)
            {
                _c.MakeSchemaCompliant();
            }
        }

        public bool Remove(AbstractFeature obj)
        {
            return AbstractFeature.Remove(obj);
        }

        public AbstractFeature Remove(int index)
        {
            AbstractFeature obj = AbstractFeature[index];
            AbstractFeature.Remove(obj);
            return obj;
        }
    }
}