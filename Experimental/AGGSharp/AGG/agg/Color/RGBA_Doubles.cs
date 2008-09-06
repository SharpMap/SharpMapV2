using System;

namespace AGG.Color
{
    public struct RGBA_Doubles : IColorType
    {
        //const int BaseShift = 8;
        //const uint BaseScale = (uint)(1 << BaseShift);
        //const uint BaseMask = BaseScale - 1;

        private readonly double m_r;
        private readonly double m_g;
        private readonly double m_b;
        private readonly double m_a;

        public double R
        {
            get { return m_r; }
            //set { m_r = value; }
        }

        public double G
        {
            get { return m_g; }
            //set { m_g = value; }
        }

        public double B
        {
            get { return m_b; }
            //set { m_b = value; }
        }

        public double A
        {
            get { return m_a; }
            //set { m_a = value; }
        }

        public uint R_Byte
        {
            get
            {
                return (uint)Basics.RoundUint(R * (double)ColorConstants.BaseMask);
            }
            //set
            //{
            //    R = (double)value / (double)ColorConstants.BaseMask;
            //}
        }
        public uint G_Byte
        {
            get
            {
                return (uint)Basics.RoundUint(G * (double)ColorConstants.BaseMask);
            }
            //set
            //{
            //    G = (double)value / (double)ColorConstants.BaseMask;
            //}
        }
        public uint B_Byte
        {
            get
            {
                return (uint)Basics.RoundUint(B * (double)ColorConstants.BaseMask);
            }
            //set
            //{
            //    B = (double)value / (double)ColorConstants.BaseMask;
            //}
        }
        public uint A_Byte
        {
            get
            {
                return (uint)Basics.RoundUint(A * (double)ColorConstants.BaseMask);
            }
            //set
            //{
            //    A = (double)value / (double)ColorConstants.BaseMask;
            //}
        }

        //--------------------------------------------------------------------
        public RGBA_Doubles(double r, double g, double b)
            : this(r, g, b, 1.0)
        {
        }

        //--------------------------------------------------------------------
        public RGBA_Doubles(double r, double g, double b, double a)
        {
            m_r = r;
            m_g = g;
            m_b = b;
            m_a = a;
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
        //public void Clear()
        //{
        //    R = G = B = A = 0;
        //}

        private static RGBA_Doubles _empty = new RGBA_Doubles(0, 0, 0, 0);
        private static RGBA_Doubles _black = new RGBA_Doubles(0, 0, 0, 1);

        public static RGBA_Doubles Empty
        {
            get
            {
                return _empty;
            }
        }

        public static RGBA_Doubles Black
        {
            get
            {
                return _black;
            }
        }


        //--------------------------------------------------------------------
        public RGBA_Doubles Transparent
        {
            get
            {
                return _empty;
            }
        }

        //--------------------------------------------------------------------
        public RGBA_Doubles Opacity(double a_)
        {
            //if (a_ < 0.0) a_ = 0.0;
            //if (a_ > 1.0) a_ = 1.0;
            //A = a_;
            //return this;

            return new RGBA_Doubles(R, G, B, a_);
        }

        //--------------------------------------------------------------------
        public double Opacity()
        {
            return A;
        }

        //--------------------------------------------------------------------
        public RGBA_Doubles PreMultiply()
        {
            //R *= A;
            //G *= A;
            //B *= A;
            //return this;

            return new RGBA_Doubles(R * A, G * A, B * A, A);
        }

        //--------------------------------------------------------------------
        public RGBA_Doubles PreMultiply(double a_)
        {
            if (A <= 0.0 || a_ <= 0.0)
            {
                return RGBA_Doubles.Empty;
                //R = G = B = A = 0.0;
                //return this;
            }
            //a_ /= A;
            //R *= a_;
            //G *= a_;
            //B *= a_;
            //A = a_;
            //return this;

            a_ /= A;
            return new RGBA_Doubles(R * a_, G * a_, B * a_, a_);
        }

        //--------------------------------------------------------------------
        public RGBA_Doubles DeMultiply()
        {
            if (A == 0)
            {
                return RGBA_Doubles.Empty;
                //R = G = B = 0;
                //return this;
            }
            double a_ = 1.0 / A;
            //R *= a_;
            //G *= a_;
            //B *= a_;
            //return this;

            return new RGBA_Doubles(R * a_, G * a_, B * a_, a_);
        }


        //--------------------------------------------------------------------
        public RGBA_Bytes Gradient(RGBA_Bytes c_8, double k)
        {
            RGBA_Doubles c = c_8.GetAsRGBA_Doubles();
            return new RGBA_Doubles(
            R + (c.R - R) * k,
            G + (c.G - G) * k,
            B + (c.B - B) * k,
            A + (c.A - A) * k).GetAsRGBA_Bytes();
            //return ret.GetAsRGBA_Bytes();
        }

        //--------------------------------------------------------------------
        //public static IColorType NoColor() { return (IColorType)new RGBA_Doubles(0, 0, 0, 0); }

        //--------------------------------------------------------------------
        public static RGBA_Doubles FromWavelength(double wl)
        {
            return FromWavelength(wl, 1.0);
        }

        public static RGBA_Doubles FromWavelength(double wl, double gamma)
        {
            // RGBA_Doubles t = new RGBA_Doubles(0.0, 0.0, 0.0);
            double tr = 0.0, tg = 0.0, tb = 0.0;
            if (wl >= 380.0 && wl <= 440.0)
            {
                tr = -1.0 * (wl - 440.0) / (440.0 - 380.0);
                tb = 1.0;
            }
            else
                if (wl >= 440.0 && wl <= 490.0)
                {
                    tg = (wl - 440.0) / (490.0 - 440.0);
                    tb = 1.0;
                }
                else
                    if (wl >= 490.0 && wl <= 510.0)
                    {
                        tg = 1.0;
                        tb = -1.0 * (wl - 510.0) / (510.0 - 490.0);
                    }
                    else
                        if (wl >= 510.0 && wl <= 580.0)
                        {
                            tr = (wl - 510.0) / (580.0 - 510.0);
                            tg = 1.0;
                        }
                        else
                            if (wl >= 580.0 && wl <= 645.0)
                            {
                                tr = 1.0;
                                tg = -1.0 * (wl - 645.0) / (645.0 - 580.0);
                            }
                            else
                                if (wl >= 645.0 && wl <= 780.0)
                                {
                                    tr = 1.0;
                                }

            double s = 1.0;
            if (wl > 700.0) s = 0.3 + 0.7 * (780.0 - wl) / (780.0 - 700.0);
            else if (wl < 420.0) s = 0.3 + 0.7 * (wl - 380.0) / (420.0 - 380.0);

            tr = Math.Pow(tr * s, gamma);
            tg = Math.Pow(tg * s, gamma);
            tb = Math.Pow(tb * s, gamma);

            return new RGBA_Doubles(tr, tg, tb);
            //return t;
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
                Color1.R * RatioOf1 + Color2.R * RatioOf2,
                Color1.G * RatioOf1 + Color2.G * RatioOf2,
                Color1.B * RatioOf1 + Color2.B * RatioOf2);
        }
    };
}
