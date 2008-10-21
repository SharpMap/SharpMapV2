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

namespace SharpMap.Presentation.Web.SharpLayers.Layers.Tms
{
    public class TmsLayerBuilderParams : LayerBuilderParamsBase, IGridBasedLayerBuilderParams
    {
        private readonly CollectionBase<ResourceUri> _urls = new CollectionBase<ResourceUri>((a, b) => a.Uri != b.Uri);

        [SharpLayersSerialization(SerializedName = "layername")]
        public string TileCatalogName { get; set; }

        [SharpLayersSerialization(SerializedName = "type")]
        public string ImageExtension { get; set; }

        [SharpLayersSerialization(SerializedName = "url"),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
         PersistenceMode(PersistenceMode.InnerProperty)]
        public CollectionBase<ResourceUri> TmsServerUrls
        {
            get { return _urls; }
        }

        #region IGridBasedLayerBuilderParams Members

        [SharpLayersSerialization(SerializedName = "tileSize"),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
         PersistenceMode(PersistenceMode.InnerProperty)]
        public Size TileSize { get; set; }

        [SharpLayersSerialization(SerializedName = "transitionEffect")]
        public TransitionEffects TransitionEffect { get; set; }

        #endregion
    }
}