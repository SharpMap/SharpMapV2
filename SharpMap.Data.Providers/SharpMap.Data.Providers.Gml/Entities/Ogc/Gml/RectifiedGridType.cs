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
    [Serializable, XmlType(TypeName = "RectifiedGridType", Namespace = Declarations.SchemaVersion)]
    public class RectifiedGridType : GridType
    {
        [XmlIgnore] private List<VectorType> _offsetVector;
        [XmlIgnore] private DateTime _origin;
        [XmlIgnore] public bool OriginSpecified;

        public RectifiedGridType()
        {
            Origin = DateTime.Now;
        }

        [XmlElement(Type = typeof (VectorType), ElementName = "offsetVector", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<VectorType> OffsetVector
        {
            get
            {
                if (_offsetVector == null)
                {
                    _offsetVector = new List<VectorType>();
                }
                return _offsetVector;
            }
            set { _offsetVector = value; }
        }

        [XmlElement(ElementName = "origin", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "dateTime",
            Namespace = Declarations.SchemaVersion)]
        public DateTime Origin
        {
            get { return _origin; }
            set
            {
                _origin = value;
                OriginSpecified = true;
            }
        }

        [XmlIgnore]
        public DateTime OriginUtc
        {
            get { return _origin.ToUniversalTime(); }
            set
            {
                _origin = value.ToLocalTime();
                OriginSpecified = true;
            }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
            foreach (VectorType _c in OffsetVector)
            {
                _c.MakeSchemaCompliant();
            }
        }
    }
}