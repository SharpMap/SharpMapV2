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
    [Serializable, XmlType(TypeName = "OrientableCurveType", Namespace = "http://www.opengis.net/gml/3.2")]
    public class OrientableCurveType : AbstractCurveType
    {
        [XmlIgnore] private BaseCurve _baseCurve;
        [XmlIgnore] private SignType _orientation;
        [XmlIgnore] public bool OrientationSpecified;

        public OrientableCurveType()
        {
            Orientation = SignType.Positive;
        }

        [XmlElement(Type = typeof (BaseCurve), ElementName = "baseCurve", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public BaseCurve BaseCurve
        {
            get { return _baseCurve; }
            set { _baseCurve = value; }
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
            BaseCurve.MakeSchemaCompliant();
        }
    }
}