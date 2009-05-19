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
    [Serializable, XmlType(TypeName = "OperationMethodType", Namespace = Declarations.SchemaVersion)]
    public class OperationMethodType : IdentifiedObjectType
    {
        [XmlIgnore] private Formula _formula;
        [XmlIgnore] private FormulaCitation _formulaCitation;
        [XmlIgnore] private List<ParameterProperty> _parameter;
        [XmlIgnore] private string _sourceDimensions;
        [XmlIgnore] private string _targetDimensions;

        [XmlElement(Type = typeof (Formula), ElementName = "formula", IsNullable = false, Form = XmlSchemaForm.Qualified
            , Namespace = Declarations.SchemaVersion)]
        public Formula Formula
        {
            get { return _formula; }
            set { _formula = value; }
        }

        [XmlElement(Type = typeof (FormulaCitation), ElementName = "formulaCitation", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public FormulaCitation FormulaCitation
        {
            get { return _formulaCitation; }
            set { _formulaCitation = value; }
        }

        [XmlElement(Type = typeof (ParameterProperty), ElementName = "parameter", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<ParameterProperty> Parameter
        {
            get
            {
                if (_parameter == null)
                {
                    _parameter = new List<ParameterProperty>();
                }
                return _parameter;
            }
            set { _parameter = value; }
        }

        [XmlElement(ElementName = "sourceDimensions", IsNullable = false, Form = XmlSchemaForm.Qualified,
            DataType = "positiveInteger", Namespace = Declarations.SchemaVersion)]
        public string SourceDimensions
        {
            get { return _sourceDimensions; }
            set { _sourceDimensions = value; }
        }

        [XmlElement(ElementName = "targetDimensions", IsNullable = false, Form = XmlSchemaForm.Qualified,
            DataType = "positiveInteger", Namespace = Declarations.SchemaVersion)]
        public string TargetDimensions
        {
            get { return _targetDimensions; }
            set { _targetDimensions = value; }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
            FormulaCitation.MakeSchemaCompliant();
            Formula.MakeSchemaCompliant();
        }
    }
}