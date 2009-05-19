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
using SharpMap.Entities.Iso.Gco;

namespace SharpMap.Entities.Iso.Gmd
{
    [Serializable, XmlType(TypeName = "DQ_dataQuality_Type", Namespace = "http://www.isotc211.org/2005/gmd")]
    public class DQ_dataQuality_Type : AbstractObjectType
    {
        [XmlIgnore] private LI_lineage_PropertyType _lineage;
        [XmlIgnore] private List<DQ_element_PropertyType> _report;
        [XmlIgnore] private DQ_scope_PropertyType _scope;

        [XmlElement(Type = typeof (LI_lineage_PropertyType), ElementName = "lineage", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public LI_lineage_PropertyType Lineage
        {
            get { return _lineage; }
            set { _lineage = value; }
        }

        [XmlElement(Type = typeof (DQ_element_PropertyType), ElementName = "report", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<DQ_element_PropertyType> Report
        {
            get
            {
                if (_report == null)
                {
                    _report = new List<DQ_element_PropertyType>();
                }
                return _report;
            }
            set { _report = value; }
        }

        [XmlElement(Type = typeof (DQ_scope_PropertyType), ElementName = "scope", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public DQ_scope_PropertyType Scope
        {
            get { return _scope; }
            set { _scope = value; }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
            Scope.MakeSchemaCompliant();
        }
    }
}