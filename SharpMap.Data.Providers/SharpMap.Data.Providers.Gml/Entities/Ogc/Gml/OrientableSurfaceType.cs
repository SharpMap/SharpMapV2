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
    [Serializable, XmlType(TypeName = "OrientableSurfaceType", Namespace = "http://www.opengis.net/gml/3.2")]
    public class OrientableSurfaceType : AbstractSurfaceType
    {
        [XmlIgnore] private BaseSurface _baseSurface;
        [XmlIgnore] private SignType _orientation;
        [XmlIgnore] public bool OrientationSpecified;

        public OrientableSurfaceType()
        {
            Orientation = SignType.Positive;
        }

        [XmlElement(Type = typeof (BaseSurface), ElementName = "baseSurface", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public BaseSurface BaseSurface
        {
            get { return _baseSurface; }
            set { _baseSurface = value; }
        }

        [XmlAttribute(AttributeName = "orientation")]
        public SignType Orientation
        {
            get { return _orientation; }
            set
            {
                _orientation = value;
                OrientationSpecified = true;
            }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
            BaseSurface.MakeSchemaCompliant();
        }
    }
}