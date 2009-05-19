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
    [Serializable, XmlType(TypeName = "CoverageFunctionType", Namespace = "http://www.opengis.net/gml/3.2")]
    public class CoverageFunctionType
    {
        [XmlIgnore] private CoverageMappingRule _coverageMappingRule;
        [XmlIgnore] private GridFunction _gridFunction;
        [XmlIgnore] private MappingRule _mappingRule;

        [XmlElement(Type = typeof (CoverageMappingRule), ElementName = "CoverageMappingRule", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public CoverageMappingRule CoverageMappingRule
        {
            get { return _coverageMappingRule; }
            set { _coverageMappingRule = value; }
        }

        [XmlElement(Type = typeof (GridFunction), ElementName = "GridFunction", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public GridFunction GridFunction
        {
            get { return _gridFunction; }
            set { _gridFunction = value; }
        }

        [XmlElement(Type = typeof (MappingRule), ElementName = "MappingRule", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public MappingRule MappingRule
        {
            get { return _mappingRule; }
            set { _mappingRule = value; }
        }

        public virtual void MakeSchemaCompliant()
        {
            MappingRule.MakeSchemaCompliant();
            CoverageMappingRule.MakeSchemaCompliant();
            GridFunction.MakeSchemaCompliant();
        }
    }
}