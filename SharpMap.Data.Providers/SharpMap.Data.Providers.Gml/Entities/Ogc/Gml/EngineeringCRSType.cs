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
    [Serializable, XmlType(TypeName = "EngineeringCRSType", Namespace = "http://www.opengis.net/gml/3.2")]
    public class EngineeringCRSType : AbstractCRSType
    {
        [XmlIgnore] private AffineCSProperty _affineCS;
        [XmlIgnore] private CartesianCSProperty _cartesianCS;
        [XmlIgnore] private CoordinateSystemProperty _coordinateSystem;
        [XmlIgnore] private CylindricalCSProperty _cylindricalCS;
        [XmlIgnore] private EngineeringDatumProperty _engineeringDatum;
        [XmlIgnore] private LinearCSProperty _linearCS;
        [XmlIgnore] private PolarCSProperty _polarCS;
        [XmlIgnore] private SphericalCSProperty _sphericalCS;
        [XmlIgnore] private UserDefinedCSProperty _userDefinedCS;

        [XmlElement(Type = typeof (AffineCSProperty), ElementName = "affineCS", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public AffineCSProperty AffineCS
        {
            get { return _affineCS; }
            set { _affineCS = value; }
        }

        [XmlElement(Type = typeof (CartesianCSProperty), ElementName = "cartesianCS", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public CartesianCSProperty CartesianCS
        {
            get { return _cartesianCS; }
            set { _cartesianCS = value; }
        }

        [XmlElement(Type = typeof (CoordinateSystemProperty), ElementName = "coordinateSystem", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public CoordinateSystemProperty CoordinateSystem
        {
            get { return _coordinateSystem; }
            set { _coordinateSystem = value; }
        }

        [XmlElement(Type = typeof (CylindricalCSProperty), ElementName = "cylindricalCS", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public CylindricalCSProperty CylindricalCS
        {
            get { return _cylindricalCS; }
            set { _cylindricalCS = value; }
        }

        [XmlElement(Type = typeof (EngineeringDatumProperty), ElementName = "engineeringDatum", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public EngineeringDatumProperty EngineeringDatum
        {
            get { return _engineeringDatum; }
            set { _engineeringDatum = value; }
        }

        [XmlElement(Type = typeof (LinearCSProperty), ElementName = "linearCS", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public LinearCSProperty LinearCS
        {
            get { return _linearCS; }
            set { _linearCS = value; }
        }

        [XmlElement(Type = typeof (PolarCSProperty), ElementName = "polarCS", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public PolarCSProperty PolarCS
        {
            get { return _polarCS; }
            set { _polarCS = value; }
        }

        [XmlElement(Type = typeof (SphericalCSProperty), ElementName = "sphericalCS", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public SphericalCSProperty SphericalCS
        {
            get { return _sphericalCS; }
            set { _sphericalCS = value; }
        }

        [XmlElement(Type = typeof (UserDefinedCSProperty), ElementName = "userDefinedCS", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public UserDefinedCSProperty UserDefinedCS
        {
            get { return _userDefinedCS; }
            set { _userDefinedCS = value; }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
            AffineCS.MakeSchemaCompliant();
            CartesianCS.MakeSchemaCompliant();
            CylindricalCS.MakeSchemaCompliant();
            LinearCS.MakeSchemaCompliant();
            PolarCS.MakeSchemaCompliant();
            SphericalCS.MakeSchemaCompliant();
            UserDefinedCS.MakeSchemaCompliant();
            CoordinateSystem.MakeSchemaCompliant();
            EngineeringDatum.MakeSchemaCompliant();
        }
    }
}