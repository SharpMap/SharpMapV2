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
    [Serializable, XmlType(TypeName = "TopoSurfacePropertyType", Namespace = Declarations.SchemaVersion)]
    public class TopoSurfacePropertyType
    {
        [XmlIgnore] private bool _owns;
        [XmlIgnore] private TopoSurface _topoSurface;
        [XmlIgnore] public bool OwnsSpecified;

        public TopoSurfacePropertyType()
        {
            Owns = false;
        }

        [XmlAttribute(AttributeName = "owns", DataType = "boolean")]
        public bool Owns
        {
            get { return _owns; }
            set
            {
                _owns = value;
                OwnsSpecified = true;
            }
        }

        [XmlElement(Type = typeof (TopoSurface), ElementName = "TopoSurface", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public TopoSurface TopoSurface
        {
            get { return _topoSurface; }
            set { _topoSurface = value; }
        }

        public virtual void MakeSchemaCompliant()
        {
            TopoSurface.MakeSchemaCompliant();
        }
    }
}