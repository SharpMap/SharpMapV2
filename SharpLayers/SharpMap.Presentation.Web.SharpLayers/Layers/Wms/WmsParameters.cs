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


using System.ComponentModel;
using System.Web.UI;

namespace SharpMap.Presentation.Web.SharpLayers.Layers.Wms
{
    public class WmsParameters
    {
        private readonly CollectionBase<StringValue> _layers = new CollectionBase<StringValue>((a, b) => a.Value != b.Value);
        private readonly CollectionBase<UriValue> _urls = new CollectionBase<UriValue>((a, b) => a.Value != b.Value);
        private string _imageMimeType = "image/png";
        private bool _transaparent = true;
        private string _wmsVersion = "1.3.0";

        [SharpLayersSerialization(SerializedName = "version")]
        public string WmsVersion
        {
            get { return _wmsVersion; }
            set { _wmsVersion = value; }
        }

        [
            SharpLayersSerialization(SerializedName = "url"),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
            PersistenceMode(PersistenceMode.InnerProperty)
        ]
        public CollectionBase<UriValue> WmsServerUrls
        {
            get { return _urls; }
        }

        [
            SharpLayersSerialization(SerializedName = "layers"),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
            PersistenceMode(PersistenceMode.InnerProperty)
        ]
        public CollectionBase<StringValue> WmsLayerNames
        {
            get { return _layers; }
            //set { _layers = value; }
        }

        [SharpLayersSerialization(SerializedName = "crs")]
        public string Crs { get; set; }

        [SharpLayersSerialization(SerializedName = "TRANSPARENT")]
        public bool Transaparent
        {
            get { return _transaparent; }
            set { _transaparent = value; }
        }

        [SharpLayersSerialization(SerializedName = "FORMAT")]
        public string ImageMimeType
        {
            get { return _imageMimeType; }
            set { _imageMimeType = value; }
        }
    }
}