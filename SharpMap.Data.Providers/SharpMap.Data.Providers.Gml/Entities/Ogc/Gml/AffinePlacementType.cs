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
    [Serializable, XmlType(TypeName = "AffinePlacementType", Namespace = "http://www.opengis.net/gml/3.2")]
    public class AffinePlacementType
    {
        [XmlIgnore] private string _inDimension;
        [XmlIgnore] private Location _location;
        [XmlIgnore] private string _outDimension;
        [XmlIgnore] private List<VectorType> _refDirection;

        public AffinePlacementType()
        {
            InDimension = string.Empty;
            OutDimension = string.Empty;
        }

        [XmlElement(ElementName = "inDimension", IsNullable = false, Form = XmlSchemaForm.Qualified,
            DataType = "positiveInteger", Namespace = "http://www.opengis.net/gml/3.2")]
        public string InDimension
        {
            get { return _inDimension; }
            set { _inDimension = value; }
        }

        [XmlElement(Type = typeof (Location), ElementName = "location", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public Location Location
        {
            get { return _location; }
            set { _location = value; }
        }

        [XmlElement(ElementName = "outDimension", IsNullable = false, Form = XmlSchemaForm.Qualified,
            DataType = "positiveInteger", Namespace = "http://www.opengis.net/gml/3.2")]
        public string OutDimension
        {
            get { return _outDimension; }
            set { _outDimension = value; }
        }

        [XmlElement(Type = typeof (VectorType), ElementName = "refDirection", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public List<VectorType> RefDirection
        {
            get
            {
                if (_refDirection == null)
                {
                    _refDirection = new List<VectorType>();
                }
                return _refDirection;
            }
            set { _refDirection = value; }
        }

        public virtual void MakeSchemaCompliant()
        {
            Location.MakeSchemaCompliant();
            foreach (VectorType _c in RefDirection)
            {
                _c.MakeSchemaCompliant();
            }
        }
    }
}