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
using System.ComponentModel;
using System.Web.UI;

namespace SharpMap.Presentation.Web.SharpLayers.Layers
{
    public class LayerBuilderParamsBase : ILayerBuilderParams
    {
        //[SharpLayersSerialization(SerializedName = "name")]
        //public string Name { get; set; }

        #region ILayerBuilderParams Members

        [SharpLayersSerialization(SerializedName = "isBaseLayer")]
        public bool IsBaseLayer { get; set; }

        [SharpLayersSerialization(SerializedName = "displayInLayerSwitcher")]
        public bool DisplayInLayerSwitcher { get; set; }

        [SharpLayersSerialization(SerializedName = "visibility")]
        public bool Visibility { get; set; }

        [SharpLayersSerialization(SerializedName = "units")]
        public MapUnits Units { get; set; }

        [SharpLayersSerialization(SerializedName = "maxExtent"),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
         PersistenceMode(PersistenceMode.InnerProperty)]
        public Bounds MaxExtent { get; set; }

        [SharpLayersSerialization(SerializedName = "minExtent"),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
         PersistenceMode(PersistenceMode.InnerProperty)]
        public Bounds MinExtent { get; set; }

        [SharpLayersSerialization(SerializedName = "displayOutsideMaxExtent")]
        public bool DisplayOutsideMaxExtent { get; set; }

        [SharpLayersSerialization(SerializedName = "wrapDateLine")]
        public bool WrapDateLine { get; set; }

        [SharpLayersSerialization(SerializedName = "attribution")]
        public string Attribution { get; set; }

        #endregion
    }
}