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
    [Serializable, XmlType(TypeName = "MovingObjectStatusType", Namespace = "http://www.opengis.net/gml/3.2")]
    public class MovingObjectStatusType : AbstractTimeSliceType
    {
        [XmlIgnore] private MeasureType _acceleration;
        [XmlIgnore] private DirectionPropertyType _bearing;
        [XmlIgnore] private MeasureType _elevation;
        [XmlIgnore] private Location _location;
        [XmlIgnore] private LocationName _locationName;
        [XmlIgnore] private LocationReference _locationReference;
        [XmlIgnore] private Pos _pos;
        [XmlIgnore] private Position _position;
        [XmlIgnore] private MeasureType _speed;
        [XmlIgnore] private Status _status;
        [XmlIgnore] private StatusReference _statusReference;

        [XmlElement(Type = typeof (MeasureType), ElementName = "acceleration", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public MeasureType Acceleration
        {
            get { return _acceleration; }
            set { _acceleration = value; }
        }

        [XmlElement(Type = typeof (DirectionPropertyType), ElementName = "bearing", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public DirectionPropertyType Bearing
        {
            get { return _bearing; }
            set { _bearing = value; }
        }

        [XmlElement(Type = typeof (MeasureType), ElementName = "elevation", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public MeasureType Elevation
        {
            get { return _elevation; }
            set { _elevation = value; }
        }

        [XmlElement(Type = typeof (Location), ElementName = "location", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public Location Location
        {
            get { return _location; }
            set { _location = value; }
        }

        [XmlElement(Type = typeof (LocationName), ElementName = "locationName", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public LocationName LocationName
        {
            get { return _locationName; }
            set { _locationName = value; }
        }

        [XmlElement(Type = typeof (LocationReference), ElementName = "locationReference", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public LocationReference LocationReference
        {
            get { return _locationReference; }
            set { _locationReference = value; }
        }

        [XmlElement(Type = typeof (Pos), ElementName = "pos", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = "http://www.opengis.net/gml/3.2")]
        public Pos Pos
        {
            get { return _pos; }
            set { _pos = value; }
        }

        [XmlElement(Type = typeof (Position), ElementName = "position", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public Position Position
        {
            get { return _position; }
            set { _position = value; }
        }

        [XmlElement(Type = typeof (MeasureType), ElementName = "speed", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public MeasureType Speed
        {
            get { return _speed; }
            set { _speed = value; }
        }

        [XmlElement(Type = typeof (Status), ElementName = "status", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = "http://www.opengis.net/gml/3.2")]
        public Status Status
        {
            get { return _status; }
            set { _status = value; }
        }

        [XmlElement(Type = typeof (StatusReference), ElementName = "statusReference", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public StatusReference StatusReference
        {
            get { return _statusReference; }
            set { _statusReference = value; }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
            Position.MakeSchemaCompliant();
            Pos.MakeSchemaCompliant();
            LocationName.MakeSchemaCompliant();
            LocationReference.MakeSchemaCompliant();
            Location.MakeSchemaCompliant();
        }
    }
}