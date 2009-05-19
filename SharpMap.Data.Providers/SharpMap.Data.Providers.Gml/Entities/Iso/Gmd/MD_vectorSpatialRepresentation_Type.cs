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

namespace SharpMap.Entities.Iso.Gmd
{
    [Serializable,
     XmlType(TypeName = "MD_vectorSpatialRepresentation_Type", Namespace = "http://www.isotc211.org/2005/gmd")]
    public class MD_vectorSpatialRepresentation_Type : AbstractMD_spatialRepresentation_Type
    {
        [XmlIgnore] private List<MD_geometricObjects_PropertyType> _geometricObjects;
        [XmlIgnore] private MD_topologyLevelCode_PropertyType _topologyLevel;

        [XmlElement(Type = typeof (MD_geometricObjects_PropertyType), ElementName = "geometricObjects",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<MD_geometricObjects_PropertyType> GeometricObjects
        {
            get
            {
                if (_geometricObjects == null)
                {
                    _geometricObjects = new List<MD_geometricObjects_PropertyType>();
                }
                return _geometricObjects;
            }
            set { _geometricObjects = value; }
        }

        [XmlElement(Type = typeof (MD_topologyLevelCode_PropertyType), ElementName = "topologyLevel", IsNullable = false
            , Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public MD_topologyLevelCode_PropertyType TopologyLevel
        {
            get { return _topologyLevel; }
            set { _topologyLevel = value; }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }
    }
}