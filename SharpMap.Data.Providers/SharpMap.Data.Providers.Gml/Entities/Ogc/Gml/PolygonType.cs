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
    [Serializable, XmlType(TypeName = "PolygonType", Namespace = Declarations.SchemaVersion)]
    public class PolygonType : AbstractSurfaceType
    {
        [XmlIgnore] private Exterior _exterior;
        [XmlIgnore] private List<Interior> _interior;

        [XmlElement(Type = typeof (Exterior), ElementName = "exterior", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public Exterior Exterior
        {
            get { return _exterior; }
            set { _exterior = value; }
        }

        [XmlElement(Type = typeof (Interior), ElementName = "interior", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<Interior> Interior
        {
            get
            {
                if (_interior == null)
                {
                    _interior = new List<Interior>();
                }
                return _interior;
            }
            set { _interior = value; }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }
    }
}