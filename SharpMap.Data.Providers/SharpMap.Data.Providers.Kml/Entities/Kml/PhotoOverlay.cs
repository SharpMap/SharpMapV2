// /*
//  *  The attached / following is part of SharpMap.Data.Providers.Kml
//  *  SharpMap.Data.Providers.Kml is free software © 2008 Newgrove Consultants Limited, 
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
using System.Xml.Serialization;

namespace SharpMap.Entities.Ogc.Kml
{
    /// <remarks/>
    [Serializable]
    [XmlType(TypeName = "PhotoOverlayType", Namespace = "http://www.opengis.net/kml/2.2")]
    [XmlRoot("PhotoOverlay", Namespace = "http://www.opengis.net/kml/2.2", IsNullable = false)]
    public class PhotoOverlay : OverlayBase
    {
        private ImagePyramid imagePyramidField;
        private KmlObjectBase[] photoOverlayObjectExtensionGroupField;
        private string[] photoOverlaySimpleExtensionGroupField;

        private Point pointField;
        private double rotationField;

        private bool rotationFieldSpecified;

        private ShapeType shapeField;

        private bool shapeFieldSpecified;
        private ViewVolume viewVolumeField;

        public PhotoOverlay()
        {
            rotationField = 0;
            shapeField = ShapeType.Rectangle;
        }

        /// <remarks/>
        public double rotation
        {
            get { return rotationField; }
            set { rotationField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool rotationSpecified
        {
            get { return rotationFieldSpecified; }
            set { rotationFieldSpecified = value; }
        }

        /// <remarks/>
        public ViewVolume ViewVolume
        {
            get { return viewVolumeField; }
            set { viewVolumeField = value; }
        }

        /// <remarks/>
        public ImagePyramid ImagePyramid
        {
            get { return imagePyramidField; }
            set { imagePyramidField = value; }
        }

        /// <remarks/>
        public Point Point
        {
            get { return pointField; }
            set { pointField = value; }
        }

        /// <remarks/>
        public ShapeType shape
        {
            get { return shapeField; }
            set { shapeField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool shapeSpecified
        {
            get { return shapeFieldSpecified; }
            set { shapeFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement("PhotoOverlaySimpleExtensionGroup")]
        public string[] PhotoOverlaySimpleExtensionGroup
        {
            get { return photoOverlaySimpleExtensionGroupField; }
            set { photoOverlaySimpleExtensionGroupField = value; }
        }

        /// <remarks/>
        [XmlElement("PhotoOverlayObjectExtensionGroup")]
        public KmlObjectBase[] PhotoOverlayObjectExtensionGroup
        {
            get { return photoOverlayObjectExtensionGroupField; }
            set { photoOverlayObjectExtensionGroupField = value; }
        }
    }
}