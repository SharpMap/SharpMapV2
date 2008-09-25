/*
 *	This file is part of SharpLayers
 *  SharpLayers is free software © 2008 Newgrove Consultants Limited, 
 *  http://www.newgrove.com; you can redistribute it and/or modify it under the terms 
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
namespace SharpLayers.Layers
{
    public abstract class LayerOptionsBase
        : ILayerOptions
    {
        protected LayerOptionsBase()
        {
            DisplayInLayerSwitcher = true;
            DisplayOutsideMaxExtent = true;
            Units = MapUnits.m;
        }

        #region ILayerOptions Members

        [OLJsonSerialization(SerializedName = "isBaseLayer")]
        public virtual bool IsBaseLayer { get; set; }

        [OLJsonSerialization(SerializedName = "displayInLayerSwitcher")]
        public bool DisplayInLayerSwitcher { get; set; }

        [OLJsonSerialization(SerializedName = "visibility")]
        public bool Visibility { get; set; }


        [OLJsonSerialization(SerializedName = "units")]
        public MapUnits Units { get; set; }

        [OLJsonSerialization(SerializedName = "maxExtent", SerializationFlags = OLSerializationFlags.CreateOLClass)]
        public OLBounds MaxExtent { get; set; }

        [OLJsonSerialization(SerializedName = "minExtent")]
        public OLBounds MinExtent { get; set; }

        [OLJsonSerialization(SerializedName = "displayOutsideMaxExtent")]
        public bool DisplayOutsideMaxExtent { get; set; }


        [OLJsonSerialization(SerializedName = "wrapDateLine")]
        public bool WrapDateLine { get; set; }


        [OLJsonSerialization(SerializedName = "attribution")]
        public string Attribution { get; set; }

        #endregion

        public virtual string ToJSON()
        {
            return OLJsonSerializer.Serialize(this);
        }
    }
}