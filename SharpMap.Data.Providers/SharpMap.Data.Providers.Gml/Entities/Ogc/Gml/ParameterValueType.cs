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
    [Serializable, XmlType(TypeName = "ParameterValueType", Namespace = "http://www.opengis.net/gml/3.2")]
    public class ParameterValueType : AbstractGeneralParameterValueType
    {
        [XmlIgnore] private Boolean _booleanValue;
        [XmlIgnore] private DmsAngleValue _dmsAngleValue;
        [XmlIgnore] private string _integerValue;
        [XmlIgnore] private string _integerValueList;
        [XmlIgnore] private OperationParameterProperty _operationParameter;
        [XmlIgnore] private string _stringValue;
        [XmlIgnore] private Value _value;
        [XmlIgnore] private string _valueFile;
        [XmlIgnore] private ValueList _valueList;
        [XmlIgnore] public bool BooleanValueSpecified;

        public ParameterValueType()
        {
            StringValue = string.Empty;
            IntegerValue = string.Empty;
            BooleanValueSpecified = true;
            ValueFile = string.Empty;
        }

        [XmlElement(ElementName = "booleanValue", IsNullable = false, Form = XmlSchemaForm.Qualified,
            DataType = "boolean", Namespace = "http://www.opengis.net/gml/3.2")]
        public Boolean BooleanValue
        {
            get { return _booleanValue; }
            set
            {
                _booleanValue = value;
                BooleanValueSpecified = true;
            }
        }

        [XmlElement(Type = typeof (DmsAngleValue), ElementName = "dmsAngleValue", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public DmsAngleValue DmsAngleValue
        {
            get { return _dmsAngleValue; }
            set { _dmsAngleValue = value; }
        }

        [XmlElement(ElementName = "integerValue", IsNullable = false, Form = XmlSchemaForm.Qualified,
            DataType = "positiveInteger", Namespace = "http://www.opengis.net/gml/3.2")]
        public string IntegerValue
        {
            get { return _integerValue; }
            set { _integerValue = value; }
        }

        [XmlElement(ElementName = "integerValueList", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = "http://www.opengis.net/gml/3.2")]
        public string IntegerValueList
        {
            get { return _integerValueList; }
            set { _integerValueList = value; }
        }

        [XmlElement(Type = typeof (OperationParameterProperty), ElementName = "operationParameter", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public OperationParameterProperty OperationParameter
        {
            get { return _operationParameter; }
            set { _operationParameter = value; }
        }

        [XmlElement(ElementName = "stringValue", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "string"
            , Namespace = "http://www.opengis.net/gml/3.2")]
        public string StringValue
        {
            get { return _stringValue; }
            set { _stringValue = value; }
        }

        [XmlElement(Type = typeof (Value), ElementName = "value", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = "http://www.opengis.net/gml/3.2")]
        public Value Value
        {
            get { return _value; }
            set { _value = value; }
        }

        [XmlElement(ElementName = "valueFile", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "anyURI",
            Namespace = "http://www.opengis.net/gml/3.2")]
        public string ValueFile
        {
            get { return _valueFile; }
            set { _valueFile = value; }
        }

        [XmlElement(Type = typeof (ValueList), ElementName = "valueList", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public ValueList ValueList
        {
            get { return _valueList; }
            set { _valueList = value; }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
            Value.MakeSchemaCompliant();
            DmsAngleValue.MakeSchemaCompliant();
            ValueList.MakeSchemaCompliant();
            OperationParameter.MakeSchemaCompliant();
        }
    }
}