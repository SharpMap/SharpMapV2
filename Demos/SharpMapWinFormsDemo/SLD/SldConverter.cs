using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Xml;
using SharpMap.Rendering.Rendering2D;
using SharpMap.Styles;
#if DOTNET35
using Processor = System.Linq.Enumerable;
using Enumerable = System.Linq.Enumerable;
using Caster = System.Linq.Enumerable;
#else
using Processor = GeoAPI.DataStructures.Processor;
using Enumerable = GeoAPI.DataStructures.Enumerable;
using Caster = GeoAPI.DataStructures.Caster;
#endif
namespace MapViewer.SLD
{
    public class SldConverter
    {
        private void SetFillStyle(GeometryStyle style, XmlElement fillSymbolizer, XmlNamespaceManager nsm)
        {
            string stroke = string.Empty;
            string strokeWidth = string.Empty;
            string strokeOpacity = string.Empty;
            string strokeLinejoin = string.Empty;
            string strokeLineCap = string.Empty;
            string strokeDasharray = string.Empty;
            string strokeDashOffset = string.Empty;
            string fill = string.Empty;
            string fillOpacity = string.Empty;
            string pointSymbolPath = string.Empty;

            XmlNodeList polygonFillSymbols = fillSymbolizer.SelectNodes("sld:Fill/sld:CssParameter", nsm);

            foreach (XmlElement polygonFillSymbol in polygonFillSymbols)
            {
                if (polygonFillSymbol != null)
                {
                    switch (polygonFillSymbol.GetAttribute("name"))
                    {
                        //polygon
                        case "fill":
                            fill = polygonFillSymbol.InnerXml;
                            break;
                        case "fill-opacity":
                            fillOpacity = polygonFillSymbol.InnerXml;
                            break;
                    }
                }
            }

            SetStyle(style, stroke, strokeWidth, strokeOpacity, strokeLinejoin, strokeLineCap, strokeDasharray,
                     strokeDashOffset, fill, fillOpacity, pointSymbolPath);


            //Call down to stroke style
            SetStrokeStyle(style, fillSymbolizer, nsm);
        }

        private void SetStrokeStyle(GeometryStyle style, XmlElement StrokeSymbolizer, XmlNamespaceManager nsm)
        {
            string stroke = string.Empty;
            string strokeWidth = string.Empty;
            string strokeOpacity = string.Empty;
            string strokeLinejoin = string.Empty;
            string strokeLineCap = string.Empty;
            string strokeDasharray = string.Empty;
            string strokeDashOffset = string.Empty;
            string fill = string.Empty;
            string fillOpacity = string.Empty;
            string pointSymbolPath = string.Empty;

            XmlNodeList polygonStrokeSymbols = StrokeSymbolizer.SelectNodes("sld:Stroke/sld:CssParameter", nsm);

            foreach (XmlElement polygonStrokeSymbol in polygonStrokeSymbols)
            {
                if (polygonStrokeSymbol != null)
                {
                    switch (polygonStrokeSymbol.GetAttribute("name"))
                    {
                        // line 
                        case "stroke":
                            stroke = polygonStrokeSymbol.InnerXml;
                            break;
                        case "stroke-width":
                            strokeWidth = polygonStrokeSymbol.InnerXml;
                            break;
                        case "stroke-opacity":
                            strokeOpacity = polygonStrokeSymbol.InnerXml;
                            break;
                        case "stroke-linejoin": //“mitre”, “round”, and “bevel”
                            strokeLinejoin = polygonStrokeSymbol.InnerXml;
                            break;
                        case "stroke-linecap": //“butt”, “round”, and “square”.
                            strokeLineCap = polygonStrokeSymbol.InnerXml;
                            break;
                        case "stroke-dasharray":
                            strokeDasharray = polygonStrokeSymbol.InnerXml;
                            break;
                        case "stroke-dashoffset":
                            strokeDashOffset = polygonStrokeSymbol.InnerXml;
                            break;
                    }
                }
            }

            SetStyle(style, stroke, strokeWidth, strokeOpacity, strokeLinejoin, strokeLineCap, strokeDasharray,
                     strokeDashOffset, fill, fillOpacity, pointSymbolPath);
        }

