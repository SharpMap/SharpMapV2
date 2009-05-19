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
    [Serializable, XmlType(TypeName = "AbstractDS_aggregate_Type", Namespace = "http://www.isotc211.org/2005/gmd")]
    public abstract class AbstractDS_aggregate_Type : AbstractObjectType
    {
        [XmlIgnore] private List<DS_dataSet_PropertyType> _composedOf;
        [XmlIgnore] private List<MD_metadata_PropertyType> _seriesMetadata;
        [XmlIgnore] private List<DS_aggregate_PropertyType> _subset;
        [XmlIgnore] private List<DS_aggregate_PropertyType> _superset;

        [XmlElement(Type = typeof (DS_dataSet_PropertyType), ElementName = "composedOf", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<DS_dataSet_PropertyType> ComposedOf
        {
            get
            {
                if (_composedOf == null)
                {
                    _composedOf = new List<DS_dataSet_PropertyType>();
                }
                return _composedOf;
            }
            set { _composedOf = value; }
        }

        [XmlElement(Type = typeof (MD_metadata_PropertyType), ElementName = "seriesMetadata", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<MD_metadata_PropertyType> SeriesMetadata
        {
            get
            {
                if (_seriesMetadata == null)
                {
                    _seriesMetadata = new List<MD_metadata_PropertyType>();
                }
                return _seriesMetadata;
            }
            set { _seriesMetadata = value; }
        }

        [XmlElement(Type = typeof (DS_aggregate_PropertyType), ElementName = "subset", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<DS_aggregate_PropertyType> Subset
        {
            get
            {
                if (_subset == null)
                {
                    _subset = new List<DS_aggregate_PropertyType>();
                }
                return _subset;
            }
            set { _subset = value; }
        }

        [XmlElement(Type = typeof (DS_aggregate_PropertyType), ElementName = "superset", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<DS_aggregate_PropertyType> Superset
        {
            get
            {
                if (_superset == null)
                {
                    _superset = new List<DS_aggregate_PropertyType>();
                }
                return _superset;
            }
            set { _superset = value; }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
            foreach (DS_dataSet_PropertyType _c in ComposedOf)
            {
                _c.MakeSchemaCompliant();
            }
            foreach (MD_metadata_PropertyType _c in SeriesMetadata)
            {
                _c.MakeSchemaCompliant();
            }
        }
    }
}