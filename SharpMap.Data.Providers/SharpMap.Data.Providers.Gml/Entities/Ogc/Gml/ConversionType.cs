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
    [Serializable, XmlType(TypeName = "ConversionType", Namespace = Declarations.SchemaVersion)]
    public class ConversionType : AbstractGeneralConversionType
    {
        [XmlIgnore] private MethodProperty _method;
        [XmlIgnore] private List<ParameterValueProperty> _parameterValue;

        [XmlElement(Type = typeof (MethodProperty), ElementName = "method", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public MethodProperty Method
        {
            get { return _method; }
            set { _method = value; }
        }

        [XmlElement(Type = typeof (ParameterValueProperty), ElementName = "parameterValue", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<ParameterValueProperty> ParameterValue
        {
            get
            {
                if (_parameterValue == null)
                {
                    _parameterValue = new List<ParameterValueProperty>();
                }
                return _parameterValue;
            }
            set { _parameterValue = value; }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
            Method.MakeSchemaCompliant();
        }
    }
}