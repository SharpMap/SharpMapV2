
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
// Contact: mcseem@antigrain.com
//          mcseemagg@yahoo.com
//          http://www.antigrain.com
//----------------------------------------------------------------------------
//
// Adaptation for high precision colors has been sponsored by 
// Liberty Technology Systems, Inc., visit http://lib-sys.com
//
// Liberty Technology Systems, Inc. is the provider of
// PostScript and PDF technology for software developers.
// 
//----------------------------------------------------------------------------
//
// color types gray8, gray16
//
//----------------------------------------------------------------------------
namespace AGG
{

    //===================================================================gray8
    public struct Gray8
    {
        const int BaseShift = 8;
        const uint BaseScale = (uint)(1 << BaseShift);
        const uint BaseMask = BaseScale - 1;

        byte v;
        byte a;

        //--------------------------------------------------------------------
        public Gray8(uint v_)
            : this(v_, (uint)BaseMask)
        {

        }

        public Gray8(uint v_, uint a_)
        {
            v = (byte)(v_);
            a = (byte)(a_);
        }

        //--------------------------------------------------------------------
        Gray8(Gray8 c, uint a_)
        {
            v = (c.v);
            a = (byte)(a_);
        }

        //--------------------------------------------------------------------
        public Gray8(RGBA_Doubles c)
        {
            v = ((byte)Basics.RoundUint((0.299 * c.R_Byte + 0.587 * c.G_Byte + 0.114 * c.B_Byte) * (double)(BaseMask)));
            a = ((byte)Basics.RoundUint(c.A_Byte * (double)(BaseMask)));
        }


        //--------------------------------------------------------------------
        public Gray8(RGBA_Doubles c, double a_)
        {
            v = ((byte)Basics.RoundUint((0.299 * c.R_Byte + 0.587 * c.G_Byte + 0.114 * c.B_Byte) * (double)(BaseMask)));
            a = ((byte)Basics.RoundUint(a_ * (double)(BaseMask)));
        }

        //--------------------------------------------------------------------
        public Gray8(RGBA_Bytes c)
        {
            v = (byte)((c.R_Byte * 77 + c.G_Byte * 150 + c.B_Byte * 29) >> 8);
            a = (byte)(c.A_Byte);
        }

        //--------------------------------------------------------------------
        public Gray8(RGBA_Bytes c, uint a_)
        {
            v = (byte)((c.R_Byte * 77 + c.G_Byte * 150 + c.B_Byte * 29) >> 8);
            a = (byte)(a_);
        }

        //--------------------------------------------------------------------
        public void Clear()
        {
            v = a = 0;
        }

        //--------------------------------------------------------------------
        public Gray8 Transparent()
        {
            a = 0;
            return this;
        }

        //--------------------------------------------------------------------
        public void Opacity(double a_)
        {
            if (a_ < 0.0) a_ = 0.0;
            if (a_ > 1.0) a_ = 1.0;
            a = (byte)Basics.RoundUint(a_ * (double)(BaseMask));
        }

        //--------------------------------------------------------------------
        public double Opacity()
        {
            return (double)(a) / (double)(BaseMask);
        }


        //--------------------------------------------------------------------
        public Gray8 PreMultiply()
        {
            if (a == (byte)BaseMask) return this;
            if (a == 0)
            {
                v = 0;
                return this;
            }
            v = (byte)(((uint)(v) * a) >> BaseShift);
            return this;
        }

        //--------------------------------------------------------------------
        public Gray8 PreMultiply(uint a_)
        {
            if (a == (int)BaseMask && a_ >= (int)BaseMask) return this;
            if (a == 0 || a_ == 0)
            {
                v = a = 0;
                return this;
            }
            uint v_ = ((uint)(v) * a_) / a;
            v = (byte)((v_ > a_) ? a_ : v_);
            a = (byte)(a_);
            return this;
        }

        //--------------------------------------------------------------------
        public Gray8 DeMultiply()
        {
            if (a == (int)BaseMask) return this;
            if (a == 0)
            {
                v = 0;
                return this;
            }
            uint v_ = ((uint)(v) * (int)BaseMask) / a;
            v = (byte)((v_ > (int)BaseMask) ? (byte)BaseMask : v_);
            return this;
        }

        //--------------------------------------------------------------------
        public Gray8 Gradient(Gray8 c, double k)
        {
            Gray8 ret;
            uint ik = Basics.RoundUint(k * (int)BaseScale);
            ret.v = (byte)((uint)(v) + ((((uint)(c.v) - v) * ik) >> BaseShift));
            ret.a = (byte)((uint)(a) + ((((uint)(c.a) - a) * ik) >> BaseShift));
            return ret;
        }

        /*
        //--------------------------------------------------------------------
        void add(gray8 c, uint cover)
        {
            uint cv, ca;
            if(cover == cover_mask)
            {
                if (c.a == base_mask) 
                {
                    *this = c;
                }
                else
                {
                    cv = v + c.v; v = (cv > (uint)(base_mask)) ? (uint)(base_mask) : cv;
                    ca = a + c.a; a = (ca > (uint)(base_mask)) ? (uint)(base_mask) : ca;
                }
            }
            else
            {
                cv = v + ((c.v * cover + cover_mask/2) >> cover_shift);
                ca = a + ((c.a * cover + cover_mask/2) >> cover_shift);
                v = (cv > (uint)(base_mask)) ? (uint)(base_mask) : cv;
                a = (ca > (uint)(base_mask)) ? (uint)(base_mask) : ca;
            }
        }
         */

        //--------------------------------------------------------------------
        //static gray8 no_color() { return gray8(0,0); }

        /*
        static gray8 gray8_pre(uint v, uint a = gray8.base_mask)
        {
            return gray8(v,a).premultiply();
        }

        static gray8 gray8_pre(gray8 c, uint a)
        {
            return gray8(c,a).premultiply();
        }

        static gray8 gray8_pre(rgba& c)
        {
            return gray8(c).premultiply();
        }

        static gray8 gray8_pre(rgba& c, double a)
        {
            return gray8(c,a).premultiply();
        }

        static gray8 gray8_pre(rgba8& c)
        {
            return gray8(c).premultiply();
        }

        static gray8 gray8_pre(rgba8& c, uint a)
        {
            return gray8(c,a).premultiply();
        }
         */
    };
}
