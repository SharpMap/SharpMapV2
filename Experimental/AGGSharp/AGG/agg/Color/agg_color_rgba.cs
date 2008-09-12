
/*
 *	Portions of this file are  © 2008 Newgrove Consultants Limited, 
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
 *  Original notices below.
 * 
 */
//----------------------------------------------------------------------------
// Anti-Grain Geometry - Version 2.4
// Copyright (C) 2002-2005 Maxim Shemanarev (http://www.antigrain.com)
//
// C# Port port by: Lars Brubaker
//                  larsbrubaker@gmail.com
// Copyright (C) 2007
//
// Permission to copy, use, modify, sell and distribute this software 
// is granted provided this copyright notice appears in all copies. 
// This software is provided "as is" without express or implied
// warranty, and with no claim as to its suitability for any purpose.
//
//----------------------------------------------------------------------------
//
// Adaptation for high precision colors has been sponsored by 
// Liberty Technology Systems, Inc., visit http://lib-sys.com
//
// Liberty Technology Systems, Inc. is the provider of
// PostScript and PDF technology for software developers.
// 
//----------------------------------------------------------------------------
// Contact: mcseem@antigrain.com
//          mcseemagg@yahoo.com
//          http://www.antigrain.com
//----------------------------------------------------------------------------
using System;

namespace AGG
{
    // Supported byte orders for RGB and RGBA pixel formats
    //=======================================================================
    struct OrderRgb { enum RGB { R = 0, G = 1, B = 2, RGBTag }; };       //----order_rgb
    struct OrderBgr { enum BGR { B = 0, G = 1, R = 2, RGBTag }; };       //----order_bgr
    struct OrderRgba { enum RGBA { R = 0, G = 1, B = 2, A = 3, RGBATag }; }; //----order_rgba
    struct OrderArgb { enum ARGB { A = 0, R = 1, G = 2, B = 3, RGBATag }; }; //----order_argb
    struct OrderAbgr { enum ABGR { A = 0, B = 1, G = 2, R = 3, RGBATag }; }; //----order_abgr
    struct OrderBgra { enum BGRA { B = 0, G = 1, R = 2, A = 3, RGBATag }; }; //----order_bgra

    //====================================================================rgba
    public struct RGBA_Doubles : IColorType
    {
        const int BaseShift = 8;
        const uint BaseScale = (uint)(1 << BaseShift);
        const uint BaseMask = BaseScale - 1;

        public double m_r;
        public double m_g;
        public double m_b;
        public double m_a;

        public uint R_Byte { get { return (uint)Basics.RoundUint(m_r * (double)BaseMask); } set { m_r = (double)value / (double)BaseMask; } }
        public uint G_Byte { get { return (uint)Basics.RoundUint(m_g * (double)BaseMask); } set { m_g = (double)value / (double)BaseMask; } }
        public uint B_Byte { get { return (uint)Basics.RoundUint(m_b * (double)BaseMask); } set { m_b = (double)value / (double)BaseMask; } }
        public uint A_Byte { get { return (uint)Basics.RoundUint(m_a * (double)BaseMask); } set { m_a = (double)value / (double)BaseMask; } }

        //--------------------------------------------------------------------
        public RGBA_Doubles(double r_, double g_, double b_)
            : this(r_, g_, b_, 1.0)
        {
        }

        //--------------------------------------------------------------------
        public RGBA_Doubles(double r_, double g_, double b_, double a_)
        {
            m_r = r_;
            m_g = g_;
            m_b = b_;
            m_a = a_;
        }

        //--------------------------------------------------------------------
        public RGBA_Doubles(RGBA_Doubles c)
            : this(c, 1)
        {
        }

        public RGBA_Doubles(RGBA_Doubles c, double a_)
        {
            m_r = c.m_r;
            m_g = c.m_g;
            m_b = c.m_b;
            m_a = a_;
        }

        //--------------------------------------------------------------------
        public RGBA_Doubles(double wavelen)
            : this(wavelen, 1.0)
        {

        }

        public RGBA_Doubles(double wavelen, double gamma)
        {
            this = FromWavelength(wavelen, gamma);
        }

        public RGBA_Bytes GetAsRGBA_Bytes()
        {
            return new RGBA_Bytes(R_Byte, G_Byte, B_Byte, A_Byte);
        }

        public RGBA_Doubles GetAsRGBA_Doubles()
        {
            return this;
        }

