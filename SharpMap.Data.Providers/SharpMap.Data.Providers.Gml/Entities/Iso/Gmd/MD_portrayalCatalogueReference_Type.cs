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
using SharpMap.Entities.Iso.Gco;

namespace SharpMap.Entities.Iso.Gmd
{
    [Serializable,
     XmlType(TypeName = "MD_portrayalCatalogueReference_Type", Namespace = "http://www.isotc211.org/2005/gmd")]
    public class MD_portrayalCatalogueReference_Type : AbstractObjectType
    {
        [XmlIgnore] private List<CI_citation_PropertyType> _portrayalCatalogueCitation;

        [XmlIgnore]
        public int Count
        {
            get { return PortrayalCatalogueCitation.Count; }
        }

        [XmlIgnore]
        public CI_citation_PropertyType this[int index]
        {
            get { return PortrayalCatalogueCitation[index]; }
        }

        [XmlElement(Type = typeof (CI_citation_PropertyType), ElementName = "portrayalCatalogueCitation",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<CI_citation_PropertyType> PortrayalCatalogueCitation
        {
            get
            {
                if (_portrayalCatalogueCitation == null)
                {
                    _portrayalCatalogueCitation = new List<CI_citation_PropertyType>();
                }
                return _portrayalCatalogueCitation;
            }
            set { _portrayalCatalogueCitation = value; }
        }

        public void Add(CI_citation_PropertyType obj)
        {
            PortrayalCatalogueCitation.Add(obj);
        }

        public void Clear()
        {
            PortrayalCatalogueCitation.Clear();
        }

        [DispId(-4)]
        public IEnumerator GetEnumerator()
        {
            return PortrayalCatalogueCitation.GetEnumerator();
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
            foreach (CI_citation_PropertyType _c in PortrayalCatalogueCitation)
            {
                _c.MakeSchemaCompliant();
            }
        }

        public bool Remove(CI_citation_PropertyType obj)
        {
            return PortrayalCatalogueCitation.Remove(obj);
        }

        public CI_citation_PropertyType Remove(int index)
        {
            CI_citation_PropertyType obj = PortrayalCatalogueCitation[index];
            PortrayalCatalogueCitation.Remove(obj);
            return obj;
        }
    }
}