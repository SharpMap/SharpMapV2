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
    [Serializable, XmlType(TypeName = "ArcStringByBulgeType", Namespace = Declarations.SchemaVersion)]
    public class ArcStringByBulgeType : ArcStringByBulgeTypeBase
    {

        [XmlIgnore]
        private List<VectorType> _normal;

        [XmlIgnore]
        private List<double> _bulge;

        public ArcStringByBulgeType()
            : base()
        {
        }

        [XmlElement(Type = typeof(double), ElementName = "bulge", IsNullable = false, Form = XmlSchemaForm.Qualified,
            DataType = "double", Namespace = Declarations.SchemaVersion)]
        public List<double> Bulge
        {
            get
            {
                if (_bulge == null)
                {
                    _bulge = new List<double>();
                }
                return _bulge;
            }
            set { _bulge = value; }
        }
        [XmlElement(Type = typeof(VectorType), ElementName = "normal", IsNullable = false,
    Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<VectorType> Normal
        {
            get
            {
                if (_normal == null)
                {
                    _normal = new List<VectorType>();
                }
                return _normal;
            }
            set { _normal = value; }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
            foreach (VectorType _c in Normal)
            {
                _c.MakeSchemaCompliant();
            }
        }
    }
}