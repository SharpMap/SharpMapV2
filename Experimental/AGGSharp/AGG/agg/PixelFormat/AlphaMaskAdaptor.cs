
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

using AGG.Color;
namespace AGG.PixelFormat
{
    //==================================================pixfmt_amask_adaptor
    public sealed class AlphaMaskAdaptor : PixelFormatProxy
    {
        IAlphaMask m_mask;
        ArrayPOD<byte> m_span;

        enum SpanExtraTail { span_extra_tail = 256 };
        static byte CoverFull = 255;

        void ReAllocSpan(int len)
        {
            if(len > m_span.Size())
            {
                m_span.Resize(len + (int)SpanExtraTail.span_extra_tail);
            }
        }

        void InitSpan(int len)
        {
            InitSpan(len, CoverFull);
        }

        void InitSpan(int len, byte cover)
        {
            ReAllocSpan(len);
            unsafe
            {
                fixed (byte* pBuffer = m_span.Array)
                {
                    Basics.MemSet(pBuffer, cover, (int)len);
                }
            }
        }

        unsafe void InitSpan(int len, byte* covers)
        {
            ReAllocSpan(len);
            unsafe
            {
            	byte[] array = m_span.Array;
                for (int i = 0; i < (int)len; i++)
                {
                    array[i] = *covers++;
                }
            }
        }


        public AlphaMaskAdaptor(IPixelFormat pixf, IAlphaMask mask)
            : base(pixf)
        {
            m_pixf = pixf;
            m_mask = mask;
            m_span = new ArrayPOD<byte>(255);
        }

        public void AttachPixFmt(IPixelFormat pixf)
        {
            m_pixf = pixf; 
        }
        public void AttachAlphaMask(IAlphaMask mask) 
        {
            m_mask = mask; 
        }

        //--------------------------------------------------------------------
        public void CopyPixel(int x, int y, RGBA_Bytes c)
        {
            m_pixf.BlendPixel(x, y, c, m_mask.Pixel(x, y));
        }

        //--------------------------------------------------------------------
        public override void BlendPixel(int x, int y, RGBA_Bytes c, byte cover)
        {
            m_pixf.BlendPixel(x, y, c, m_mask.CombinePixel(x, y, cover));
        }

        //--------------------------------------------------------------------
        public override void CopyHLine(int x, int y, uint len, RGBA_Bytes c)
        {
            ReAllocSpan((int)len);
            unsafe
            {
                fixed (byte* pBuffer = m_span.Array)
                {
                    m_mask.FillHSpan(x, y, pBuffer, (int)len);
                    m_pixf.BlendSolidHSpan(x, y, len, c, pBuffer);
                }
            }
        }

        //--------------------------------------------------------------------
        public override void BlendHLine(int x1, int y, int x2, RGBA_Bytes c, byte cover)
        {
            int len = x2 - x1 + 1;
            if (cover == CoverFull)
            {
                ReAllocSpan(len);
                unsafe
                {
                    fixed (byte* pBuffer = m_span.Array)
                    {
                        m_mask.CombineHSpanFullCover(x1, y, pBuffer, (int)len);
                        m_pixf.BlendSolidHSpan(x1, y, (uint)len, c, pBuffer);
                    }
                }
            }
            else
            {
                InitSpan(len, cover);
                unsafe
                {
                    fixed (byte* pBuffer = m_span.Array)
                    {
                        m_mask.CombineHSpan(x1, y, pBuffer, (int)len);
                        m_pixf.BlendSolidHSpan(x1, y, (uint)len, c, pBuffer);
                    }
                }
            }
        }

        //--------------------------------------------------------------------
        public override void CopyVLine(int x, int y, uint len, RGBA_Bytes c)
        {
            ReAllocSpan((int)len);
            unsafe
            {
                fixed (byte* pBuffer = m_span.Array)
                {
                    m_mask.FillVSpan(x, y, pBuffer, (int)len);
                    m_pixf.BlendSolidVSpan(x, y, len, c, pBuffer);
                }
            }
        }

