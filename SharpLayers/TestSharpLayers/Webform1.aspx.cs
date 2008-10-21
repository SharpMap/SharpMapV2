using System;
using System.Configuration;
using System.Linq;
using System.Web.UI;
using SharpMap.Presentation.Web.SharpLayers;
using SharpMap.Presentation.Web.SharpLayers.Layers.Tms;

namespace TestSharpLayers
{
    public partial class Webform1 : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Panel1_MapHostExtender.BuilderParams = new MapHostBuilderParams
                                                       {
                                                           TileSize =
                                                               new Size
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
                                                               = new Bounds
                                                                     {
                                                                         Left = 0,
                                                                         Bottom = 0,
                                                                         Right = 2219485.87,
                                                                         Top = 2219485.87
                                                                     },
                                                           Theme = "test theme",
                                                           Projection = "EPSG:27700"
                                                       };

            Panel1_MapHostExtender.BuilderParams.Resolutions.AddRange(new[]
                                                                          {
                                                                              8669.866667, 4334.933333, 2167.466667,
                                                                              1083.733333,
                                                                              541.8666667, 270.9333333, 135.4666667,
                                                                              67.73333333,
                                                                              33.86666667, 16.93333333, 8.466666667,
                                                                              4.233333333,
                                                                              2.116666667, 1.058333333, 0.529166667,
                                                                              0.264583333
                                                                          }.Select(o => new Resolution { Value = o }));
            Panel1_MapHostExtender.LayerComponents.FindByType<TmsLayerComponent>().First().BuilderParams
                = new TmsLayerBuilderParams
                      {
                          TileCatalogName = "OS",
                          DisplayInLayerSwitcher = true,
                          IsBaseLayer = true,
                          Visibility = true,
                          WrapDateLine = false,
                          Units = MapUnits.m,
                          TileSize = Panel1_MapHostExtender.BuilderParams.TileSize,
                          MaxExtent = Panel1_MapHostExtender.BuilderParams.MaxExtent,
                          ImageExtension = "png",
                          Attribution = "&copy; John Diss 2008"
                      };


            Panel1_MapHostExtender.LayerComponents.FindByType<TmsLayerComponent>().First().BuilderParams.TmsServerUrls.
                AddRange(
                ConfigurationManager.AppSettings["TmsServerUrl"].Split(',').Select(
                    o => new ResourceUri { Uri = new Uri(o) }));
        }
    }
}