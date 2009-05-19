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
    [Serializable, XmlType(TypeName = "ImageCRSType", Namespace = Declarations.SchemaVersion)]
    public class ImageCRSType : AbstractCRSType
    {
        [XmlIgnore] private AffineCSProperty _affineCS;
        [XmlIgnore] private CartesianCSProperty _cartesianCS;
        [XmlIgnore] private ImageDatumProperty _imageDatum;
        [XmlIgnore] private UsesObliqueCartesianCS _usesObliqueCartesianCS;

        [XmlElement(Type = typeof (AffineCSProperty), ElementName = "affineCS", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public AffineCSProperty AffineCS
        {
            get { return _affineCS; }
            set { _affineCS = value; }
        }

        [XmlElement(Type = typeof (CartesianCSProperty), ElementName = "cartesianCS", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public CartesianCSProperty CartesianCS
        {
            get { return _cartesianCS; }
            set { _cartesianCS = value; }
        }

        [XmlElement(Type = typeof (ImageDatumProperty), ElementName = "imageDatum", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public ImageDatumProperty ImageDatum
        {
            get { return _imageDatum; }
            set { _imageDatum = value; }
        }

        [XmlElement(Type = typeof (UsesObliqueCartesianCS), ElementName = "usesObliqueCartesianCS", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public UsesObliqueCartesianCS UsesObliqueCartesianCS
        {
            get { return _usesObliqueCartesianCS; }
            set { _usesObliqueCartesianCS = value; }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
            CartesianCS.MakeSchemaCompliant();
            AffineCS.MakeSchemaCompliant();
            UsesObliqueCartesianCS.MakeSchemaCompliant();
            ImageDatum.MakeSchemaCompliant();
        }
    }
}