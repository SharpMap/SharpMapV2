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
    [Serializable, XmlType(TypeName = "FormulaType", Namespace = Declarations.SchemaVersion)]
    public class FormulaType
    {
        [XmlIgnore] private double _a;
        [XmlIgnore] public bool _aSpecified;
        [XmlIgnore] private double _b;
        [XmlIgnore] public bool _bSpecified = true;
        [XmlIgnore] private double _c;
        [XmlIgnore] public bool _cSpecified = true;
        [XmlIgnore] private double _d;
        [XmlIgnore] public bool _dSpecified;

        [XmlElement(ElementName = "a", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "double",
            Namespace = Declarations.SchemaVersion)]
        public double a
        {
            get { return _a; }
            set
            {
                _a = value;
                _aSpecified = true;
            }
        }

        [XmlElement(ElementName = "b", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "double",
            Namespace = Declarations.SchemaVersion)]
        public double b
        {
            get { return _b; }
            set
            {
                _b = value;
                _bSpecified = true;
            }
        }

        [XmlElement(ElementName = "c", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "double",
            Namespace = Declarations.SchemaVersion)]
        public double c
        {
            get { return _c; }
            set
            {
                _c = value;
                _cSpecified = true;
            }
        }

        [XmlElement(ElementName = "d", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "double",
            Namespace = Declarations.SchemaVersion)]
        public double d
        {
            get { return _d; }
            set
            {
                _d = value;
                _dSpecified = true;
            }
        }

        public virtual void MakeSchemaCompliant()
        {
        }
    }
}