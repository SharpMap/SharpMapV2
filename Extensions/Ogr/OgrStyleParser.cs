using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using SharpMap.Styles;
using OgrLayerDefn = OSGeo.OGR.FeatureDefn;
using OgrFeature = OSGeo.OGR.Feature;

namespace SharpMap.Data.Providers
{
    enum OgrStyleToolUnit
    {
// ReSharper disable InconsistentNaming
        g, 
        px, 
        pt, 
        mm, 
        cm, 
        inch
// ReSharper restore InconsistentNaming
    }

    public static class OgrStyleParser
    {
        private static readonly Dictionary<string, IStyle> ParsedStyles = new Dictionary<string, IStyle>();

        private static void AddStyle(string ogrStyleString, IStyle value)
        {
            ParsedStyles.Add(ogrStyleString, value);
        }

        private static bool IsColumnName(string descriptor)
        {
            return descriptor.StartsWith("{") && descriptor.EndsWith("}");
        }

        private static string ExtractColumnName(string decoratedColumnName)
        {
            return decoratedColumnName.Substring(1, decoratedColumnName.Length - 2);
        }

        public static IStyle Parse(OgrLayerDefn ogrLayer, OgrFeature ogrFeature)
        {
            IStyle result;
            string ogrStyleString = ogrFeature.GetStyleString();

            if (string.IsNullOrEmpty(ogrStyleString))
                return null;

            if (IsColumnName(ogrStyleString))
                ogrStyleString = ogrFeature.GetFieldAsString(ExtractColumnName(ogrStyleString));

            if (ParsedStyles.TryGetValue(ogrStyleString, out result))
                return result;

            foreach (string s in ogrStyleString.Split(new [] {';'}))
            {
                int pos = s.IndexOf("(") - 1;
                switch (s.Substring(0, pos))
                {
                    case "PEN":
                        ParsePenStyle(result, s.Substring(pos + 2, s.Length - pos - 3));
                        break;
                    case "BRUSH":
                        ParseBrushStyle(result, s.Substring(pos + 2, s.Length - pos - 3));
                        break;
                    case "SYMBOL":
                        ParseSymbolStyle(result, s.Substring(pos + 2, s.Length - pos - 3));
                        break;
                    case "LABEL":
                        ParseLabelStyle(result, s.Substring(pos + 2, s.Length - pos - 3));
                        break;
                }
            }

            AddStyle(ogrStyleString, result);
            return result;
        }

        private static void ParseLabelStyle(IStyle result, string styleDefinition)
        {
            ;
        }

        private static void ParseSymbolStyle(IStyle result, string styleDefinition)
        {
            ;
        }

        private static void ParseBrushStyle(IStyle result, string styleDefinition)
        {
            ;
        }

        private static void ParsePenStyle(IStyle result, string styleDefinition)
        {
            OgrStyleToolUnit unit;

            StylePen pen = new StylePen();
            foreach (string styleToolParameter in StyleToolParameters(styleDefinition))
            {
                string[] toolDefinition = styleToolParameter.Split(':');
                switch (toolDefinition[0])
                {
                    case "c":
                        pen.BackgroundBrush = new SolidStyleBrush(StyleColorFromHexString(toolDefinition[1]));
                        break;

                    case "w":
                        double value;
                        ValueUnit(toolDefinition[1], out value, out unit);
                        //ToDo: take unit into account
                        pen.Width = value;
                        break;

                    case "p":
                        pen.DashPattern = DecodePattern(toolDefinition[1], out unit);
                        break;

                    case "id":
                        float[] dashes;
                        pen.DashStyle = DecodePenId(toolDefinition[1], out dashes);
                        if (pen.DashStyle == LineDashStyle.Custom)
                            pen.DashPattern = dashes;
                        break;

                    case "cap":
                        pen.StartCap = DecodePenCap(toolDefinition[1]);
                        pen.EndCap = pen.StartCap;
                        break;

                    case "j":
                        pen.LineJoin = DecodePenJoin(toolDefinition[1]);
                        break;

                    case "dp":
                    case "l":
                    default:
                        break;
                }
            }
        }

