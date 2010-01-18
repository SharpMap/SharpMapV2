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
 *  Author: John Diss 2009
 * 
 */
using AjaxControlToolkit;

namespace SharpMap.Presentation.Web.SharpLayers.Controls.Measure
{
    public enum MeasureToolMode
    {
        Area,
        Distance
    }

    public enum MeasureToolUnits
    {
        English,
        Metric,
        Geographic
    }

    public class MeasureToolBuilderParams : ToolBuilderParamsBase
    {
        private MeasureToolMode _mode = MeasureToolMode.Distance;
        private MeasureToolUnits _unit = MeasureToolUnits.Metric;

        [ExtenderControlProperty]
        [ClientPropertyName("mode")]
        [ComponentReference]
        public MeasureToolMode Mode
        {
            get { return _mode; }
            set { _mode = value; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("handler")]
        public string Handler
        {
            get { return Mode == MeasureToolMode.Distance ? "OpenLayers.Handler.Path" : "OpenLayers.Handler.Polygon"; }
        }

        public MeasureToolUnits Units
        {
            get { return _unit; }
            set { _unit = value; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("displaySystem")]
        public string DisplaySystem
        {
            get
            {
                switch (Units)
                {
                    case MeasureToolUnits.English:
                        return "english";
                    case MeasureToolUnits.Geographic:
                        return "geographic";
                    default:
                        return "metric";
                }
            }
        }


        [ExtenderControlProperty]
        [ClientPropertyName("geodesic")]
        public bool IsGeodesic { get; set; }


        [ExtenderControlProperty]
        [ClientPropertyName("persist")]
        public bool Persist { get; set; }

        [ExtenderControlProperty]
        [ClientPropertyName("sld")]
        [ComponentReference]
        public string Sld { get; set; }

        [ExtenderControlProperty]
        [ClientPropertyName("styleSelector")]
        public string StyleSelector { get; set; }

        [ExtenderControlProperty]
        [ClientPropertyName("onClientMeasure")]
        public string OnClientMeasure { get; set; }
    }
}