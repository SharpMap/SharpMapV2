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
    [Serializable, XmlType(TypeName = "DerivedUnitType", Namespace = "http://www.opengis.net/gml/3.2")]
    public class DerivedUnitType : UnitDefinitionType
    {
        [XmlIgnore] private List<DerivationUnitTerm> _derivationUnitTerm;

        [XmlIgnore]
        public int Count
        {
            get { return DerivationUnitTerm.Count; }
        }

        [XmlElement(Type = typeof (DerivationUnitTerm), ElementName = "derivationUnitTerm", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public List<DerivationUnitTerm> DerivationUnitTerm
        {
            get
            {
                if (_derivationUnitTerm == null)
                {
                    _derivationUnitTerm = new List<DerivationUnitTerm>();
                }
                return _derivationUnitTerm;
            }
            set { _derivationUnitTerm = value; }
        }

        [XmlIgnore]
        public DerivationUnitTerm this[int index]
        {
            get { return DerivationUnitTerm[index]; }
        }

        public void Add(DerivationUnitTerm obj)
        {
            DerivationUnitTerm.Add(obj);
        }

        public void Clear()
        {
            DerivationUnitTerm.Clear();
        }

        [DispId(-4)]
        public IEnumerator GetEnumerator()
        {
            return DerivationUnitTerm.GetEnumerator();
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
            foreach (DerivationUnitTerm _c in DerivationUnitTerm)
            {
                _c.MakeSchemaCompliant();
            }
        }

        public bool Remove(DerivationUnitTerm obj)
        {
            return DerivationUnitTerm.Remove(obj);
        }

        public DerivationUnitTerm Remove(int index)
        {
            DerivationUnitTerm obj = DerivationUnitTerm[index];
            DerivationUnitTerm.Remove(obj);
            return obj;
        }
    }
}