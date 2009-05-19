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
    [Serializable, XmlType(TypeName = "PassThroughOperationType", Namespace = Declarations.SchemaVersion)]
    public class PassThroughOperationType : AbstractCoordinateOperationType
    {
        [XmlIgnore] private AggregationType _aggregationType;
        [XmlIgnore] private CoordOperationProperty _coordOperation;
        [XmlIgnore] private List<string> _modifiedCoordinate;
        [XmlIgnore] public bool AggregationTypeSpecified;

        [XmlAttribute(AttributeName = "aggregationType")]
        public AggregationType AggregationType
        {
            get { return _aggregationType; }
            set
            {
                _aggregationType = value;
                AggregationTypeSpecified = true;
            }
        }

        [XmlElement(Type = typeof (CoordOperationProperty), ElementName = "coordOperation", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public CoordOperationProperty CoordOperation
        {
            get { return _coordOperation; }
            set { _coordOperation = value; }
        }

        [XmlElement(Type = typeof (string), ElementName = "modifiedCoordinate", IsNullable = false,
            Form = XmlSchemaForm.Qualified, DataType = "positiveInteger", Namespace = Declarations.SchemaVersion)]
        public List<string> ModifiedCoordinate
        {
            get
            {
                if (_modifiedCoordinate == null)
                {
                    _modifiedCoordinate = new List<string>();
                }
                return _modifiedCoordinate;
            }
            set { _modifiedCoordinate = value; }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
            CoordOperation.MakeSchemaCompliant();
        }
    }
}