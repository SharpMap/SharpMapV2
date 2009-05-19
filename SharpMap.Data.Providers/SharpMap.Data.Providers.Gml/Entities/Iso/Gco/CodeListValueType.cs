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
using System.Xml.Serialization;

namespace SharpMap.Entities.Iso.Gco
{
    [Serializable, XmlType(TypeName = "CodeListValue_type", Namespace = "http://www.isotc211.org/2005/gco")]
    public class CodeListValueType
    {
        [XmlIgnore] private string _codeList;
        [XmlIgnore] private string _codeListValue;
        [XmlIgnore] private string _codeSpace;
        [XmlIgnore] private string _value;

        public CodeListValueType()
        {
            CodeList = string.Empty;
            CodeListValue = string.Empty;
        }

        [XmlAttribute(AttributeName = "codeList", DataType = "anyURI")]
        public string CodeList
        {
            get { return _codeList; }
            set { _codeList = value; }
        }

        [XmlAttribute(AttributeName = "codeListValue", DataType = "anyURI")]
        public string CodeListValue
        {
            get { return _codeListValue; }
            set { _codeListValue = value; }
        }

        [XmlAttribute(AttributeName = "codeSpace", DataType = "anyURI")]
        public string CodeSpace
        {
            get { return _codeSpace; }
            set { _codeSpace = value; }
        }

        [XmlText(DataType = "string")]
        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public virtual void MakeSchemaCompliant()
        {
        }
    }
}