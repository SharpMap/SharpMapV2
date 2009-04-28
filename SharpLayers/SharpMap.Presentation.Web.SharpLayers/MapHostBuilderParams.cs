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
using AjaxControlToolkit;

namespace SharpMap.Presentation.Web.SharpLayers
{
    [Serializable]
    public class MapHostBuilderParams : BuilderParamsBase
    {
        private readonly CollectionBase<DoubleValue> _resolutions = new CollectionBase<DoubleValue>(
            delegate(DoubleValue a, DoubleValue b) { return a != b; });

        [ClientPropertyName("tileSize")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)
        , PersistenceMode(PersistenceMode.InnerProperty)]
        public Size TileSize { get; set; }

        [ClientPropertyName("projection")]
        public string Projection { get; set; }

        [ClientPropertyName("units")]
        public MapUnits Units { get; set; }

        [ExtenderControlProperty]
        [ClientPropertyName("resolutions")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)
        , PersistenceMode(PersistenceMode.InnerProperty)]
        public CollectionBase<DoubleValue> Resolutions
        {
            get { return _resolutions; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("maxResolution")]
        public double? MaxResolution { get; set; }

        [ExtenderControlProperty]
        [ClientPropertyName("minResolution")]
        public double? MinResolution { get; set; }

        [ExtenderControlProperty]
        [ClientPropertyName("maxScale")]
        public double? MaxScale { get; set; }

        [ExtenderControlProperty]
        [ClientPropertyName("minScale")]
        public double? MinScale { get; set; }

        [ExtenderControlProperty]
        [ClientPropertyName("maxExtent")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)
        , PersistenceMode(PersistenceMode.InnerProperty)]
        public Bounds MaxExtent { get; set; }

        [ExtenderControlProperty]
        [ClientPropertyName("minExtent")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)
        , PersistenceMode(PersistenceMode.InnerProperty)]
        public Bounds MinExtent { get; set; }

        [ExtenderControlProperty]
        [ClientPropertyName("restrictedExtent")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)
        , PersistenceMode(PersistenceMode.InnerProperty)]
        public Bounds RestrictedExtent { get; set; }

        [ExtenderControlProperty]
        [ClientPropertyName("numZoomLevels")]
        private int? NumZoomLevels { get; set; }

        [ExtenderControlProperty]
        [UrlProperty]
        [ClientPropertyName("theme")]
        public string Theme { get; set; }

        [ExtenderControlProperty]
        [ClientPropertyName("fallThrough")]
        public bool FallThrough { get; set; }
    }
}