using AGG.Gamma;

namespace AGG.Color
{
    public struct RGBA_Bytes
        : IColorType
    {
        //public const int CoverShift = 8;
        //public const int CoverSize = 1 << CoverShift;  //----cover_size 
        //public const int CoverMask = CoverSize - 1;    //----cover_mask 
        //public const int cover_none  = 0,                 //----cover_none 
        //public const int cover_full  = cover_mask         //----cover_full 

        //public const int  BaseShift = 8;
        //public const uint BaseScale = (uint)(1 << BaseShift);
        //public const uint BaseMask = BaseScale - 1;

        private readonly byte m_R;
        private readonly byte m_G;
        private readonly byte m_B;
        private readonly byte m_A;

        public byte R
        {
            get { return m_R; }
            //set { m_R = value; }
        }

        public byte G
        {
            get { return m_G; }
            //set { m_G = value; }
        }

        public byte B
        {
            get { return m_B; }
            //set { m_B = value; }
        }

        public byte A
        {
            get { return m_A; }
            //set { m_A = value; }
        }

        public uint R_Byte
        {
            get { return (uint)R; }
            //set { R = (byte)value; }
        }
        public uint G_Byte
        {
            get { return (uint)G; }
            //set { G = (byte)value; }
        }
        public uint B_Byte
        {
            get { return (uint)B; }
            //set { B = (byte)value; }
        }
        public uint A_Byte
        {
            get { return (uint)A; }
            //set { A = (byte)value; }
        }

        //--------------------------------------------------------------------
        public RGBA_Bytes(uint r_, uint g_, uint b_)
            : this(r_, g_, b_, ColorConstants.BaseMask)
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
            : this(r_, g_, b_, (int)ColorConstants.BaseMask)
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
            m_R = ((byte)Basics.RoundUint(r_ * (double)ColorConstants.BaseMask));
            m_G = ((byte)Basics.RoundUint(g_ * (double)ColorConstants.BaseMask));
            m_B = ((byte)Basics.RoundUint(b_ * (double)ColorConstants.BaseMask));
            m_A = ((byte)Basics.RoundUint(a_ * (double)ColorConstants.BaseMask));
        }

        //--------------------------------------------------------------------
        public RGBA_Bytes(double r_, double g_, double b_)
        {
            m_R = ((byte)Basics.RoundUint(r_ * (double)ColorConstants.BaseMask));
            m_G = ((byte)Basics.RoundUint(g_ * (double)ColorConstants.BaseMask));
            m_B = ((byte)Basics.RoundUint(b_ * (double)ColorConstants.BaseMask));
            m_A = (byte)ColorConstants.BaseMask;
        }

        //--------------------------------------------------------------------
        RGBA_Bytes(RGBA_Doubles c, double a_)
        {
            m_R = ((byte)Basics.RoundUint(c.R * (double)ColorConstants.BaseMask));
            m_G = ((byte)Basics.RoundUint(c.G * (double)ColorConstants.BaseMask));
            m_B = ((byte)Basics.RoundUint(c.B * (double)ColorConstants.BaseMask));
            m_A = ((byte)Basics.RoundUint(a_ * (double)ColorConstants.BaseMask));
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
            m_R = ((byte)Basics.RoundUint(c.R * (double)ColorConstants.BaseMask));
            m_G = ((byte)Basics.RoundUint(c.G * (double)ColorConstants.BaseMask));
            m_B = ((byte)Basics.RoundUint(c.B * (double)ColorConstants.BaseMask));
            m_A = ((byte)Basics.RoundUint(c.A * (double)ColorConstants.BaseMask));
        }

        public RGBA_Doubles GetAsRGBA_Doubles()
        {
            return new RGBA_Doubles((double)R / (double)ColorConstants.BaseMask, (double)G / (double)ColorConstants.BaseMask, (double)B / (double)ColorConstants.BaseMask, (double)A / (double)ColorConstants.BaseMask);
        }

        public RGBA_Bytes GetAsRGBA_Bytes()
        {
            return this;
        }

        //--------------------------------------------------------------------
        //void Clear()
        //{
        //    R = G = B = A = 0;
        //}

        private static RGBA_Bytes _empty = new RGBA_Bytes(0, 0, 0, 0);
        private static RGBA_Bytes _black = new RGBA_Bytes(0, 0, 0, 255u);

        public static RGBA_Bytes Empty
        {
            get
            {
                return _empty;
            }
        }

        public static RGBA_Bytes Black
        {
            get
            {
                return _black;
            }
        }



        //--------------------------------------------------------------------
        public RGBA_Bytes Gradient(RGBA_Bytes c, double k)
        {
            //RGBA_Bytes ret = new RGBA_Bytes();
            uint ik = Basics.RoundUint(k * ColorConstants.BaseScale);

            //ret.R_Byte = (byte)((uint)(R_Byte) + ((((uint)(c.R_Byte) - R_Byte) * ik) >> ColorConstants.BaseShift));
            //ret.G_Byte = (byte)((uint)(G_Byte) + ((((uint)(c.G_Byte) - G_Byte) * ik) >> ColorConstants.BaseShift));
            //ret.B_Byte = (byte)((uint)(B_Byte) + ((((uint)(c.B_Byte) - B_Byte) * ik) >> ColorConstants.BaseShift));
            //ret.A_Byte = (byte)((uint)(A_Byte) + ((((uint)(c.A_Byte) - A_Byte) * ik) >> ColorConstants.BaseShift));
            //return ret;

            return new RGBA_Bytes(
                 (byte)((uint)(R_Byte) + ((((uint)(c.R_Byte) - R_Byte) * ik) >> ColorConstants.BaseShift)),
                 (byte)((uint)(G_Byte) + ((((uint)(c.G_Byte) - G_Byte) * ik) >> ColorConstants.BaseShift)),
                 (byte)((uint)(B_Byte) + ((((uint)(c.B_Byte) - B_Byte) * ik) >> ColorConstants.BaseShift)),
                 (byte)((uint)(A_Byte) + ((((uint)(c.A_Byte) - A_Byte) * ik) >> ColorConstants.BaseShift)));
        }

