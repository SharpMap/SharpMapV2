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
    [Serializable, XmlType(TypeName = "OffsetCurveType", Namespace = "http://www.opengis.net/gml/3.2")]
    public class OffsetCurveType : AbstractCurveSegmentType
    {
        [XmlIgnore] private LengthType _distance;
        [XmlIgnore] private CurvePropertyType _offsetBase;
        [XmlIgnore] private VectorType _refDirection;

        [XmlElement(Type = typeof (LengthType), ElementName = "distance", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public LengthType Distance
        {
            get { return _distance; }
            set { _distance = value; }
        }

        [XmlElement(Type = typeof (CurvePropertyType), ElementName = "offsetBase", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public CurvePropertyType OffsetBase
        {
            get { return _offsetBase; }
            set { _offsetBase = value; }
        }

        [XmlElement(Type = typeof (VectorType), ElementName = "refDirection", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public VectorType RefDirection
        {
            get { return _refDirection; }
            set { _refDirection = value; }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
            OffsetBase.MakeSchemaCompliant();
            Distance.MakeSchemaCompliant();
        }
    }
}