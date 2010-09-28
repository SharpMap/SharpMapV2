using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SharpMap;
using SharpMap.Styles;
using SharpMap.Utilities;
using SharpMap.Utilities.SridUtility;

namespace SharpMapWpfDemo
{
    /// <summary>
    /// Interaktionslogik für Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
            SridMap.DefaultInstance = new SridMap(new SridMapStrategyBase[] { new SridProj4Strategy(0, new GeometryServices().CoordinateSystemFactory)});
            var gs = new GeometryServices();
            Map map = new Map("WpfSample", gs["EPSG:4326"], gs.CoordinateTransformationFactory);
            //wpfControl.Width = 
            AddLayers(map);
            wpfControl.Map = map;
            wpfControl.ZoomToExtents();
        }

        private static void AddLayers(Map map)
        {
            string[] layers = new [] { "Countries", "Rivers", "Cities"};
            GeometryStyle[] s = new []
                { new GeometryStyle { Fill = new SolidStyleBrush(StyleColor.DarkGreen), Outline = new StylePen() },
                  new GeometryStyle { Line = new StylePen(new SolidStyleBrush(StyleColor.DarkSeaGreen), 2d), Outline = new StylePen(new SolidStyleBrush(StyleColor.Blue), 4d)},
                  new GeometryStyle()};
            int i = 0;
            foreach (string layer in layers)
            {
                var prov = new SharpMap.Data.Providers.SpatiaLite2Provider(new GeometryServices()["EPSG:4326"],
                                                                           @"Data Source=sample.sqlite", "main", layer, "PK_UID", "geom");
                var fl = new SharpMap.Layers.GeometryLayer(layer, s[i++],prov);
                map.Layers.Add(fl);
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateLayout();
        }
    }
}
