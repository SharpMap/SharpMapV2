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
using System.Reflection;
using System.Web.UI;

namespace SharpLayers
{
    public abstract class OpenLayersScripts
    {
        public static IEnumerable<ScriptReference> GetScriptReferences()
        {
            Assembly a = Assembly.GetExecutingAssembly();


            yield return new ScriptReference("SharpLayers.Firebug.firebug.js", a.FullName);
            yield return new ScriptReference("SharpLayers.Firebug.firebugx.js", a.FullName);
            yield return new ScriptReference("SharpLayers.NamespaceInit.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Util.js", a.FullName);

            yield return new ScriptReference("SharpLayers.OpenLayers.Lang.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Lang.cs-CZ.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Lang.de.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Lang.en-CA.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Lang.en.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Lang.fr.js", a.FullName);


            yield return new ScriptReference("SharpLayers.OpenLayers.Filter.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Filter.Comparison.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Filter.FeatureId.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Filter.Logical.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Tween.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Lang.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Projection.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Rule.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Style.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.StyleMap.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.BaseTypes.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.BaseTypes.Bounds.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.BaseTypes.Element.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.BaseTypes.LonLat.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.BaseTypes.Pixel.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.BaseTypes.Size.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Console.js", a.FullName);
            yield return new ScriptReference("SharpLayers.Rico.Corner.js", a.FullName);
            yield return new ScriptReference("SharpLayers.Rico.Color.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Ajax.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Events.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Map.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Layer.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Icon.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Marker.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Marker.Box.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Popup.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Tile.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Tile.Image.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Tile.WFS.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Layer.Image.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Layer.SphericalMercator.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Layer.EventPane.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Layer.FixedZoomLevels.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Layer.Google.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Layer.VirtualEarth.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Layer.Yahoo.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Layer.HTTPRequest.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Layer.Grid.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Layer.MapServer.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Layer.KaMap.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Layer.MultiMap.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Layer.Markers.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Layer.Text.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Layer.WorldWind.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Layer.WMS.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Layer.GeoRSS.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Layer.Boxes.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Layer.TMS.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Layer.TileCache.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Popup.Anchored.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Popup.AnchoredBubble.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Popup.Framed.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Popup.FramedCloud.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Feature.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Feature.Vector.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Feature.WFS.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Handler.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Handler.Point.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Handler.Path.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Handler.Polygon.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Handler.Feature.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Handler.Drag.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Handler.RegularPolygon.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Handler.Box.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Handler.MouseWheel.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Handler.Click.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Handler.Keyboard.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Control.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Control.Button.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Control.Attribution.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Control.ZoomBox.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Control.ZoomToMaxExtent.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Control.DragPan.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Control.Navigation.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Control.NavigationHistory.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Control.OverviewMap.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Control.MouseDefaults.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Control.MousePosition.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Control.OverviewMap.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Control.KeyboardDefaults.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Control.PanZoom.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Control.PanZoomBar.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Control.ArgParser.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Control.Permalink.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Control.Scale.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Control.LayerSwitcher.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Control.DrawFeature.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Control.DragFeature.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Control.ModifyFeature.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Control.Panel.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Control.SelectFeature.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Control.ScaleLine.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Geometry.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Geometry.Rectangle.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Geometry.Collection.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Geometry.Point.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Geometry.MultiPoint.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Geometry.Curve.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Geometry.LineString.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Geometry.LinearRing.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Geometry.Polygon.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Geometry.MultiLineString.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Geometry.MultiPolygon.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Geometry.Surface.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Renderer.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Renderer.Elements.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Renderer.SVG.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Renderer.VML.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Layer.Vector.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Layer.GML.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Format.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Format.XML.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Format.GML.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Format.KML.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Format.GeoRSS.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Format.WFS.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Format.WKT.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Format.JSON.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Format.GeoJSON.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Format.OSM.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Format.SLD.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Format.Text.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Format.WMC.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Layer.WFS.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Layer.MapGuide.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Layer.PointTrack.js", a.FullName);

            yield return new ScriptReference("SharpLayers.OpenLayers.Control.MouseToolbar.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Control.NavToolbar.js", a.FullName);
            yield return new ScriptReference("SharpLayers.OpenLayers.Control.EditingToolbar.js", a.FullName);
        }
    }
}