// /*
//  *  The attached / following is part of SharpMap.Data.Providers.Kml
//  *  SharpMap.Data.Providers.Kml is free software © 2008 Newgrove Consultants Limited, 
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
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SharpMap.Entities.Ogc.Kml
{
    [XmlType(TypeName = "ExtendedDataType", Namespace = Declarations.SchemaVersion), Serializable]
    public class ExtendedDataType
    {
        [XmlIgnore] private List<Data> __Data;

        [XmlIgnore] private List<SchemaData> __SchemaData;

        [XmlAnyElement] public XmlElement[] Any;

        [XmlElement(Type = typeof (Data), ElementName = "Data", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = Declarations.SchemaVersion)]
        public List<Data> Data
        {
            get
            {
                if (__Data == null) __Data = new List<Data>();
                return __Data;
            }
            set { __Data = value; }
        }

        [XmlElement(Type = typeof (SchemaData), ElementName = "SchemaData", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<SchemaData> SchemaData
        {
            get
            {
                if (__SchemaData == null) __SchemaData = new List<SchemaData>();
                return __SchemaData;
            }
            set { __SchemaData = value; }
        }

        public void MakeSchemaCompliant()
        {
        }
    }
}