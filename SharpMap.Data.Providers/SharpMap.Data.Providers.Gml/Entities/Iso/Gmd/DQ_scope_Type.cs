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
    [Serializable, XmlType(TypeName = "DQ_scope_Type", Namespace = "http://www.isotc211.org/2005/gmd")]
    public class DQ_scope_Type : AbstractObjectType
    {
        [XmlIgnore] private EX_extent_PropertyType _extent;
        [XmlIgnore] private MD_scopeCode_PropertyType _level;
        [XmlIgnore] private List<MD_scopeDescription_PropertyType> _levelDescription;

        [XmlElement(Type = typeof (EX_extent_PropertyType), ElementName = "extent", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public EX_extent_PropertyType Extent
        {
            get { return _extent; }
            set { _extent = value; }
        }

        [XmlElement(Type = typeof (MD_scopeCode_PropertyType), ElementName = "level", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public MD_scopeCode_PropertyType Level
        {
            get { return _level; }
            set { _level = value; }
        }

        [XmlElement(Type = typeof (MD_scopeDescription_PropertyType), ElementName = "levelDescription",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<MD_scopeDescription_PropertyType> LevelDescription
        {
            get
            {
                if (_levelDescription == null)
                {
                    _levelDescription = new List<MD_scopeDescription_PropertyType>();
                }
                return _levelDescription;
            }
            set { _levelDescription = value; }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
            Level.MakeSchemaCompliant();
        }
    }
}