        private static StyleLineJoin DecodePenJoin(string dashCap)
        {
            switch (dashCap)
            {
                case "r":
                    return StyleLineJoin.Round;
                case "b":
                    return StyleLineJoin.Bevel;
                default:
                    return StyleLineJoin.Miter;

            }
        }
        private static StyleLineCap DecodePenCap(string dashCap)
        {
            switch (dashCap)
            {
                case "r":
                    return StyleLineCap.Round;
                default:
                    return StyleLineCap.Flat;

            }
        }

        private static LineDashStyle DecodePenId(string id, out float[] dashes)
        {
            dashes = null;
            id = id.Substring(1, id.Length-2);
            string[] ids = id.Split(',');
            foreach (string s in ids)
            {
                switch (s)
                {
                    case "ogr-pen-0":
                        return LineDashStyle.Solid;
                    case "ogr-pen-1":
                        dashes = new float[0];
                        return LineDashStyle.Custom;
                    case "ogr-pen-2":
                        return LineDashStyle.Dash;
                    case "ogr-pen-3":
                        dashes = new float[] { 3f, 3f };
                        return LineDashStyle.Custom;
                    case "ogr-pen-4":
                        dashes = new float[] { 7f, 7f };
                        return LineDashStyle.Custom;
                    case "ogr-pen-5":
                        return LineDashStyle.Dot;
                    case "ogr-pen-6":
                        return LineDashStyle.DashDot;
                    case "ogr-pen-7": //
                        return LineDashStyle.DashDotDot;
                    case "ogr-pen-8": // alternate line
                        dashes = new float[] {1f, 1f};
                        return LineDashStyle.Custom;
                    default:
                        //ToDo call delegate?
                        break;
                }
            }
            return LineDashStyle.Solid;
        }

        private static float[] DecodePattern(string input, out OgrStyleToolUnit unit)
        {
            unit = OgrStyleToolUnit.px;
            input = input.Substring(1, input.Length - 1);
            string[] patternParts = input.Split(' ');
            float[] result = new float[patternParts.Length];
            for (int i = 0; i < patternParts.Length; i++)
            {
                double value;
                ValueUnit(patternParts[i], out value, out unit);
                result[i] = (float) value;
            }
            return result;
        }

        private static void ValueUnit(string input, out double value, out OgrStyleToolUnit unit)
        {
            if (input.EndsWith("g"))
            {
                unit = OgrStyleToolUnit.g;
                input = input.Substring(0, input.Length - 1);
            }
            else
            {
                if (input.EndsWith("px"))
                    unit = OgrStyleToolUnit.px;
                else if (input.EndsWith("pt"))
                    unit = OgrStyleToolUnit.pt;
                else if (input.EndsWith("px"))
                    unit = OgrStyleToolUnit.mm;
                else if (input.EndsWith("px"))
                    unit = OgrStyleToolUnit.cm;
                else
                    unit = OgrStyleToolUnit.inch;
                input = input.Substring(0, input.Length - 2);
            }
            if (!double.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                value = 1;
        }

        private static StyleColor StyleColorFromHexString(string hexString)
        {
            if (hexString.Length == 7)
                hexString += "FF";
            byte[] rgba = Convert.FromBase64String(hexString.Substring(1));
            return new StyleColor(rgba[2], rgba[1], rgba[0], rgba[3]);
        }

        private static IEnumerable<String> StyleToolParameters(IEnumerable<char> styleDef)
        {
            StringBuilder result = new StringBuilder();

            bool noQuoute = true;
            foreach (char c in styleDef)
            {
                switch (c)
                {
                    case ',':
                        if(noQuoute)
                        {
                            yield return result.ToString();
                            result = new StringBuilder();
                        }
                        else
                            result.Append(c);
                        break;
                    case '"':
                        noQuoute = !noQuoute;
                        result.Append(c);
                        break;
                    default:
                        result.Append(c);
                        break;
                }
            }
            yield return result.ToString();
        }
    }
}