        private void SetPointStyle(GeometryStyle style, XmlElement PointSymbolizer, XmlNamespaceManager nsm)
        {
            string stroke = string.Empty;
            string strokeWidth = string.Empty;
            string strokeOpacity = string.Empty;
            string strokeLinejoin = string.Empty;
            string strokeLineCap = string.Empty;
            string strokeDasharray = string.Empty;
            string strokeDashOffset = string.Empty;
            string fill = string.Empty;
            string fillOpacity = string.Empty;
            string pointSymbolPath = string.Empty;


            XmlNodeList pointSymbols = PointSymbolizer.SelectNodes(
                "sld:Graphic/sld:ExternalGraphic/sld:OnlineResource", nsm);

            foreach (XmlElement pointSymbol in pointSymbols)
            {
                if (pointSymbol != null)
                {
                    pointSymbolPath = pointSymbol.GetAttribute("xlink:href");
                }
            }
            SetStyle(style, stroke, strokeWidth, strokeOpacity, strokeLinejoin, strokeLineCap, strokeDasharray,
                     strokeDashOffset, fill, fillOpacity, pointSymbolPath);
        }


        private void SetSymbologyForRule(GeometryStyle style, XmlElement rule, XmlNamespaceManager nsm)
        {
            XmlNodeList polygonSymbolizers = rule.SelectNodes("sld:PolygonSymbolizer", nsm);
            XmlNodeList lineSymbolizers = rule.SelectNodes("sld:LineSymbolizer", nsm);
            XmlNodeList pointSymbolizers = rule.SelectNodes("sld:PointSymbolizer", nsm);

            if (polygonSymbolizers != null)
                if (polygonSymbolizers.Count > 0)
                {
                    foreach (XmlElement polygonSymbolizer in polygonSymbolizers)
                    {
                        SetFillStyle(style, polygonSymbolizer, nsm);
                    }
                }

            if (lineSymbolizers != null)
                if (lineSymbolizers.Count > 0)
                {
                    foreach (XmlElement lineSymbolizer in lineSymbolizers)
                    {
                        SetStrokeStyle(style, lineSymbolizer, nsm);
                    }
                }

            if (pointSymbolizers != null)
                if (pointSymbolizers.Count > 0)
                {
                    foreach (XmlElement pointSymbolizer in pointSymbolizers)
                    {
                        SetPointStyle(style, pointSymbolizer, nsm);
                    }
                }
        }

