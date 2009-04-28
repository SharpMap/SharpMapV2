/*
 *  The attached / following is part of SharpMap.Presentation.AspNet
 *  SharpMap.Presentation.AspNet is free software © 2008 Newgrove Consultants Limited, 
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
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using SharpMap.Rendering.Gdi;
using SharpMap.Rendering.Rendering2D;
using SharpMap.Styles;

namespace SharpMap.Presentation.AspNet.Demo
{
    /// <summary>
    /// Silly class used to randomize symbology. 
    /// </summary>
    public static class RandomStyle
    {
        //[ThreadStatic]
        private static readonly List<Symbol2D> _symbols;

        private static readonly Random random;

        static RandomStyle()
        {
            random = new Random(DateTime.Now.Millisecond);
            _symbols = CreateSymbols();
        }

        public static List<Symbol2D> Symbols
        {
            get
            {
                // _symbols = _symbols ?? CreateSymbols();
                return _symbols;
            }
        }

        private static List<Symbol2D> CreateSymbols()
        {
            List<Symbol2D> symbols = new List<Symbol2D>();
            for (int i = 0; i < 20; i++)
                symbols.Add(CreateRandomSymbol());
            return symbols;
        }

        /// <summary>
        /// returns  a Random VectorStyle with no symbols.
        /// 
        /// </summary>
        /// <returns></returns>
        public static GeoJsonGeometryStyle RandomGeometryStyle()
        {
            GeoJsonGeometryStyle vs = new GeoJsonGeometryStyle();

            vs.EnableOutline = random.Next(0, 2) == 1;

            vs.Fill = RandomBrush();
            vs.Line = RandomPen();
            vs.Outline = RandomPen();
            vs.Symbol = RandomSymbol();
            vs.Outline.Width += vs.Line.Width;

            vs.HighlightFill = RandomBrush();
            vs.HighlightLine = RandomPen();
            vs.HighlightOutline = RandomPen();
            vs.HighlightSymbol = RandomSymbol();


            vs.SelectFill = RandomBrush();
            vs.SelectLine = RandomPen();
            vs.SelectOutline = RandomPen();
            vs.SelectSymbol = RandomSymbol();


            return vs;
        }

        private static Symbol2D RandomSymbol()
        {
            return (Symbol2D) Symbols[random.Next(19)].Clone();
        }


        private static Symbol2D CreateRandomSymbol()
        {
            Size2D sz;
            return new Symbol2D(RandomIcon(out sz), sz);
        }

        private static Stream RandomIcon(out Size2D sz)
        {
            sz = RandomSymbolSize();
            Bitmap b = new Bitmap((int) sz.Width, (int) sz.Height);

            Graphics g = Graphics.FromImage(b);
            g.Clear(Color.Transparent);

            int rnd = random.Next(6);

            switch (rnd)
            {
                case 0:
                    {
                        StylePen p = RandomPen();
                        g.DrawEllipse(ViewConverter.Convert(p), (int) Math.Ceiling(p.Width/2),
                                      (int) Math.Ceiling(p.Width/2), b.Width - (int) p.Width,
                                      b.Height - (int) p.Width);
                        break;
                    }
                case 1:
                    {
                        g.FillEllipse(ViewConverter.Convert(RandomBrush()), 0, 0, b.Width, b.Height);
                        break;
                    }
                case 2:
                    {
                        StylePen p = RandomPen();
                        g.DrawRectangle(ViewConverter.Convert(p), (int) Math.Ceiling(p.Width/2),
                                        (int) Math.Ceiling(p.Width/2), b.Width - (int) p.Width,
                                        b.Height - (int) p.Width);
                        break;
                    }
                case 3:
                    g.FillRectangle(ViewConverter.Convert(RandomBrush()), 0, 0, b.Width, b.Height);
                    break;
                case 4:
                    {
                        StylePen p = RandomPen();
                        g.FillEllipse(ViewConverter.Convert(RandomBrush()), 0, 0, b.Width, b.Height);
                        g.DrawEllipse(ViewConverter.Convert(p), (int) Math.Ceiling(p.Width/2),
                                      (int) Math.Ceiling(p.Width/2), b.Width - (int) p.Width,
                                      b.Height - (int) p.Width);
                        break;
                    }
                case 5:
                    {
                        StylePen p = RandomPen();
                        g.FillRectangle(ViewConverter.Convert(RandomBrush()), 0, 0, b.Width, b.Height);
                        g.DrawRectangle(ViewConverter.Convert(p), (int) Math.Ceiling(p.Width/2),
                                        (int) Math.Ceiling(p.Width/2), b.Width - (int) p.Width,
                                        b.Height - (int) p.Width);
                        break;
                    }
            }

            MemoryStream ms = new MemoryStream();
            b.Save(ms, ImageFormat.Png);
            return ms;
        }

        private static Size2D RandomSymbolSize()
        {
            int d = random.Next(15, 25);
            return new Size2D(d, d);
        }


        public static StylePen RandomPen()
        {
            StylePen p = new StylePen(RandomColor(), random.Next(1, 5))
                             {
                                 Alignment = StylePenAlignment.Center,
                                 LineJoin = StyleLineJoin.MiterClipped,
                                 MiterLimit = 2,
                                 StartCap = StyleLineCap.NoAnchor,
                                 EndCap = StyleLineCap.NoAnchor
                             };
            return p;
        }

        public static StyleBrush RandomBrush()
        {
            return new SolidStyleBrush(RandomColor());
        }

        public static StyleColor RandomColor()
        {
            return new StyleColor(random.Next(0, 255), random.Next(0, 255), random.Next(0, 255), random.Next(60, 255));
        }
    }
}