        //--------------------------------------------------------------------
        public void Clear()
        {
            m_r = m_g = m_b = m_a = 0;
        }

        //--------------------------------------------------------------------
        public RGBA_Doubles Transparent()
        {
            m_a = 0.0;
            return this;
        }

        //--------------------------------------------------------------------
        public RGBA_Doubles Opacity(double a_)
        {
            if (a_ < 0.0) a_ = 0.0;
            if (a_ > 1.0) a_ = 1.0;
            m_a = a_;
            return this;
        }

        //--------------------------------------------------------------------
        public double Opacity()
        {
            return m_a;
        }

        //--------------------------------------------------------------------
        public RGBA_Doubles PreMultiply()
        {
            m_r *= m_a;
            m_g *= m_a;
            m_b *= m_a;
            return this;
        }

        //--------------------------------------------------------------------
        public RGBA_Doubles PreMultiply(double a_)
        {
            if (m_a <= 0.0 || a_ <= 0.0)
            {
                m_r = m_g = m_b = m_a = 0.0;
                return this;
            }
            a_ /= m_a;
            m_r *= a_;
            m_g *= a_;
            m_b *= a_;
            m_a = a_;
            return this;
        }

        //--------------------------------------------------------------------
        public RGBA_Doubles DeMultiply()
        {
            if (m_a == 0)
            {
                m_r = m_g = m_b = 0;
                return this;
            }
            double a_ = 1.0 / m_a;
            m_r *= a_;
            m_g *= a_;
            m_b *= a_;
            return this;
        }


        //--------------------------------------------------------------------
        public RGBA_Bytes Gradient(RGBA_Bytes c_8, double k)
        {
            RGBA_Doubles c = c_8.GetAsRGBA_Doubles();
            RGBA_Doubles ret;
            ret.m_r = m_r + (c.m_r - m_r) * k;
            ret.m_g = m_g + (c.m_g - m_g) * k;
            ret.m_b = m_b + (c.m_b - m_b) * k;
            ret.m_a = m_a + (c.m_a - m_a) * k;
            return ret.GetAsRGBA_Bytes();
        }

        //--------------------------------------------------------------------
        public static IColorType NoColor() { return (IColorType)new RGBA_Doubles(0, 0, 0, 0); }

        //--------------------------------------------------------------------
        public static RGBA_Doubles FromWavelength(double wl)
        {
            return FromWavelength(wl, 1.0);
        }

        public static RGBA_Doubles FromWavelength(double wl, double gamma)
        {
            RGBA_Doubles t = new RGBA_Doubles(0.0, 0.0, 0.0);

            if (wl >= 380.0 && wl <= 440.0)
            {
                t.m_r = -1.0 * (wl - 440.0) / (440.0 - 380.0);
                t.m_b = 1.0;
            }
            else
                if (wl >= 440.0 && wl <= 490.0)
                {
                    t.m_g = (wl - 440.0) / (490.0 - 440.0);
                    t.m_b = 1.0;
                }
                else
                    if (wl >= 490.0 && wl <= 510.0)
                    {
                        t.m_g = 1.0;
                        t.m_b = -1.0 * (wl - 510.0) / (510.0 - 490.0);
                    }
                    else
                        if (wl >= 510.0 && wl <= 580.0)
                        {
                            t.m_r = (wl - 510.0) / (580.0 - 510.0);
                            t.m_g = 1.0;
                        }
                        else
                            if (wl >= 580.0 && wl <= 645.0)
                            {
                                t.m_r = 1.0;
                                t.m_g = -1.0 * (wl - 645.0) / (645.0 - 580.0);
                            }
                            else
                                if (wl >= 645.0 && wl <= 780.0)
                                {
                                    t.m_r = 1.0;
                                }

            double s = 1.0;
            if (wl > 700.0) s = 0.3 + 0.7 * (780.0 - wl) / (780.0 - 700.0);
            else if (wl < 420.0) s = 0.3 + 0.7 * (wl - 380.0) / (420.0 - 380.0);

            t.m_r = Math.Pow(t.m_r * s, gamma);
            t.m_g = Math.Pow(t.m_g * s, gamma);
            t.m_b = Math.Pow(t.m_b * s, gamma);
            return t;
        }

        public static RGBA_Doubles RgbaPre(double r, double g, double b)
        {
            return RgbaPre(r, g, b, 1.0);
        }

        public static RGBA_Doubles RgbaPre(double r, double g, double b, double a)
        {
            return new RGBA_Doubles(r, g, b, a).PreMultiply();
        }

