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
    [Serializable, XmlType(TypeName = "TopoVolumePropertyType", Namespace = Declarations.SchemaVersion)]
    public class TopoVolumePropertyType
    {
        [XmlIgnore] private bool _owns;
        [XmlIgnore] private TopoVolume _topoVolume;
        [XmlIgnore] public bool OwnsSpecified;

        public TopoVolumePropertyType()
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

        [XmlElement(Type = typeof (TopoVolume), ElementName = "TopoVolume", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public TopoVolume TopoVolume
        {
            get { return _topoVolume; }
            set { _topoVolume = value; }
        }

        public virtual void MakeSchemaCompliant()
        {
            TopoVolume.MakeSchemaCompliant();
        }
    }
}