        public IDictionary<string, GeometryStyle> ParseFeatureStyleFromXmlText(string xmlText)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlText);
            return ParseFeatureStyle(doc);
        }

        public IDictionary<string, GeometryStyle> ParseFeatureStyleFromFile(string filePath)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(filePath);
            return ParseFeatureStyle(doc);
        }

        public IDictionary<string, GeometryStyle> ParseFeatureStyle(XmlDocument doc)
        {
            var styles = new Dictionary<string, GeometryStyle>();

            // Load SLD file
            var nt = new NameTable();
            var nsm = new XmlNamespaceManager(nt);
            nsm.AddNamespace("sld", "http://www.opengis.net/sld");
            nsm.AddNamespace("ogc", "http://www.opengis.net/ogc");
            nsm.AddNamespace("xlink", "http://www.w3.org/1999/xlink");



            XmlDocument sldConfig = new XmlDocument(nt);
            sldConfig.LoadXml(doc.OuterXml);


            XmlNodeList featureTypeStyleEls = sldConfig.SelectNodes("//sld:FeatureTypeStyle", nsm);


            foreach (XmlElement featTypeStyle in featureTypeStyleEls)
            {

                XmlElement el = (XmlElement)featTypeStyle.SelectSingleNode("sld:FeatureTypeName", nsm);
                string mainName = el != null ? el.InnerText : "";
                XmlNodeList rules = featTypeStyle.SelectNodes("sld:Rule", nsm);

                foreach (XmlElement rule in rules)
                {
                    el = (XmlElement)rule.SelectSingleNode("sld:Name", nsm);
                    string name = el != null ? el.InnerText : "";
                    var style = new GeometryStyle();
                    SetSymbologyForRule(style, rule, nsm);
                    styles.Add(mainName + ":" + name, style);

                }
            }

            return styles;


            //style.AreFeaturesSelectable
            //style.Enabled
            //style.EnableOutline
            //style.Fill
            //style.HighlightFill
            //style.HighlightLine
            //style.HighlightOutline
            //style.HighlightSymbol
            //style.Line
            //style.MaxVisible
            //style.MinVisible
            //style.Outline
            //style.RenderingMode
            //style.SelectFill
            //style.SelectLine
            //style.SelectOutline
            //style.SelectSymbol
        }

        #region "SetStyle"

        private void SetStyle(
            GeometryStyle style,
            string stroke,
            string strokeWidth,
            string strokeOpacity,
            string strokeLinejoin,
            string strokeLineCap,
            string strokeDasharray,
            string strokeDashOffset,
            string fill,
            string fillOpacity,
            string pointSymbolPath
            )
        {
            if (!String.IsNullOrEmpty(stroke))
            {
                Color color = ColorTranslator.FromHtml(stroke);
                int opacity = 255;
                double width = 1;

                if (!String.IsNullOrEmpty(strokeOpacity))
                {
                    opacity = Convert.ToInt32(Math.Round(Convert.ToDouble(strokeOpacity) / 0.0039215, 0));
                    if (opacity > 255)
                        opacity = 255;
                }

                if (!String.IsNullOrEmpty(strokeWidth))
                {
                    width = Convert.ToDouble(strokeWidth);
                }

                StyleBrush brush =
                    new SolidStyleBrush(new StyleColor(Convert.ToInt32(color.B), Convert.ToInt32(color.G),
                                                       Convert.ToInt32(color.R), opacity));
                var pen = new StylePen(brush, width);

                if (!String.IsNullOrEmpty(strokeLinejoin))
                {
                    switch (strokeLinejoin.ToLower())
                    {
                        case "mitre":
                            pen.LineJoin = StyleLineJoin.Miter;
                            break;
                        case "round":
                            pen.LineJoin = StyleLineJoin.Round;
                            break;
                        case "bevel":
                            pen.LineJoin = StyleLineJoin.Bevel;
                            break;

                        //case "miterclipped": // Not in SLD
                        //    pen.LineJoin = StyleLineJoin.MiterClipped;
                        //    break;
                    }
                }

                if (!String.IsNullOrEmpty(strokeLineCap))
                {
                    switch (strokeLineCap.ToLower())
                    {
                        case "butt":
                            pen.StartCap = StyleLineCap.Flat;
                            pen.EndCap = StyleLineCap.Flat;
                            break;
                        case "round":
                            pen.StartCap = StyleLineCap.Round;
                            pen.EndCap = StyleLineCap.Round;
                            break;
                        case "square":
                            pen.StartCap = StyleLineCap.Square;
                            pen.EndCap = StyleLineCap.Square;
                            break;

                        // N.B. Loads of others not used in SLD
                    }
                }

                if (!String.IsNullOrEmpty(strokeDasharray))
                {
                    string[] Numbers = strokeDasharray.Split(Char.Parse(" "));

                    IEnumerable<float> dbls = Processor.Select(Numbers, delegate(string o) { return float.Parse(o); });
                    pen.DashPattern = Enumerable.ToArray(dbls);
                }

                if (!String.IsNullOrEmpty(strokeDashOffset))
                {
                    float dashOffset;
                    bool success;
                    success = float.TryParse(strokeDashOffset, out dashOffset);
                    if (success)
                        pen.DashOffset = dashOffset;
                }

                // Set pen
                style.Line = pen;
            }

            if (!String.IsNullOrEmpty(fill))
            {
                Color color = ColorTranslator.FromHtml(fill);
                int opacity = 255;

                if (!String.IsNullOrEmpty(fillOpacity))
                {
                    opacity = Convert.ToInt32(Math.Round(Convert.ToDouble(fillOpacity) / 0.0039215, 0));
                    if (opacity > 255)
                        opacity = 255;
                }

                StyleBrush brush =
                    new SolidStyleBrush(new StyleColor(Convert.ToInt32(color.B), Convert.ToInt32(color.G),
                                                       Convert.ToInt32(color.R), opacity));

                style.Fill = brush;
            }


            if (!String.IsNullOrEmpty(pointSymbolPath))
            {
                var source = new Uri(pointSymbolPath);

                if (source.IsFile && File.Exists(source.AbsolutePath))
                {
                    var b = new Bitmap(source.AbsolutePath);

                    var ms = new MemoryStream();
                    b.Save(ms, ImageFormat.Png);
                    ms.Seek(0, SeekOrigin.Begin);

                    style.Symbol = new Symbol2D(ms, new Size2D(12, 10));
                }
                else if (source.IsAbsoluteUri)
                {
                    ///TODO
                }
            }

            style.Enabled = true;
            style.EnableOutline = true;
        }

        #endregion
    }
}