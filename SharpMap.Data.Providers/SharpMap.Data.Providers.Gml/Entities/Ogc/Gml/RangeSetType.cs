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

namespace SharpMap.Entities.Ogc.Gml
{
    [Serializable, XmlType(TypeName = "RangeSetType", Namespace = Declarations.SchemaVersion)]
    public class RangeSetType
    {
        [XmlIgnore] private List<object> _abstractScalarValueList;
        [XmlIgnore] private DataBlock _dataBlock;
        [XmlIgnore] private File _file;
        [XmlIgnore] private List<ValueArray> _valueArray;

        [XmlElement(Type = typeof (object), ElementName = "AbstractScalarValueList", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<object> AbstractScalarValueList
        {
            get
            {
                if (_abstractScalarValueList == null)
                {
                    _abstractScalarValueList = new List<object>();
                }
                return _abstractScalarValueList;
            }
            set { _abstractScalarValueList = value; }
        }

        [XmlElement(Type = typeof (DataBlock), ElementName = "DataBlock", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public DataBlock DataBlock
        {
            get { return _dataBlock; }
            set { _dataBlock = value; }
        }

        [XmlElement(Type = typeof (File), ElementName = "File", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = Declarations.SchemaVersion)]
        public File File
        {
            get { return _file; }
            set { _file = value; }
        }

        [XmlElement(Type = typeof (ValueArray), ElementName = "ValueArray", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<ValueArray> ValueArray
        {
            get
            {
                if (_valueArray == null)
                {
                    _valueArray = new List<ValueArray>();
                }
                return _valueArray;
            }
            set { _valueArray = value; }
        }

        public virtual void MakeSchemaCompliant()
        {
            foreach (ValueArray _c in ValueArray)
            {
                _c.MakeSchemaCompliant();
            }
            DataBlock.MakeSchemaCompliant();
            File.MakeSchemaCompliant();
        }
    }
}