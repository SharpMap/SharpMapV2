/*
 *  The attached / following is free software © 2008 Newgrove Consultants Limited, 
 *  www.newgrove.com; you can redistribute it and/or modify it under the terms 
 *  of the current GNU Lesser General Public License (LGPL) as published by and 
 *  available from the Free Software Foundation, Inc., 
 *  59 Temple Place, Suite 330, Boston, MA 02111-1307 USA: http://fsf.org/    
 *  This program is distributed without any warranty; 
 *  without even the implied warranty of merchantability or fitness for purpose.  
 *  See the GNU Lesser General Public License for the full details. 
 *  
 *  Author: John Diss 2008
 * 
 */
using System.Collections.Generic;

namespace SharpLayers.Controls
{
    public class OLMapOptions
        : ISerializeToOLJson
    {
        [OLJsonSerialization(SerializedName = "tileSize", SerializationFlags = OLSerializationFlags.CreateOLClass)]
        public OLSize TileSize { get; set; }

        [OLJsonSerialization(SerializedName = "projection")]
        public string Projection { get; set; }

        [OLJsonSerialization(SerializedName = "units")]
        public MapUnits Units { get; set; }

        [OLJsonSerialization(SerializedName = "resolutions")]
        public ICollection<double> Resolutions { get; set; }


        [OLJsonSerialization(SerializedName = "maxResolution")]
        public double? MaxResolution { get; set; }

        [OLJsonSerialization(SerializedName = "minResolution")]
        public double? MinResolution { get; set; }

        [OLJsonSerialization(SerializedName = "maxScale")]
        public double? MaxScale { get; set; }

        [OLJsonSerialization(SerializedName = "minScale")]
        public double? MinScale { get; set; }

        [OLJsonSerialization(SerializedName = "maxExtent", SerializationFlags = OLSerializationFlags.CreateOLClass)]
        public OLBounds MaxExtent { get; set; }

        [OLJsonSerialization(SerializedName = "minExtent", SerializationFlags = OLSerializationFlags.CreateOLClass)]
        public OLBounds MinExtent { get; set; }

        [OLJsonSerialization(SerializedName = "restrictedExtent",
            SerializationFlags = OLSerializationFlags.CreateOLClass)]
        public OLBounds RestrictedExtent { get; set; }

        [OLJsonSerialization(SerializedName = "numZoomLevels")]
        private int? NumZoomLevels { get; set; }

        [OLJsonSerialization(SerializedName = "theme")]
        public string Theme { get; set; }

        [OLJsonSerialization(SerializedName = "fallThrough")]
        public bool FallThrough { get; set; }
    }
}