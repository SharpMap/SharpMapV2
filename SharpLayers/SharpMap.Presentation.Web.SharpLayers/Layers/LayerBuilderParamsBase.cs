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
using AjaxControlToolkit;

namespace SharpMap.Presentation.Web.SharpLayers.Layers
{
    public class LayerBuilderParamsBase : ILayerBuilderParams
    {
        private readonly CollectionBase<DoubleValue> _resolutions = new CollectionBase<DoubleValue>(
            delegate(DoubleValue a, DoubleValue b) { return a != b; });
        private readonly CollectionBase<DoubleValue> _scales = new CollectionBase<DoubleValue>(
            delegate(DoubleValue a, DoubleValue b) { return a != b; });

        #region ILayerBuilderParams Members

        [ExtenderControlProperty]
        [ClientPropertyName("alwaysInRange")]
        public bool AlwaysInRange { get; set; }

        [ExtenderControlProperty]
        [ClientPropertyName("resolutions")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
         PersistenceMode(PersistenceMode.InnerProperty)]
        public CollectionBase<DoubleValue> Resolutions
        {
            get { return _resolutions; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("scales")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
         PersistenceMode(PersistenceMode.InnerProperty)]
        public CollectionBase<DoubleValue> Scales
        {
            get { return _scales; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("minScale")]
        public double? MinScale { get; set; }

        [ExtenderControlProperty]
        [ClientPropertyName("maxScale")]
        public double? MaxScale { get; set; }

        [ExtenderControlProperty]
        [ClientPropertyName("minResolution")]
        public double? MinResolution { get; set; }

        [ExtenderControlProperty]
        [ClientPropertyName("maxResolution")]
        public double? MaxResolution { get; set; }

        [ExtenderControlProperty]
        [ClientPropertyName("gutter")]
        public int? Gutter { get; set; }

        [ExtenderControlProperty]
        [ClientPropertyName("isBaseLayer")]
        public bool IsBaseLayer { get; set; }

        [ExtenderControlProperty]
        [ClientPropertyName("displayInLayerSwitcher")]
        public bool DisplayInLayerSwitcher { get; set; }

        [ExtenderControlProperty]
        [ClientPropertyName("visibility")]
        public bool Visibility { get; set; }

        [ExtenderControlProperty]
        [ClientPropertyName("units")]
        public MapUnits Units { get; set; }

        [ExtenderControlProperty]
        [ClientPropertyName("maxExtent")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
         PersistenceMode(PersistenceMode.InnerProperty)]
        public Bounds MaxExtent { get; set; }

        [ExtenderControlProperty]
        [ClientPropertyName("minExtent")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
         PersistenceMode(PersistenceMode.InnerProperty)]
        public Bounds MinExtent { get; set; }

        [ExtenderControlProperty]
        [ClientPropertyName("displayOutsideMaxExtent")]
        public bool DisplayOutsideMaxExtent { get; set; }

        [ExtenderControlProperty]
        [ClientPropertyName("wrapDateLine")]
        public bool WrapDateLine { get; set; }

        [ExtenderControlProperty]
        [ClientPropertyName("attribution")]
        public string Attribution { get; set; }

        #endregion
    }
}