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
    [Serializable, XmlType(TypeName = "DirectionVectorType", Namespace = "http://www.opengis.net/gml/3.2")]
    public class DirectionVectorType
    {
        [XmlIgnore] private AngleType _horizontalAngle;
        [XmlIgnore] private Vector _vector;
        [XmlIgnore] private AngleType _verticalAngle;

        [XmlElement(Type = typeof (AngleType), ElementName = "horizontalAngle", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public AngleType HorizontalAngle
        {
            get { return _horizontalAngle; }
            set { _horizontalAngle = value; }
        }

        [XmlElement(Type = typeof (Vector), ElementName = "vector", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = "http://www.opengis.net/gml/3.2")]
        public Vector Vector
        {
            get { return _vector; }
            set { _vector = value; }
        }

        [XmlElement(Type = typeof (AngleType), ElementName = "verticalAngle", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public AngleType VerticalAngle
        {
            get { return _verticalAngle; }
            set { _verticalAngle = value; }
        }

        public virtual void MakeSchemaCompliant()
        {
            Vector.MakeSchemaCompliant();
            HorizontalAngle.MakeSchemaCompliant();
            VerticalAngle.MakeSchemaCompliant();
        }
    }
}