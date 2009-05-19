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
    [Serializable, XmlType(TypeName = "GeodeticDatumType", Namespace = Declarations.SchemaVersion)]
    public class GeodeticDatumType : AbstractDatumType
    {
        [XmlIgnore] private Ellipsoid_E _ellipsoid;
        [XmlIgnore] private PrimeMeridian_E _primeMeridian;

        [XmlElement(Type = typeof (Ellipsoid_E), ElementName = "ellipsoid", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public Ellipsoid_E Ellipsoid
        {
            get { return _ellipsoid; }
            set { _ellipsoid = value; }
        }

        [XmlElement(Type = typeof (PrimeMeridian_E), ElementName = "primeMeridian", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public PrimeMeridian_E PrimeMeridian
        {
            get { return _primeMeridian; }
            set { _primeMeridian = value; }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
            PrimeMeridian.MakeSchemaCompliant();
            Ellipsoid.MakeSchemaCompliant();
        }
    }
}