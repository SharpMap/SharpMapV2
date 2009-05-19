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
    [Serializable, XmlType(TypeName = "EnvelopeType", Namespace = Declarations.SchemaVersion)]
    public class EnvelopeType
    {
        [XmlIgnore] private string _axisLabels;
        [XmlIgnore] private Coordinates _coordinates;
        [XmlIgnore] private DirectPositionType _lowerCorner;
        [XmlIgnore] private List<Pos> _pos;
        [XmlIgnore] private string _srsDimension;
        [XmlIgnore] private string _srsName;
        [XmlIgnore] private string _uomLabels;
        [XmlIgnore] private DirectPositionType _upperCorner;

        [XmlAttribute(AttributeName = "axisLabels")]
        public string AxisLabels
        {
            get { return _axisLabels; }
            set { _axisLabels = value; }
        }

        [XmlElement(Type = typeof (Coordinates), ElementName = "coordinates", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public Coordinates Coordinates
        {
            get { return _coordinates; }
            set { _coordinates = value; }
        }

        [XmlElement(Type = typeof (DirectPositionType), ElementName = "lowerCorner", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public DirectPositionType LowerCorner
        {
            get { return _lowerCorner; }
            set { _lowerCorner = value; }
        }

        [XmlElement(Type = typeof (Pos), ElementName = "pos", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = Declarations.SchemaVersion)]
        public List<Pos> Pos
        {
            get
            {
                if (_pos == null)
                {
                    _pos = new List<Pos>();
                }
                return _pos;
            }
            set { _pos = value; }
        }

        [XmlAttribute(AttributeName = "srsDimension", DataType = "positiveInteger")]
        public string SrsDimension
        {
            get { return _srsDimension; }
            set { _srsDimension = value; }
        }

        [XmlAttribute(AttributeName = "srsName", DataType = "anyURI")]
        public string SrsName
        {
            get { return _srsName; }
            set { _srsName = value; }
        }

        [XmlAttribute(AttributeName = "uomLabels")]
        public string UomLabels
        {
            get { return _uomLabels; }
            set { _uomLabels = value; }
        }

        [XmlElement(Type = typeof (DirectPositionType), ElementName = "upperCorner", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public DirectPositionType UpperCorner
        {
            get { return _upperCorner; }
            set { _upperCorner = value; }
        }

        public virtual void MakeSchemaCompliant()
        {
            LowerCorner.MakeSchemaCompliant();
            UpperCorner.MakeSchemaCompliant();
            foreach (Pos _c in Pos)
            {
                _c.MakeSchemaCompliant();
            }
            Coordinates.MakeSchemaCompliant();
        }
    }
}