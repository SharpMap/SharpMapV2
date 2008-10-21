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
using System;
using System.Configuration;
using System.Web.UI;
using SharpLayers;
using SharpLayers.Controls;
using SharpLayers.Layers;

namespace TestSharpLayers
{
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Map1.Options =
                new OLMapOptions
                    {
                        TileSize =
                            new OLSize
                                {
                                    Width = 256,
                                    Height = 256
                                },
                        Units = MapUnits.m,
                        MaxExtent
                            //= new Bounds
                            //      {
                            //          Left = -981932.997723,
                            //          Bottom = 6415360.178803,
                            //          Right = 240784.582575,
                            //          Top = 8610694.016157
                            //      },
                            //= new Bounds
                            //      {
                            //          Left = -1167.99914774828,
                            //          Bottom = -82031.999329218,
                            //          Right = 655990.002157278,
                            //          Top = 1218644.99575382
                            //      },
                            = new OLBounds
                                  {
                                      Left = 0,
                                      Bottom = 0,
                                      Right = 2219485.87,
                                      Top = 2219485.87
                                  },
                        Resolutions =
                            new[]
                                {
                                    8669.866667, 4334.933333, 2167.466667, 1083.733333, 541.8666667, 270.9333333,
                                    135.4666667, 67.73333333, 33.86666667, 16.93333333, 8.466666667, 4.233333333,
                                    2.116666667, 1.058333333, 0.529166667, 0.264583333
                                }
                    };


            //var g = new GoogleLayer();
            //g.Id = "myFirstGoogleLayer";
            //g.Name = "gurgle";
            //g.Options =
            //    new GoogleLayerOptions
            //        {
            //        };


            var scaleControl = new ScaleControl
                                   {
                                       Options = new ControlOptions
                                                     {
                                                         TargetDivId = "scaleDiv"
                                                     }
                                   };


            //var tms = new TmsLayer
            //              {
            //                  Id = "tms",
            //                  Name = "My TMS LayerComponent",
            //                  Options = new TmsLayerOptions
            //                                {
            //                                    TileCatalogName = "OS",
            //                                    DisplayInLayerSwitcher = true,
            //                                    IsBaseLayer = true,
            //                                    Visibility = true,
            //                                    WrapDateLine = false,
            //                                    Units = MapUnits.m,
            //                                    TileSize = Map1.Options.TileSize,
            //                                    MaxExtent = Map1.Options.MaxExtent,
            //                                    ImageExtension = "png",
            //                                    Attribution = "&copy; John Diss 2008"
            //                                },
            //                  Urls = ConfigurationManager.AppSettings["TmsServerUrl"].Split(',')
            //              };


            var wms = new WmsLayer
                          {
                              Id = "myWmsLayer",
                              Name = "WMS",
                              Options = new WmsLayerOptions
                                            {
                                                WmsParameters = new WmsParameters
                                                                    {
                                                                        Urls =
                                                                            ConfigurationManager.AppSettings[
                                                                            "WmsServerUrl"].Split(','),
                                                                        Layers =
                                                                            ConfigurationManager.AppSettings[
                                                                            "LayerNames"].Split(','),
                                                                        WmsVersion = "1.3.0",
                                                                        Crs = "EPSG:"
                                                                    },
                                                TileSize = Map1.Options.TileSize,
                                                MaxExtent = Map1.Options.MaxExtent,
                                                IsBaseLayer = true,
                                                Visibility = true,
                                                Units = MapUnits.m,
                                                DisplayInLayerSwitcher = true,
                                                DisplayOutsideMaxExtent = true,
                                                WrapDateLine = false,
                                                Attribution = "&copy; Newgrove 2008",
                                                TransitionEffect = TransitionEffects.resize
                                            },
                          };

            //var vectorLayer = new VectorLayer
            //                      {
            //                          Id = "vector1",
            //                          Name = "my Vector LayerComponent",
            //                          Options = new VectorLayerOptions
            //                                        {
            //                                            Attribution = "You!",
            //                                            DisplayInLayerSwitcher = true,
            //                                            MaxExtent = Map1.Options.MaxExtent
            //                                        }
            //                      };

            var layerSwitcher = new LayerSwitcher
                                    {
                                        Id = "layerSwitcher1",
                                        Options = new LayerSwitcherOptions()
                                    };

            var mousePosition = new MousePosition
                                    {
                                        Id = "mousePosition1",
                                        Options = new ControlOptions
                                                      {
                                                          TargetDivId = "coordDiv"
                                                      }
                                    };
            var attribution = new Attribution
                                  {
                                      Options = new ControlOptions
                                                    {
                                                        TargetDivId = "attribution1"
                                                    }
                                  };

            //var editingToolbar = new EditingToolbar
            //                         {
            //                             EditableLayerId = vectorLayer.Id,
            //                             Options = new ControlOptions
            //                                           {
            //                                               TargetDivId = "navtoolbar1"
            //                                           }
            //                         };

            var keyboardDefaults = new KeyboardDefaults
                                       {
                                           Id = "keyboardDefs",
                                           Options = new ControlOptions()
                                       };

            var panZoomBar = new PanZoomBar
                                 {
                                     Id = "myPanZoomBar",
                                     Options = new PanZoomBarOptions
                                                   {
                                                       ZoomWorldIcon = true,
                                                       ZoomStopWidth = 18,
                                                       ZoomStopHeight = 20
                                                   }
                                 };

            /*
             * 
             *Markers don't work just yet... 
             * 
             */
            //var marker = new Marker
            //                 {
            //                     Id = "myMarker",
            //                     LonLat = new LonLat
            //                                  {
            //                                      Lon = (Map1.Options.MaxExtent.Left + Map1.Options.MaxExtent.Right) / 2,
            //                                      Lat = (Map1.Options.MaxExtent.Bottom + Map1.Options.MaxExtent.Top) / 2
            //                                  },
            //                     Icon = new Icon
            //                                {
            //                                    Offset = new Pixel
            //                                                 {
            //                                                     X = 0,
            //                                                     Y = 0
            //                                                 },
            //                                    Url = "img/marker-blue.png",
            //                                    Size = new Size
            //                                               {
            //                                                   Height = 25,
            //                                                   Width = 25
            //                                               }
            //                                }
            //                 };


            //var markerLayer = new MarkerLayer
            //                      {
            //                          Id = "myMarkers",
            //                          Name = "myMarkers",
            //                          Markers = new[] { marker },
            //                          Options = new MarkerLayerOptions
            //                                        {
            //                                            MaxExtent = Map1.Options.MaxExtent,
            //                                            DisplayInLayerSwitcher = true,
            //                                            Attribution = "Just Me 2008",
            //                                            Visibility = true
            //                                        }
            //                      };

            //Map1.LayerComponent.Add(g);


            //Map1.LayerComponent.Add(tms);

            Map1.Layers.Add(wms);

            //Map1.LayerComponent.Add(vectorLayer);

            //Map1.LayerComponent.Add(markerLayer);

            Map1.BaseLayerId = wms.Id;

            Map1.OLControls.Add(panZoomBar);
            Map1.OLControls.Add(scaleControl);
            Map1.OLControls.Add(layerSwitcher);
            Map1.OLControls.Add(mousePosition);
            Map1.OLControls.Add(attribution);
            Map1.OLControls.Add(keyboardDefaults);
            //Map1.OLControls.Add(editingToolbar);
        }
    }
}