        public static RGBA_Doubles RgbaPre(RGBA_Doubles c)
        {
            return new RGBA_Doubles(c).PreMultiply();
        }

        public static RGBA_Doubles RgbaPre(RGBA_Doubles c, double a)
        {
            return new RGBA_Doubles(c, a).PreMultiply();
        }

        public static RGBA_Doubles GetTweenColor(RGBA_Doubles Color1, RGBA_Doubles Color2, double RatioOf2)
        {
            if (RatioOf2 <= 0)
            {
                return new RGBA_Doubles(Color1);
            }

            if (RatioOf2 >= 1.0)
            {
                return new RGBA_Doubles(Color2);
            }

            // figure out how much of each color we should be.
            double RatioOf1 = 1.0 - RatioOf2;
            return new RGBA_Doubles(
                Color1.m_r * RatioOf1 + Color2.m_r * RatioOf2,
                Color1.m_g * RatioOf1 + Color2.m_g * RatioOf2,
                Color1.m_b * RatioOf1 + Color2.m_b * RatioOf2);
        }
    };

    //===================================================================rgba8
    public struct RGBA_Bytes : IColorType
    {
        public const int CoverShift = 8;
        public const int CoverSize = 1 << CoverShift;  //----cover_size 
        public const int CoverMask = CoverSize - 1;    //----cover_mask 
        //public const int cover_none  = 0,                 //----cover_none 
        //public const int cover_full  = cover_mask         //----cover_full 

        public const int BaseShift = 8;
        public const uint BaseScale = (uint)(1 << BaseShift);
        public const uint BaseMask = BaseScale - 1;

        public byte m_R;
        public byte m_G;
        public byte m_B;
        public byte m_A;

        public uint R_Byte { get { return (uint)m_R; } set { m_R = (byte)value; } }
        public uint G_Byte { get { return (uint)m_G; } set { m_G = (byte)value; } }
        public uint B_Byte { get { return (uint)m_B; } set { m_B = (byte)value; } }
        public uint A_Byte { get { return (uint)m_A; } set { m_A = (byte)value; } }

        //--------------------------------------------------------------------
        public RGBA_Bytes(uint r_, uint g_, uint b_)
            : this(r_, g_, b_, BaseMask)
        { }

        //--------------------------------------------------------------------
        public RGBA_Bytes(uint r_, uint g_, uint b_, uint a_)
        {
            m_R = (byte)r_;
            m_G = (byte)g_;
            m_B = (byte)b_;
            m_A = (byte)a_;
        }

        //--------------------------------------------------------------------
        public RGBA_Bytes(int r_, int g_, int b_)
            : this(r_, g_, b_, (int)BaseMask)
        { }

        //--------------------------------------------------------------------
        public RGBA_Bytes(int r_, int g_, int b_, int a_)
        {
            m_R = (byte)r_;
            m_G = (byte)g_;
            m_B = (byte)b_;
            m_A = (byte)a_;
        }

        //--------------------------------------------------------------------
        public RGBA_Bytes(double r_, double g_, double b_, double a_)
        {
            m_R = ((byte)Basics.RoundUint(r_ * (double)BaseMask));
            m_G = ((byte)Basics.RoundUint(g_ * (double)BaseMask));
            m_B = ((byte)Basics.RoundUint(b_ * (double)BaseMask));
            m_A = ((byte)Basics.RoundUint(a_ * (double)BaseMask));
        }

        //--------------------------------------------------------------------
        public RGBA_Bytes(double r_, double g_, double b_)
        {
            m_R = ((byte)Basics.RoundUint(r_ * (double)BaseMask));
            m_G = ((byte)Basics.RoundUint(g_ * (double)BaseMask));
            m_B = ((byte)Basics.RoundUint(b_ * (double)BaseMask));
            m_A = (byte)BaseMask;
        }

        //--------------------------------------------------------------------
        RGBA_Bytes(RGBA_Doubles c, double a_)
        {
            m_R = ((byte)Basics.RoundUint(c.m_r * (double)BaseMask));
            m_G = ((byte)Basics.RoundUint(c.m_g * (double)BaseMask));
            m_B = ((byte)Basics.RoundUint(c.m_b * (double)BaseMask));
            m_A = ((byte)Basics.RoundUint(a_ * (double)BaseMask));
        }

