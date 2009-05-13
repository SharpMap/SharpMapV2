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
    [XmlType(TypeName = "ImagePyramidType", Namespace = "http://www.opengis.net/kml/2.2")]
    [XmlRoot("ImagePyramid", Namespace = "http://www.opengis.net/kml/2.2", IsNullable = false)]
    public class ImagePyramid : KmlObjectBase
    {
        private GridOrigin gridOriginField;

        private bool gridOriginFieldSpecified;

        private KmlObjectBase[] imagePyramidObjectExtensionGroupField;
        private string[] imagePyramidSimpleExtensionGroupField;
        private int maxHeightField;

        private bool maxHeightFieldSpecified;
        private int maxWidthField;

        private bool maxWidthFieldSpecified;
        private int tileSizeField;

        private bool tileSizeFieldSpecified;

        public ImagePyramid()
        {
            tileSizeField = 256;
            maxWidthField = 0;
            maxHeightField = 0;
            gridOriginField = GridOrigin.LowerLeft;
        }

        /// <remarks/>
        public int tileSize
        {
            get { return tileSizeField; }
            set { tileSizeField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool tileSizeSpecified
        {
            get { return tileSizeFieldSpecified; }
            set { tileSizeFieldSpecified = value; }
        }

        /// <remarks/>
        public int maxWidth
        {
            get { return maxWidthField; }
            set { maxWidthField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool maxWidthSpecified
        {
            get { return maxWidthFieldSpecified; }
            set { maxWidthFieldSpecified = value; }
        }

        /// <remarks/>
        public int maxHeight
        {
            get { return maxHeightField; }
            set { maxHeightField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool maxHeightSpecified
        {
            get { return maxHeightFieldSpecified; }
            set { maxHeightFieldSpecified = value; }
        }

        /// <remarks/>
        public GridOrigin gridOrigin
        {
            get { return gridOriginField; }
            set { gridOriginField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool gridOriginSpecified
        {
            get { return gridOriginFieldSpecified; }
            set { gridOriginFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement("ImagePyramidSimpleExtensionGroup")]
        public string[] ImagePyramidSimpleExtensionGroup
        {
            get { return imagePyramidSimpleExtensionGroupField; }
            set { imagePyramidSimpleExtensionGroupField = value; }
        }

        /// <remarks/>
        [XmlElement("ImagePyramidObjectExtensionGroup")]
        public KmlObjectBase[] ImagePyramidObjectExtensionGroup
        {
            get { return imagePyramidObjectExtensionGroupField; }
            set { imagePyramidObjectExtensionGroupField = value; }
        }
    }
}