        //--------------------------------------------------------------------
        public RGBA_Bytes Add(RGBA_Bytes c, uint cover)
        {
            uint cr, cg, cb, ca;
            if (cover == ColorConstants.CoverMask)
            {
                if (c.A_Byte == ColorConstants.BaseMask)
                {
                    return c;
                    //this = c;
                }
                else
                {
                    cr = R_Byte + c.R_Byte;
                    //R_Byte = (cr > (uint)(ColorConstants.BaseMask)) ? (uint)(ColorConstants.BaseMask) : cr;
                    cg = G_Byte + c.G_Byte;
                    //G_Byte = (cg > (uint)(ColorConstants.BaseMask)) ? (uint)(ColorConstants.BaseMask) : cg;
                    cb = B_Byte + c.B_Byte;
                    //B_Byte = (cb > (uint)(ColorConstants.BaseMask)) ? (uint)(ColorConstants.BaseMask) : cb;
                    ca = A_Byte + c.A_Byte;
                    //A_Byte = (ca > (uint)(ColorConstants.BaseMask)) ? (uint)(ColorConstants.BaseMask) : ca;

                    return new RGBA_Bytes(
                         (cr > (uint)(ColorConstants.BaseMask)) ? (uint)(ColorConstants.BaseMask) : cr,
                         (cg > (uint)(ColorConstants.BaseMask)) ? (uint)(ColorConstants.BaseMask) : cg,
                         (cb > (uint)(ColorConstants.BaseMask)) ? (uint)(ColorConstants.BaseMask) : cb,
                         (ca > (uint)(ColorConstants.BaseMask)) ? (uint)(ColorConstants.BaseMask) : ca);
                }
            }
            else
            {
                cr = R_Byte + ((c.R_Byte * cover + ColorConstants.CoverMask / 2) >> ColorConstants.CoverShift);
                cg = G_Byte + ((c.G_Byte * cover + ColorConstants.CoverMask / 2) >> ColorConstants.CoverShift);
                cb = B_Byte + ((c.B_Byte * cover + ColorConstants.CoverMask / 2) >> ColorConstants.CoverShift);
                ca = A_Byte + ((c.A_Byte * cover + ColorConstants.CoverMask / 2) >> ColorConstants.CoverShift);
                //R_Byte = (cr > (uint)(ColorConstants.BaseMask)) ? (uint)(ColorConstants.BaseMask) : cr;
                //G_Byte = (cg > (uint)(ColorConstants.BaseMask)) ? (uint)(ColorConstants.BaseMask) : cg;
                //B_Byte = (cb > (uint)(ColorConstants.BaseMask)) ? (uint)(ColorConstants.BaseMask) : cb;
                //A_Byte = (ca > (uint)(ColorConstants.BaseMask)) ? (uint)(ColorConstants.BaseMask) : ca;

                return new RGBA_Bytes(
                         (cr > (uint)(ColorConstants.BaseMask)) ? (uint)(ColorConstants.BaseMask) : cr,
                         (cg > (uint)(ColorConstants.BaseMask)) ? (uint)(ColorConstants.BaseMask) : cg,
                         (cb > (uint)(ColorConstants.BaseMask)) ? (uint)(ColorConstants.BaseMask) : cb,
                         (ca > (uint)(ColorConstants.BaseMask)) ? (uint)(ColorConstants.BaseMask) : ca
                    );
            }
        }

        //--------------------------------------------------------------------
        public RGBA_Bytes ApplyGammaDir(GammaLut gamma)
        {
            //R_Byte = gamma.Dir((byte)R_Byte);
            //G_Byte = gamma.Dir((byte)G_Byte);
            //B_Byte = gamma.Dir((byte)B_Byte);

            return new RGBA_Bytes(
                 gamma.Dir((byte)R_Byte),
                 gamma.Dir((byte)G_Byte),
                 gamma.Dir((byte)B_Byte),
                 A_Byte);

        }

        //public static IColorType no_color() { return new RGBA_Bytes(0, 0, 0, 0); }

        //-------------------------------------------------------------rgb8_packed
        static public RGBA_Bytes Rgb8Packed(uint v)
        {
            return new RGBA_Bytes((v >> 16) & 0xFF, (v >> 8) & 0xFF, v & 0xFF);
        }

        public static RGBA_Bytes ModifyComponent(RGBA_Bytes color, Component comp, uint value)
        {
            return new RGBA_Bytes(
                 comp == Component.R ? value : color.R_Byte,
                 comp == Component.G ? value : color.G_Byte,
                 comp == Component.B ? value : color.B_Byte,
                 comp == Component.A ? value : color.A_Byte);

        }
    };
}