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
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SharpMap.Entities.Ogc.Gml
{
    [Serializable, XmlType(TypeName = "DynamicFeatureType", Namespace = "http://www.opengis.net/gml/3.2")]
    public class DynamicFeatureType : AbstractFeatureType
    {
        [XmlIgnore] private DataSource _dataSource;
        [XmlIgnore] private DataSourceReference _dataSourceReference;
        [XmlIgnore] private History _history;
        [XmlIgnore] private ValidTime _validTime;

        [XmlElement(Type = typeof (DataSource), ElementName = "dataSource", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public DataSource DataSource
        {
            get { return _dataSource; }
            set { _dataSource = value; }
        }

        [XmlElement(Type = typeof (DataSourceReference), ElementName = "dataSourceReference", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public DataSourceReference DataSourceReference
        {
            get { return _dataSourceReference; }
            set { _dataSourceReference = value; }
        }

        [XmlElement(Type = typeof (History), ElementName = "history", IsNullable = false, Form = XmlSchemaForm.Qualified
            , Namespace = "http://www.opengis.net/gml/3.2")]
        public History History
        {
            get { return _history; }
            set { _history = value; }
        }

        [XmlElement(Type = typeof (ValidTime), ElementName = "validTime", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public ValidTime ValidTime
        {
            get { return _validTime; }
            set { _validTime = value; }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }
    }
}