        //--------------------------------------------------------------------
        public override void BlendVLine(int x, int y1, int y2, RGBA_Bytes c, byte cover)
        {
            int len = y2 - y1 + 1;
            InitSpan(len, cover);
            unsafe
            {
                fixed (byte* pBuffer = m_span.Array)
                {
                    m_mask.CombineVSpan(x, y1, pBuffer, len);
                    throw new System.NotImplementedException("blend_solid_vspan does not take a y2 yet");
                    //m_pixf.blend_solid_vspan(x, y1, y2, c, pBuffer);
                }
            }
        }

        //--------------------------------------------------------------------
        public override unsafe void BlendSolidHSpan(int x, int y, uint len, RGBA_Bytes c, byte* covers)
        {
            //init_span((int)len, covers);
            unsafe
            {
                fixed (byte* pBuffer = m_span.Array)
                {
                    //m_mask.combine_hspan(x, y, pBuffer, (int)len);
                    //m_pixf.blend_solid_hspan(x, y, len, c, pBuffer);
                    m_mask.CombineHSpan(x, y, covers, (int)len);
                    m_pixf.BlendSolidHSpan(x, y, len, c, covers);
                }
            }
        }


        //--------------------------------------------------------------------
        public override unsafe void BlendSolidVSpan(int x, int y, uint len, RGBA_Bytes c, byte* covers)
        {
            InitSpan((int)len, covers);
            unsafe
            {
                fixed (byte* pBuffer = m_span.Array)
                {
                    m_mask.CombineVSpan(x, y, pBuffer, (int)len);
                    m_pixf.BlendSolidVSpan(x, y, len, c, pBuffer);
                }
            }
        }


        //--------------------------------------------------------------------
        public override unsafe void CopyColorHSpan(int x, int y, uint len, RGBA_Bytes* colors)
        {
            ReAllocSpan((int)len);
            unsafe
            {
                fixed (byte* pBuffer = m_span.Array)
                {
                    m_mask.FillHSpan(x, y, pBuffer, (int)len);
                    m_pixf.BlendColorHSpan(x, y, len, colors, pBuffer, CoverFull);
                }
            }
        }

        //--------------------------------------------------------------------
        public override unsafe void CopyColorVSpan(int x, int y, uint len, RGBA_Bytes* colors)
        {
            ReAllocSpan((int)len);
            unsafe
            {
                fixed (byte* pBuffer = m_span.Array)
                {
                    m_mask.FillVSpan(x, y, pBuffer, (int)len);
                    m_pixf.BlendColorVSpan(x, y, len, colors, pBuffer, CoverFull);
                }
            }
        }

        //--------------------------------------------------------------------
        public override unsafe void BlendColorHSpan(int x, int y, uint len, RGBA_Bytes* colors, byte* covers, byte cover)
        {
            unsafe
            {
                fixed (byte* pBuffer = m_span.Array)
                {
                    if (covers != null)
                    {
                        InitSpan((int)len, covers);
                        m_mask.CombineHSpan(x, y, pBuffer, (int)len);
                    }
                    else
                    {
                        ReAllocSpan((int)len);
                        m_mask.FillHSpan(x, y, pBuffer, (int)len);
                    }
                    m_pixf.BlendColorHSpan(x, y, len, colors, pBuffer, cover);
                }
            }
        }


        //--------------------------------------------------------------------
        public override unsafe void BlendColorVSpan(int x, int y, uint len, RGBA_Bytes* colors, byte* covers, byte cover)
        {
            unsafe
            {
                fixed (byte* pBuffer = m_span.Array)
                {
                    if (covers != null)
                    {
                        InitSpan((int)len, covers);
                        m_mask.CombineVSpan(x, y, pBuffer, (int)len);
                    }
                    else
                    {
                        ReAllocSpan((int)len);
                        m_mask.FillVSpan(x, y, pBuffer, (int)len);
                    }
                    m_pixf.BlendColorVSpan(x, y, len, colors, pBuffer, cover);
                }
            }
        }
    };
}
