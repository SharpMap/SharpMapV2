/*
 *  The attached / following is part of SharpMap.Presentation.Web.SharpLayers
 *  SharpMap.Presentation.Web.SharpLayers is free software © 2008 Newgrove Consultants Limited, 
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
using System;
using System.ComponentModel;
using System.Web.UI;

namespace SharpMap.Presentation.Web.SharpLayers
{
    [Serializable]
    public class MapHostBuilderParams : BuilderParamsBase
    {
        private readonly CollectionBase<DoubleValue> _resolutions = new CollectionBase<DoubleValue>((a, b) => a != b);

        [SharpLayersSerialization(SerializedName = "tileSize",
            SerializationFlags = SharpLayersSerializationFlags.CreateClientClass),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
         PersistenceMode(PersistenceMode.InnerProperty)]
        public Size TileSize { get; set; }

        [SharpLayersSerialization(SerializedName = "projection")]
        public string Projection { get; set; }

        [SharpLayersSerialization(SerializedName = "units")]
        public MapUnits Units { get; set; }

        [SharpLayersSerialization(SerializedName = "resolutions"),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
         PersistenceMode(PersistenceMode.InnerProperty)]
        public CollectionBase<DoubleValue> Resolutions
        {
            get { return _resolutions; }
        }


        [SharpLayersSerialization(SerializedName = "maxResolution")]
        public double? MaxResolution { get; set; }

        [SharpLayersSerialization(SerializedName = "minResolution")]
        public double? MinResolution { get; set; }

        [SharpLayersSerialization(SerializedName = "maxScale")]
        public double? MaxScale { get; set; }

        [SharpLayersSerialization(SerializedName = "minScale")]
        public double? MinScale { get; set; }


        [SharpLayersSerialization(SerializedName = "maxExtent",
            SerializationFlags = SharpLayersSerializationFlags.CreateClientClass),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
         PersistenceMode(PersistenceMode.InnerProperty)]
        public Bounds MaxExtent { get; set; }

        [SharpLayersSerialization(SerializedName = "minExtent",
            SerializationFlags = SharpLayersSerializationFlags.CreateClientClass),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
         PersistenceMode(PersistenceMode.InnerProperty)]
        public Bounds MinExtent { get; set; }

        [SharpLayersSerialization(SerializedName = "restrictedExtent",
            SerializationFlags = SharpLayersSerializationFlags.CreateClientClass),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
         PersistenceMode(PersistenceMode.InnerProperty)]
        public Bounds RestrictedExtent { get; set; }

        [SharpLayersSerialization(SerializedName = "numZoomLevels")]
        private int? NumZoomLevels { get; set; }

        [SharpLayersSerialization(SerializedName = "theme")]
        public string Theme { get; set; }

        [SharpLayersSerialization(SerializedName = "fallThrough")]
        public bool FallThrough { get; set; }
    }
}