        //--------------------------------------------------------------------
        RGBA_Bytes(RGBA_Bytes c, uint a_)
        {
            m_R = (byte)c.m_R;
            m_G = (byte)c.m_G;
            m_B = (byte)c.m_B;
            m_A = (byte)a_;
        }

        public RGBA_Bytes(RGBA_Doubles c)
        {
            m_R = ((byte)Basics.RoundUint(c.m_r * (double)BaseMask));
            m_G = ((byte)Basics.RoundUint(c.m_g * (double)BaseMask));
            m_B = ((byte)Basics.RoundUint(c.m_b * (double)BaseMask));
            m_A = ((byte)Basics.RoundUint(c.m_a * (double)BaseMask));
        }

        public RGBA_Doubles GetAsRGBA_Doubles()
        {
            return new RGBA_Doubles((double)m_R / (double)BaseMask, (double)m_G / (double)BaseMask, (double)m_B / (double)BaseMask, (double)m_A / (double)BaseMask);
        }

        public RGBA_Bytes GetAsRGBA_Bytes()
        {
            return this;
        }

        //--------------------------------------------------------------------
        void Clear()
        {
            m_R = m_G = m_B = m_A = 0;
        }

        //--------------------------------------------------------------------
        public RGBA_Bytes Gradient(RGBA_Bytes c, double k)
        {
            RGBA_Bytes ret = new RGBA_Bytes();
            uint ik = Basics.RoundUint(k * BaseScale);
            ret.R_Byte = (byte)((uint)(R_Byte) + ((((uint)(c.R_Byte) - R_Byte) * ik) >> BaseShift));
            ret.G_Byte = (byte)((uint)(G_Byte) + ((((uint)(c.G_Byte) - G_Byte) * ik) >> BaseShift));
            ret.B_Byte = (byte)((uint)(B_Byte) + ((((uint)(c.B_Byte) - B_Byte) * ik) >> BaseShift));
            ret.A_Byte = (byte)((uint)(A_Byte) + ((((uint)(c.A_Byte) - A_Byte) * ik) >> BaseShift));
            return ret;
        }

        //--------------------------------------------------------------------
        public void Add(RGBA_Bytes c, uint cover)
        {
            uint cr, cg, cb, ca;
            if (cover == CoverMask)
            {
                if (c.A_Byte == BaseMask)
                {
                    this = c;
                }
                else
                {
                    cr = R_Byte + c.R_Byte; R_Byte = (cr > (uint)(BaseMask)) ? (uint)(BaseMask) : cr;
                    cg = G_Byte + c.G_Byte; G_Byte = (cg > (uint)(BaseMask)) ? (uint)(BaseMask) : cg;
                    cb = B_Byte + c.B_Byte; B_Byte = (cb > (uint)(BaseMask)) ? (uint)(BaseMask) : cb;
                    ca = A_Byte + c.A_Byte; A_Byte = (ca > (uint)(BaseMask)) ? (uint)(BaseMask) : ca;
                }
            }
            else
            {
                cr = R_Byte + ((c.R_Byte * cover + CoverMask / 2) >> CoverShift);
                cg = G_Byte + ((c.G_Byte * cover + CoverMask / 2) >> CoverShift);
                cb = B_Byte + ((c.B_Byte * cover + CoverMask / 2) >> CoverShift);
                ca = A_Byte + ((c.A_Byte * cover + CoverMask / 2) >> CoverShift);
                R_Byte = (cr > (uint)(BaseMask)) ? (uint)(BaseMask) : cr;
                G_Byte = (cg > (uint)(BaseMask)) ? (uint)(BaseMask) : cg;
                B_Byte = (cb > (uint)(BaseMask)) ? (uint)(BaseMask) : cb;
                A_Byte = (ca > (uint)(BaseMask)) ? (uint)(BaseMask) : ca;
            }
        }

        //--------------------------------------------------------------------
        public void ApplyGammaDir(GammaLut gamma)
        {
            R_Byte = gamma.Dir((byte)R_Byte);
            G_Byte = gamma.Dir((byte)G_Byte);
            B_Byte = gamma.Dir((byte)B_Byte);
        }

        public static IColorType no_color() { return new RGBA_Bytes(0, 0, 0, 0); }

        //-------------------------------------------------------------rgb8_packed
        static public RGBA_Bytes Rgb8Packed(uint v)
        {
            return new RGBA_Bytes((v >> 16) & 0xFF, (v >> 8) & 0xFF, v & 0xFF);
        }
    };
}