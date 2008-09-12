
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
// The Stack Blur Algorithm was invented by Mario Klingemann, 
// mario@quasimondo.com and described here:
// http://incubator.quasimondo.com/processing/fast_blur_deluxe.php
// (search phrase "Stackblur: Fast But Goodlooking"). 
// The major improvement is that there's no more division table
// that was very expensive to create for large blur radii. Insted, 
// for 8-bit per channel and radius not exceeding 254 the division is 
// replaced by multiplication and shift. 
//
//----------------------------------------------------------------------------
using System;
using AGG.Color;
using AGG.PixelFormat;

namespace AGG
{
    /*
    //template<class T> 
    struct stack_blur_tables
    {
        public static ushort[] g_stack_blur8_mul = 
        {
            512,512,456,512,328,456,335,512,405,328,271,456,388,335,292,512,
            454,405,364,328,298,271,496,456,420,388,360,335,312,292,273,512,
            482,454,428,405,383,364,345,328,312,298,284,271,259,496,475,456,
            437,420,404,388,374,360,347,335,323,312,302,292,282,273,265,512,
            497,482,468,454,441,428,417,405,394,383,373,364,354,345,337,328,
            320,312,305,298,291,284,278,271,265,259,507,496,485,475,465,456,
            446,437,428,420,412,404,396,388,381,374,367,360,354,347,341,335,
            329,323,318,312,307,302,297,292,287,282,278,273,269,265,261,512,
            505,497,489,482,475,468,461,454,447,441,435,428,422,417,411,405,
            399,394,389,383,378,373,368,364,359,354,350,345,341,337,332,328,
            324,320,316,312,309,305,301,298,294,291,287,284,281,278,274,271,
            268,265,262,259,257,507,501,496,491,485,480,475,470,465,460,456,
            451,446,442,437,433,428,424,420,416,412,408,404,400,396,392,388,
            385,381,377,374,370,367,363,360,357,354,350,347,344,341,338,335,
            332,329,326,323,320,318,315,312,310,307,304,302,299,297,294,292,
            289,287,285,282,280,278,275,273,271,269,267,265,263,261,259
        };

        public static byte[] g_stack_blur8_shr = 
        {
              9, 11, 12, 13, 13, 14, 14, 15, 15, 15, 15, 16, 16, 16, 16, 17, 
             17, 17, 17, 17, 17, 17, 18, 18, 18, 18, 18, 18, 18, 18, 18, 19, 
             19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 20, 20, 20,
             20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 21,
             21, 21, 21, 21, 21, 21, 21, 21, 21, 21, 21, 21, 21, 21, 21, 21,
             21, 21, 21, 21, 21, 21, 21, 21, 21, 21, 22, 22, 22, 22, 22, 22, 
             22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 22,
             22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 23, 
             23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23,
             23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23,
             23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 
             23, 23, 23, 23, 23, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 
             24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24,
             24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24,
             24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24,
             24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24
        };
    };

    //==============================================================stack_blur
    //template<class ColorT, class CalculatorT> 
    class stack_blur
    {
        //private pod_vector<color_type> m_buf;
        //private pod_vector<color_type> m_stack;
        uint base_mask = 255;

        enum order_e 
        { 
            R = 2, 
            G = 1, 
            B = 0,
            A = 3
        };

        //typedef ColorT      color_type;
        //typedef CalculatorT calculator_type;

        public void blur_x(IPixelFormat img, uint radius)
        {
            if(radius < 1) return;

            uint x, y, xp, i;
            uint stack_ptr;
            uint stack_start;

            color_type      pix;
            color_type*     stack_pix;
            calculator_type sum;
            calculator_type sum_in;
            calculator_type sum_out;

            uint w   = img.width();
            uint h   = img.height();
            uint wm  = w - 1;
            uint div = radius * 2 + 1;

            uint div_sum = (radius + 1) * (radius + 1);
            uint mul_sum = 0;
            uint shr_sum = 0;
            uint max_val = base_mask;

            if(max_val <= 255 && radius < 255)
            {
                mul_sum = stack_blur_tables.g_stack_blur8_mul[radius];
                shr_sum = stack_blur_tables.g_stack_blur8_shr[radius];
            }

            m_buf.allocate(w, 128);
            m_stack.allocate(div, 32);

            for(y = 0; y < h; y++)
            {
                sum.clear();
                sum_in.clear();
                sum_out.clear();

                pix = img.pixel(0, y);
                for(i = 0; i <= radius; i++)
                {
                    m_stack[i] = pix;
                    sum.add(pix, i + 1);
                    sum_out.add(pix);
                }
                for(i = 1; i <= radius; i++)
                {
                    pix = img.pixel((i > wm) ? wm : i, y);
                    m_stack[i + radius] = pix;
                    sum.add(pix, radius + 1 - i);
                    sum_in.add(pix);
                }

                stack_ptr = radius;
                for(x = 0; x < w; x++)
                {
                    if(mul_sum) sum.calc_pix(m_buf[x], mul_sum, shr_sum);
                    else        sum.calc_pix(m_buf[x], div_sum);

                    sum.sub(sum_out);
           
                    stack_start = stack_ptr + div - radius;
                    if(stack_start >= div) stack_start -= div;
                    stack_pix = &m_stack[stack_start];

                    sum_out.sub(*stack_pix);

                    xp = x + radius + 1;
                    if(xp > wm) xp = wm;
                    pix = img.pixel(xp, y);
            
                    *stack_pix = pix;
            
                    sum_in.add(pix);
                    sum.add(sum_in);
            
                    ++stack_ptr;
                    if(stack_ptr >= div) stack_ptr = 0;
                    stack_pix = &m_stack[stack_ptr];

                    sum_out.add(*stack_pix);
                    sum_in.sub(*stack_pix);
                }
                img.copy_color_hspan(0, y, w, &m_buf[0]);
            }
        }

        public void blur_y(IPixelFormat img, uint radius)
        {
            pixfmt_transposer<Img> img2(img);
            blur_x(img2, radius);
        }

        public void blur(IPixelFormat img, uint radius)
        {
            blur_x(img, radius);
            pixfmt_transposer<Img> img2(img);
            blur_x(img2, radius);
        }

        void stack_blur_gray8(IPixelFormat img, uint rx, uint ry)
        {
            uint x, y, xp, yp, i;
            uint stack_ptr;
            uint stack_start;

            byte* src_pix_ptr;
                  byte* dst_pix_ptr;
            uint pix;
            uint stack_pix;
            uint sum;
            uint sum_in;
            uint sum_out;

            uint w   = img.width();
            uint h   = img.height();
            uint wm  = w - 1;
            uint hm  = h - 1;

            uint div;
            uint mul_sum;
            uint shr_sum;

            pod_vector<byte> stack;

            if(rx > 0)
            {
                if(rx > 254) rx = 254;
                div = rx * 2 + 1;
                mul_sum = stack_blur_tables.g_stack_blur8_mul[rx];
                shr_sum = stack_blur_tables.g_stack_blur8_shr[rx];
                stack.allocate(div);

                for(y = 0; y < h; y++)
                {
                    sum = sum_in = sum_out = 0;

                    src_pix_ptr = img.pix_ptr(0, y);
                    pix = *src_pix_ptr;
                    for(i = 0; i <= rx; i++)
                    {
                        stack[i] = pix;
                        sum     += pix * (i + 1);
                        sum_out += pix;
                    }
                    for(i = 1; i <= rx; i++)
                    {
                        if(i <= wm) src_pix_ptr += Img::pix_step; 
                        pix = *src_pix_ptr; 
                        stack[i + rx] = pix;
                        sum    += pix * (rx + 1 - i);
                        sum_in += pix;
                    }

                    stack_ptr = rx;
                    xp = rx;
                    if(xp > wm) xp = wm;
                    src_pix_ptr = img.pix_ptr(xp, y);
                    dst_pix_ptr = img.pix_ptr(0, y);
                    for(x = 0; x < w; x++)
                    {
                        *dst_pix_ptr = (sum * mul_sum) >> shr_sum;
                        dst_pix_ptr += Img::pix_step;

                        sum -= sum_out;
           
                        stack_start = stack_ptr + div - rx;
                        if(stack_start >= div) stack_start -= div;
                        sum_out -= stack[stack_start];

                        if(xp < wm) 
                        {
                            src_pix_ptr += Img::pix_step;
                            pix = *src_pix_ptr;
                            ++xp;
                        }
            
                        stack[stack_start] = pix;
            
                        sum_in += pix;
                        sum    += sum_in;
            
                        ++stack_ptr;
                        if(stack_ptr >= div) stack_ptr = 0;
                        stack_pix = stack[stack_ptr];

                        sum_out += stack_pix;
                        sum_in  -= stack_pix;
                    }
                }
            }

            if(ry > 0)
            {
                if(ry > 254) ry = 254;
                div = ry * 2 + 1;
                mul_sum = stack_blur_tables.g_stack_blur8_mul[ry];
                shr_sum = stack_blur_tables.g_stack_blur8_shr[ry];
                stack.allocate(div);

                int stride = img.stride();
                for(x = 0; x < w; x++)
                {
                    sum = sum_in = sum_out = 0;

                    src_pix_ptr = img.pix_ptr(x, 0);
                    pix = *src_pix_ptr;
                    for(i = 0; i <= ry; i++)
                    {
                        stack[i] = pix;
                        sum     += pix * (i + 1);
                        sum_out += pix;
                    }
                    for(i = 1; i <= ry; i++)
                    {
                        if(i <= hm) src_pix_ptr += stride; 
                        pix = *src_pix_ptr; 
                        stack[i + ry] = pix;
                        sum    += pix * (ry + 1 - i);
                        sum_in += pix;
                    }

                    stack_ptr = ry;
                    yp = ry;
                    if(yp > hm) yp = hm;
                    src_pix_ptr = img.pix_ptr(x, yp);
                    dst_pix_ptr = img.pix_ptr(x, 0);
                    for(y = 0; y < h; y++)
                    {
                        *dst_pix_ptr = (sum * mul_sum) >> shr_sum;
                        dst_pix_ptr += stride;

                        sum -= sum_out;
           
                        stack_start = stack_ptr + div - ry;
                        if(stack_start >= div) stack_start -= div;
                        sum_out -= stack[stack_start];

                        if(yp < hm) 
                        {
                            src_pix_ptr += stride;
                            pix = *src_pix_ptr;
                            ++yp;
                        }
            
                        stack[stack_start] = pix;
            
                        sum_in += pix;
                        sum    += sum_in;
            
                        ++stack_ptr;
                        if(stack_ptr >= div) stack_ptr = 0;
                        stack_pix = stack[stack_ptr];

                        sum_out += stack_pix;
                        sum_in  -= stack_pix;
                    }
                }
            }
        }

        void stack_blur_bgr24(IPixelFormat img, uint rx, uint ry)
        {
            //typedef typename Img::color_type color_type;
            //typedef typename Img::order_type order_type;

            uint x, y, xp, yp, i;
            uint stack_ptr;
            uint stack_start;

            byte* src_pix_ptr;
                  byte* dst_pix_ptr;
            color_type*  stack_pix_ptr;

            uint sum_r;
            uint sum_g;
            uint sum_b;
            uint sum_in_r;
            uint sum_in_g;
            uint sum_in_b;
            uint sum_out_r;
            uint sum_out_g;
            uint sum_out_b;

            uint w   = img.width();
            uint h   = img.height();
            uint wm  = w - 1;
            uint hm  = h - 1;

            uint div;
            uint mul_sum;
            uint shr_sum;

            pod_vector<color_type> stack;

            if(rx > 0)
            {
                if(rx > 254) rx = 254;
                div = rx * 2 + 1;
                mul_sum = stack_blur_tables.g_stack_blur8_mul[rx];
                shr_sum = stack_blur_tables.g_stack_blur8_shr[rx];
                stack.allocate(div);

                for(y = 0; y < h; y++)
                {
                    sum_r = 
                    sum_g = 
                    sum_b = 
                    sum_in_r = 
                    sum_in_g = 
                    sum_in_b = 
                    sum_out_r = 
                    sum_out_g = 
                    sum_out_b = 0;

                    src_pix_ptr = img.pix_ptr(0, y);
                    for(i = 0; i <= rx; i++)
                    {
                        stack_pix_ptr    = &stack[i];
                        stack_pix_ptr->r = src_pix_ptr[R];
                        stack_pix_ptr->g = src_pix_ptr[G];
                        stack_pix_ptr->b = src_pix_ptr[B];
                        sum_r           += src_pix_ptr[R] * (i + 1);
                        sum_g           += src_pix_ptr[G] * (i + 1);
                        sum_b           += src_pix_ptr[B] * (i + 1);
                        sum_out_r       += src_pix_ptr[R];
                        sum_out_g       += src_pix_ptr[G];
                        sum_out_b       += src_pix_ptr[B];
                    }
                    for(i = 1; i <= rx; i++)
                    {
                        if(i <= wm) src_pix_ptr += Img::pix_width; 
                        stack_pix_ptr = &stack[i + rx];
                        stack_pix_ptr->r = src_pix_ptr[R];
                        stack_pix_ptr->g = src_pix_ptr[G];
                        stack_pix_ptr->b = src_pix_ptr[B];
                        sum_r           += src_pix_ptr[R] * (rx + 1 - i);
                        sum_g           += src_pix_ptr[G] * (rx + 1 - i);
                        sum_b           += src_pix_ptr[B] * (rx + 1 - i);
                        sum_in_r        += src_pix_ptr[R];
                        sum_in_g        += src_pix_ptr[G];
                        sum_in_b        += src_pix_ptr[B];
                    }

                    stack_ptr = rx;
                    xp = rx;
                    if(xp > wm) xp = wm;
                    src_pix_ptr = img.pix_ptr(xp, y);
                    dst_pix_ptr = img.pix_ptr(0, y);
                    for(x = 0; x < w; x++)
                    {
                        dst_pix_ptr[R] = (sum_r * mul_sum) >> shr_sum;
                        dst_pix_ptr[G] = (sum_g * mul_sum) >> shr_sum;
                        dst_pix_ptr[B] = (sum_b * mul_sum) >> shr_sum;
                        dst_pix_ptr   += Img::pix_width;

                        sum_r -= sum_out_r;
                        sum_g -= sum_out_g;
                        sum_b -= sum_out_b;
           
                        stack_start = stack_ptr + div - rx;
                        if(stack_start >= div) stack_start -= div;
                        stack_pix_ptr = &stack[stack_start];

                        sum_out_r -= stack_pix_ptr->r;
                        sum_out_g -= stack_pix_ptr->g;
                        sum_out_b -= stack_pix_ptr->b;

                        if(xp < wm) 
                        {
                            src_pix_ptr += Img::pix_width;
                            ++xp;
                        }
            
                        stack_pix_ptr->r = src_pix_ptr[R];
                        stack_pix_ptr->g = src_pix_ptr[G];
                        stack_pix_ptr->b = src_pix_ptr[B];
            
                        sum_in_r += src_pix_ptr[R];
                        sum_in_g += src_pix_ptr[G];
                        sum_in_b += src_pix_ptr[B];
                        sum_r    += sum_in_r;
                        sum_g    += sum_in_g;
                        sum_b    += sum_in_b;
            
                        ++stack_ptr;
                        if(stack_ptr >= div) stack_ptr = 0;
                        stack_pix_ptr = &stack[stack_ptr];

                        sum_out_r += stack_pix_ptr->r;
                        sum_out_g += stack_pix_ptr->g;
                        sum_out_b += stack_pix_ptr->b;
                        sum_in_r  -= stack_pix_ptr->r;
                        sum_in_g  -= stack_pix_ptr->g;
                        sum_in_b  -= stack_pix_ptr->b;
                    }
                }
            }

            if(ry > 0)
            {
                if(ry > 254) ry = 254;
                div = ry * 2 + 1;
                mul_sum = stack_blur_tables.g_stack_blur8_mul[ry];
                shr_sum = stack_blur_tables.g_stack_blur8_shr[ry];
                stack.allocate(div);

                int stride = img.stride();
                for(x = 0; x < w; x++)
                {
                    sum_r = 
                    sum_g = 
                    sum_b = 
                    sum_in_r = 
                    sum_in_g = 
                    sum_in_b = 
                    sum_out_r = 
                    sum_out_g = 
                    sum_out_b = 0;

                    src_pix_ptr = img.pix_ptr(x, 0);
                    for(i = 0; i <= ry; i++)
                    {
                        stack_pix_ptr    = &stack[i];
                        stack_pix_ptr->r = src_pix_ptr[R];
                        stack_pix_ptr->g = src_pix_ptr[G];
                        stack_pix_ptr->b = src_pix_ptr[B];
                        sum_r           += src_pix_ptr[R] * (i + 1);
                        sum_g           += src_pix_ptr[G] * (i + 1);
                        sum_b           += src_pix_ptr[B] * (i + 1);
                        sum_out_r       += src_pix_ptr[R];
                        sum_out_g       += src_pix_ptr[G];
                        sum_out_b       += src_pix_ptr[B];
                    }
                    for(i = 1; i <= ry; i++)
                    {
                        if(i <= hm) src_pix_ptr += stride; 
                        stack_pix_ptr = &stack[i + ry];
                        stack_pix_ptr->r = src_pix_ptr[R];
                        stack_pix_ptr->g = src_pix_ptr[G];
                        stack_pix_ptr->b = src_pix_ptr[B];
                        sum_r           += src_pix_ptr[R] * (ry + 1 - i);
                        sum_g           += src_pix_ptr[G] * (ry + 1 - i);
                        sum_b           += src_pix_ptr[B] * (ry + 1 - i);
                        sum_in_r        += src_pix_ptr[R];
                        sum_in_g        += src_pix_ptr[G];
                        sum_in_b        += src_pix_ptr[B];
                    }

                    stack_ptr = ry;
                    yp = ry;
                    if(yp > hm) yp = hm;
                    src_pix_ptr = img.pix_ptr(x, yp);
                    dst_pix_ptr = img.pix_ptr(x, 0);
                    for(y = 0; y < h; y++)
                    {
                        dst_pix_ptr[R] = (sum_r * mul_sum) >> shr_sum;
                        dst_pix_ptr[G] = (sum_g * mul_sum) >> shr_sum;
                        dst_pix_ptr[B] = (sum_b * mul_sum) >> shr_sum;
                        dst_pix_ptr += stride;

                        sum_r -= sum_out_r;
                        sum_g -= sum_out_g;
                        sum_b -= sum_out_b;
           
                        stack_start = stack_ptr + div - ry;
                        if(stack_start >= div) stack_start -= div;

                        stack_pix_ptr = &stack[stack_start];
                        sum_out_r -= stack_pix_ptr->r;
                        sum_out_g -= stack_pix_ptr->g;
                        sum_out_b -= stack_pix_ptr->b;

                        if(yp < hm) 
                        {
                            src_pix_ptr += stride;
                            ++yp;
                        }
            
                        stack_pix_ptr->r = src_pix_ptr[R];
                        stack_pix_ptr->g = src_pix_ptr[G];
                        stack_pix_ptr->b = src_pix_ptr[B];
            
                        sum_in_r += src_pix_ptr[R];
                        sum_in_g += src_pix_ptr[G];
                        sum_in_b += src_pix_ptr[B];
                        sum_r    += sum_in_r;
                        sum_g    += sum_in_g;
                        sum_b    += sum_in_b;
            
                        ++stack_ptr;
                        if(stack_ptr >= div) stack_ptr = 0;
                        stack_pix_ptr = &stack[stack_ptr];

                        sum_out_r += stack_pix_ptr->r;
                        sum_out_g += stack_pix_ptr->g;
                        sum_out_b += stack_pix_ptr->b;
                        sum_in_r  -= stack_pix_ptr->r;
                        sum_in_g  -= stack_pix_ptr->g;
                        sum_in_b  -= stack_pix_ptr->b;
                    }
                }
            }
        }

        void stack_blur_rgba32(IPixelFormat img, uint rx, uint ry)
        {
            //typedef typename Img::color_type color_type;
            //typedef typename Img::order_type order_type;

            uint x, y, xp, yp, i;
            uint stack_ptr;
            uint stack_start;

            byte* src_pix_ptr;
                  byte* dst_pix_ptr;
            color_type*  stack_pix_ptr;

            uint sum_r;
            uint sum_g;
            uint sum_b;
            uint sum_a;
            uint sum_in_r;
            uint sum_in_g;
            uint sum_in_b;
            uint sum_in_a;
            uint sum_out_r;
            uint sum_out_g;
            uint sum_out_b;
            uint sum_out_a;

            uint w   = img.width();
            uint h   = img.height();
            uint wm  = w - 1;
            uint hm  = h - 1;

            uint div;
            uint mul_sum;
            uint shr_sum;

            pod_vector<color_type> stack;

            if(rx > 0)
            {
                if(rx > 254) rx = 254;
                div = rx * 2 + 1;
                mul_sum = stack_blur_tables.g_stack_blur8_mul[rx];
                shr_sum = stack_blur_tables.g_stack_blur8_shr[rx];
                stack.allocate(div);

                for(y = 0; y < h; y++)
                {
                    sum_r = 
                    sum_g = 
                    sum_b = 
                    sum_a = 
                    sum_in_r = 
                    sum_in_g = 
                    sum_in_b = 
                    sum_in_a = 
                    sum_out_r = 
                    sum_out_g = 
                    sum_out_b = 
                    sum_out_a = 0;

                    src_pix_ptr = img.pix_ptr(0, y);
                    for(i = 0; i <= rx; i++)
                    {
                        stack_pix_ptr    = &stack[i];
                        stack_pix_ptr->r = src_pix_ptr[R];
                        stack_pix_ptr->g = src_pix_ptr[G];
                        stack_pix_ptr->b = src_pix_ptr[B];
                        stack_pix_ptr->a = src_pix_ptr[A];
                        sum_r           += src_pix_ptr[R] * (i + 1);
                        sum_g           += src_pix_ptr[G] * (i + 1);
                        sum_b           += src_pix_ptr[B] * (i + 1);
                        sum_a           += src_pix_ptr[A] * (i + 1);
                        sum_out_r       += src_pix_ptr[R];
                        sum_out_g       += src_pix_ptr[G];
                        sum_out_b       += src_pix_ptr[B];
                        sum_out_a       += src_pix_ptr[A];
                    }
                    for(i = 1; i <= rx; i++)
                    {
                        if(i <= wm) src_pix_ptr += Img::pix_width; 
                        stack_pix_ptr = &stack[i + rx];
                        stack_pix_ptr->r = src_pix_ptr[R];
                        stack_pix_ptr->g = src_pix_ptr[G];
                        stack_pix_ptr->b = src_pix_ptr[B];
                        stack_pix_ptr->a = src_pix_ptr[A];
                        sum_r           += src_pix_ptr[R] * (rx + 1 - i);
                        sum_g           += src_pix_ptr[G] * (rx + 1 - i);
                        sum_b           += src_pix_ptr[B] * (rx + 1 - i);
                        sum_a           += src_pix_ptr[A] * (rx + 1 - i);
                        sum_in_r        += src_pix_ptr[R];
                        sum_in_g        += src_pix_ptr[G];
                        sum_in_b        += src_pix_ptr[B];
                        sum_in_a        += src_pix_ptr[A];
                    }

                    stack_ptr = rx;
                    xp = rx;
                    if(xp > wm) xp = wm;
                    src_pix_ptr = img.pix_ptr(xp, y);
                    dst_pix_ptr = img.pix_ptr(0, y);
                    for(x = 0; x < w; x++)
                    {
                        dst_pix_ptr[R] = (sum_r * mul_sum) >> shr_sum;
                        dst_pix_ptr[G] = (sum_g * mul_sum) >> shr_sum;
                        dst_pix_ptr[B] = (sum_b * mul_sum) >> shr_sum;
                        dst_pix_ptr[A] = (sum_a * mul_sum) >> shr_sum;
                        dst_pix_ptr += Img::pix_width;

                        sum_r -= sum_out_r;
                        sum_g -= sum_out_g;
                        sum_b -= sum_out_b;
                        sum_a -= sum_out_a;
           
                        stack_start = stack_ptr + div - rx;
                        if(stack_start >= div) stack_start -= div;
                        stack_pix_ptr = &stack[stack_start];

                        sum_out_r -= stack_pix_ptr->r;
                        sum_out_g -= stack_pix_ptr->g;
                        sum_out_b -= stack_pix_ptr->b;
                        sum_out_a -= stack_pix_ptr->a;

                        if(xp < wm) 
                        {
                            src_pix_ptr += Img::pix_width;
                            ++xp;
                        }
            
                        stack_pix_ptr->r = src_pix_ptr[R];
                        stack_pix_ptr->g = src_pix_ptr[G];
                        stack_pix_ptr->b = src_pix_ptr[B];
                        stack_pix_ptr->a = src_pix_ptr[A];
            
                        sum_in_r += src_pix_ptr[R];
                        sum_in_g += src_pix_ptr[G];
                        sum_in_b += src_pix_ptr[B];
                        sum_in_a += src_pix_ptr[A];
                        sum_r    += sum_in_r;
                        sum_g    += sum_in_g;
                        sum_b    += sum_in_b;
                        sum_a    += sum_in_a;
            
                        ++stack_ptr;
                        if(stack_ptr >= div) stack_ptr = 0;
                        stack_pix_ptr = &stack[stack_ptr];

                        sum_out_r += stack_pix_ptr->r;
                        sum_out_g += stack_pix_ptr->g;
                        sum_out_b += stack_pix_ptr->b;
                        sum_out_a += stack_pix_ptr->a;
                        sum_in_r  -= stack_pix_ptr->r;
                        sum_in_g  -= stack_pix_ptr->g;
                        sum_in_b  -= stack_pix_ptr->b;
                        sum_in_a  -= stack_pix_ptr->a;
                    }
                }
            }

            if(ry > 0)
            {
                if(ry > 254) ry = 254;
                div = ry * 2 + 1;
                mul_sum = stack_blur_tables.g_stack_blur8_mul[ry];
                shr_sum = stack_blur_tables.g_stack_blur8_shr[ry];
                stack.allocate(div);

                int stride = img.stride();
                for(x = 0; x < w; x++)
                {
                    sum_r = 
                    sum_g = 
                    sum_b = 
                    sum_a = 
                    sum_in_r = 
                    sum_in_g = 
                    sum_in_b = 
                    sum_in_a = 
                    sum_out_r = 
                    sum_out_g = 
                    sum_out_b = 
                    sum_out_a = 0;

                    src_pix_ptr = img.pix_ptr(x, 0);
                    for(i = 0; i <= ry; i++)
                    {
                        stack_pix_ptr    = &stack[i];
                        stack_pix_ptr->r = src_pix_ptr[R];
                        stack_pix_ptr->g = src_pix_ptr[G];
                        stack_pix_ptr->b = src_pix_ptr[B];
                        stack_pix_ptr->a = src_pix_ptr[A];
                        sum_r           += src_pix_ptr[R] * (i + 1);
                        sum_g           += src_pix_ptr[G] * (i + 1);
                        sum_b           += src_pix_ptr[B] * (i + 1);
                        sum_a           += src_pix_ptr[A] * (i + 1);
                        sum_out_r       += src_pix_ptr[R];
                        sum_out_g       += src_pix_ptr[G];
                        sum_out_b       += src_pix_ptr[B];
                        sum_out_a       += src_pix_ptr[A];
                    }
                    for(i = 1; i <= ry; i++)
                    {
                        if(i <= hm) src_pix_ptr += stride; 
                        stack_pix_ptr = &stack[i + ry];
                        stack_pix_ptr->r = src_pix_ptr[R];
                        stack_pix_ptr->g = src_pix_ptr[G];
                        stack_pix_ptr->b = src_pix_ptr[B];
                        stack_pix_ptr->a = src_pix_ptr[A];
                        sum_r           += src_pix_ptr[R] * (ry + 1 - i);
                        sum_g           += src_pix_ptr[G] * (ry + 1 - i);
                        sum_b           += src_pix_ptr[B] * (ry + 1 - i);
                        sum_a           += src_pix_ptr[A] * (ry + 1 - i);
                        sum_in_r        += src_pix_ptr[R];
                        sum_in_g        += src_pix_ptr[G];
                        sum_in_b        += src_pix_ptr[B];
                        sum_in_a        += src_pix_ptr[A];
                    }

                    stack_ptr = ry;
                    yp = ry;
                    if(yp > hm) yp = hm;
                    src_pix_ptr = img.pix_ptr(x, yp);
                    dst_pix_ptr = img.pix_ptr(x, 0);
                    for(y = 0; y < h; y++)
                    {
                        dst_pix_ptr[R] = (sum_r * mul_sum) >> shr_sum;
                        dst_pix_ptr[G] = (sum_g * mul_sum) >> shr_sum;
                        dst_pix_ptr[B] = (sum_b * mul_sum) >> shr_sum;
                        dst_pix_ptr[A] = (sum_a * mul_sum) >> shr_sum;
                        dst_pix_ptr += stride;

                        sum_r -= sum_out_r;
                        sum_g -= sum_out_g;
                        sum_b -= sum_out_b;
                        sum_a -= sum_out_a;
           
                        stack_start = stack_ptr + div - ry;
                        if(stack_start >= div) stack_start -= div;

                        stack_pix_ptr = &stack[stack_start];
                        sum_out_r -= stack_pix_ptr->r;
                        sum_out_g -= stack_pix_ptr->g;
                        sum_out_b -= stack_pix_ptr->b;
                        sum_out_a -= stack_pix_ptr->a;

                        if(yp < hm) 
                        {
                            src_pix_ptr += stride;
                            ++yp;
                        }
            
                        stack_pix_ptr->r = src_pix_ptr[R];
                        stack_pix_ptr->g = src_pix_ptr[G];
                        stack_pix_ptr->b = src_pix_ptr[B];
                        stack_pix_ptr->a = src_pix_ptr[A];
            
                        sum_in_r += src_pix_ptr[R];
                        sum_in_g += src_pix_ptr[G];
                        sum_in_b += src_pix_ptr[B];
                        sum_in_a += src_pix_ptr[A];
                        sum_r    += sum_in_r;
                        sum_g    += sum_in_g;
                        sum_b    += sum_in_b;
                        sum_a    += sum_in_a;
            
                        ++stack_ptr;
                        if(stack_ptr >= div) stack_ptr = 0;
                        stack_pix_ptr = &stack[stack_ptr];

                        sum_out_r += stack_pix_ptr->r;
                        sum_out_g += stack_pix_ptr->g;
                        sum_out_b += stack_pix_ptr->b;
                        sum_out_a += stack_pix_ptr->a;
                        sum_in_r  -= stack_pix_ptr->r;
                        sum_in_g  -= stack_pix_ptr->g;
                        sum_in_b  -= stack_pix_ptr->b;
                        sum_in_a  -= stack_pix_ptr->a;
                    }
                }
            }
        }
    };

    //====================================================stack_blur_calc_rgba
    //template<class T=uint> 
    struct stack_blur_calc_rgba
    {
        //typedef T value_type;
        uint r,g,b,a;

        void clear() 
        { 
            r = g = b = a = 0; 
        }

        //template<class ArgT> 
        void add(uint v)
        {
            r += v.r;
            g += v.g;
            b += v.b;
            a += v.a;
        }

        //template<class ArgT> 
        void add(uint v, uint k)
        {
            r += v.r * k;
            g += v.g * k;
            b += v.b * k;
            a += v.a * k;
        }

        //template<class ArgT> 
        void sub(uint v)
        {
            r -= v.r;
            g -= v.g;
            b -= v.b;
            a -= v.a;
        }

        //template<class ArgT> 
        void calc_pix(uint v, uint div)
        {
            //typedef typename ArgT::value_type value_type;
            v.r = value_type(r / div);
            v.g = value_type(g / div);
            v.b = value_type(b / div);
            v.a = value_type(a / div);
        }

        //template<class ArgT> 
        void calc_pix(uint v, uint mul, uint shr)
        {
            //typedef typename ArgT::value_type value_type;
            v.r = value_type((r * mul) >> shr);
            v.g = value_type((g * mul) >> shr);
            v.b = value_type((b * mul) >> shr);
            v.a = value_type((a * mul) >> shr);
        }
    };


    //=====================================================stack_blur_calc_rgb
    //template<class T=uint> 
    struct stack_blur_calc_rgb
    {
        //typedef T value_type;
        uint r,g,b;

        void clear() 
        { 
            r = g = b = 0; 
        }

        //template<class ArgT> 
        void add(uint v)
        {
            r += v.r;
            g += v.g;
            b += v.b;
        }

        //template<class ArgT> 
        void add(uint v, uint k)
        {
            r += v.r * k;
            g += v.g * k;
            b += v.b * k;
        }

        //template<class ArgT> 
        void sub(uint v)
        {
            r -= v.r;
            g -= v.g;
            b -= v.b;
        }

        //template<class ArgT> 
        void calc_pix(uint v, uint div)
        {
            typedef typename ArgT::value_type value_type;
            v.r = value_type(r / div);
            v.g = value_type(g / div);
            v.b = value_type(b / div);
        }

        //template<class ArgT> 
        void calc_pix(uint v, uint mul, uint shr)
        {
            typedef typename ArgT::value_type value_type;
            v.r = value_type((r * mul) >> shr);
            v.g = value_type((g * mul) >> shr);
            v.b = value_type((b * mul) >> shr);
        }
    };


    //====================================================stack_blur_calc_gray
    //template<class T=uint> 
    struct stack_blur_calc_gray
    {
        //typedef T value_type;
        uint v;

        void clear() 
        { 
            v = 0; 
        }

        //template<class ArgT> 
        void add(uint a)
        {
            v += a.v;
        }

        //template<class ArgT> 
        void add(uint a, uint k)
        {
            v += a.v * k;
        }

        //template<class ArgT> 
        void sub(uint a)
        {
            v -= a.v;
        }

        //template<class ArgT> 
        void calc_pix(uint a, uint div)
        {
            //typedef typename ArgT::value_type value_type;
            a.v = (uint)(v / div);
        }

        //template<class ArgT> 
        void calc_pix(uint a, uint mul, uint shr)
        {
            //typedef typename ArgT::value_type value_type;
            a.v = (uint)((v * mul) >> shr);
        }
    };

     */

    public abstract class RecursizeBlurCalculator
    {
        public double R, G, B, A;

        public abstract RecursizeBlurCalculator CreateNew();

        public abstract void FromPix(RGBA_Bytes c);

        public abstract void Calc(double b1, double b2, double b3, double b4,
            RecursizeBlurCalculator c1, RecursizeBlurCalculator c2, RecursizeBlurCalculator c3, RecursizeBlurCalculator c4);

        public abstract void ToPix(ref RGBA_Bytes c);
    };

    //===========================================================recursive_blur
    public sealed class RecursiveBlur
    {
        VectorPOD<RecursizeBlurCalculator> m_sum1;
        VectorPOD<RecursizeBlurCalculator> m_sum2;
        VectorPOD<RGBA_Bytes> m_buf;
        RecursizeBlurCalculator m_RecursizeBlurCalculatorFactory;

        public RecursiveBlur(RecursizeBlurCalculator recursizeBluerCalculatorFactory)
        {
            m_sum1 = new VectorPOD<RecursizeBlurCalculator>();
            m_sum2 = new VectorPOD<RecursizeBlurCalculator>();
            m_buf = new VectorPOD<RGBA_Bytes>();
            m_RecursizeBlurCalculatorFactory = recursizeBluerCalculatorFactory;
        }

        public void BlurX(IPixelFormat img, double radius)
        {
            if (radius < 0.62) return;
            if (img.Width < 3) return;

            double s = (double)(radius * 0.5);
            double q = (double)((s < 2.5) ?
                                    3.97156 - 4.14554 * Math.Sqrt(1 - 0.26891 * s) :
                                    0.98711 * s - 0.96330);

            double q2 = (double)(q * q);
            double q3 = (double)(q2 * q);

            double b0 = (double)(1.0 / (1.578250 +
                                            2.444130 * q +
                                            1.428100 * q2 +
                                            0.422205 * q3));

            double b1 = (double)(2.44413 * q +
                                      2.85619 * q2 +
                                      1.26661 * q3);

            double b2 = (double)(-1.42810 * q2 +
                                     -1.26661 * q3);

            double b3 = (double)(0.422205 * q3);

            double b = (double)(1 - (b1 + b2 + b3) * b0);

            b1 *= b0;
            b2 *= b0;
            b3 *= b0;

            uint w = img.Width;
            uint h = img.Height;
            int wm = (int)w - 1;
            int x, y;

            int StartCreatingAt = (int)m_sum1.Size();
            m_sum1.Resize(w);
            m_sum2.Resize(w);
            m_buf.Allocate(w);

            RecursizeBlurCalculator[] Sum1Array = m_sum1.Array;
            RecursizeBlurCalculator[] Sum2Array = m_sum2.Array;
            RGBA_Bytes[] BufferArray = m_buf.Array;

            for (int i = StartCreatingAt; i < w; i++)
            {
                Sum1Array[i] = m_RecursizeBlurCalculatorFactory.CreateNew();
                Sum2Array[i] = m_RecursizeBlurCalculatorFactory.CreateNew();
            }

            for (y = 0; y < h; y++)
            {
                RecursizeBlurCalculator c = m_RecursizeBlurCalculatorFactory;
                c.FromPix(img.Pixel(0, y));
                Sum1Array[0].Calc(b, b1, b2, b3, c, c, c, c);
                c.FromPix(img.Pixel(1, y));
                Sum1Array[1].Calc(b, b1, b2, b3, c, Sum1Array[0], Sum1Array[0], Sum1Array[0]);
                c.FromPix(img.Pixel(2, y));
                Sum1Array[2].Calc(b, b1, b2, b3, c, Sum1Array[1], Sum1Array[0], Sum1Array[0]);

                for (x = 3; x < w; ++x)
                {
                    c.FromPix(img.Pixel(x, y));
                    Sum1Array[x].Calc(b, b1, b2, b3, c, Sum1Array[x - 1], Sum1Array[x - 2], Sum1Array[x - 3]);
                }

                Sum2Array[wm].Calc(b, b1, b2, b3, Sum1Array[wm], Sum1Array[wm], Sum1Array[wm], Sum1Array[wm]);
                Sum2Array[wm - 1].Calc(b, b1, b2, b3, Sum1Array[wm - 1], Sum2Array[wm], Sum2Array[wm], Sum2Array[wm]);
                Sum2Array[wm - 2].Calc(b, b1, b2, b3, Sum1Array[wm - 2], Sum2Array[wm - 1], Sum2Array[wm], Sum2Array[wm]);
                Sum2Array[wm].ToPix(ref BufferArray[wm]);
                Sum2Array[wm - 1].ToPix(ref BufferArray[wm - 1]);
                Sum2Array[wm - 2].ToPix(ref BufferArray[wm - 2]);

                for (x = wm - 3; x >= 0; --x)
                {
                    Sum2Array[x].Calc(b, b1, b2, b3, Sum1Array[x], Sum2Array[x + 1], Sum2Array[x + 2], Sum2Array[x + 3]);
                    Sum2Array[x].ToPix(ref BufferArray[x]);
                }

                unsafe
                {
                    fixed (RGBA_Bytes* pBuffer = BufferArray)
                    {
                        img.CopyColorHSpan(0, y, w, pBuffer);
                    }
                }
            }
        }

        public void BlurY(IPixelFormat img, double radius)
        {
            FormatTransposer img2 = new FormatTransposer(img);
            BlurX(img2, radius);
        }

        public void Blur(IPixelFormat img, double radius)
        {
            BlurX(img, radius);
            BlurY(img, radius);
        }
    };

    //=================================================recursive_blur_calc_rgb
    public sealed class RecursiveBlurCalcRgb : RecursizeBlurCalculator
    {
        public override RecursizeBlurCalculator CreateNew()
        {
            return new RecursiveBlurCalcRgb();
        }

        public override void FromPix(RGBA_Bytes c)
        {
            R = c.R;
            G = c.G;
            B = c.B;
        }

        public override void Calc(double b1, double b2, double b3, double b4,
            RecursizeBlurCalculator c1, RecursizeBlurCalculator c2, RecursizeBlurCalculator c3, RecursizeBlurCalculator c4)
        {
            R = b1 * c1.R + b2 * c2.R + b3 * c3.R + b4 * c4.R;
            G = b1 * c1.G + b2 * c2.G + b3 * c3.G + b4 * c4.G;
            B = b1 * c1.B + b2 * c2.B + b3 * c3.B + b4 * c4.B;
        }

        public override void ToPix(ref RGBA_Bytes c)
        {
            //c.R = (Byte)Basics.RoundUint(R);
            //c.G = (Byte)Basics.RoundUint(G);
            //c.B = (Byte)Basics.RoundUint(B);
            c = new RGBA_Bytes(
                (Byte)Basics.RoundUint(R),
                (Byte)Basics.RoundUint(G),
                (Byte)Basics.RoundUint(B));
        }
    };

    //=================================================recursive_blur_calc_rgba
    public sealed class RecursiveBlurCalcRgba : RecursizeBlurCalculator
    {
        public override RecursizeBlurCalculator CreateNew()
        {
            return new RecursiveBlurCalcRgba();
        }

        public override void FromPix(RGBA_Bytes c)
        {
            R = c.R;
            G = c.G;
            B = c.B;
            A = c.A;
        }

        public override void Calc(double b1, double b2, double b3, double b4,
            RecursizeBlurCalculator c1, RecursizeBlurCalculator c2, RecursizeBlurCalculator c3, RecursizeBlurCalculator c4)
        {
            R = b1 * c1.R + b2 * c2.R + b3 * c3.R + b4 * c4.R;
            G = b1 * c1.G + b2 * c2.G + b3 * c3.G + b4 * c4.G;
            B = b1 * c1.B + b2 * c2.B + b3 * c3.B + b4 * c4.B;
            A = b1 * c1.A + b2 * c2.A + b3 * c3.A + b4 * c4.A;
        }

        public override void ToPix(ref RGBA_Bytes c)
        {
            //c.R = (Byte)Basics.RoundUint(R);
            //c.G = (Byte)Basics.RoundUint(G);
            //c.B = (Byte)Basics.RoundUint(B);
            //c.A = (Byte)Basics.RoundUint(A);
            c = new RGBA_Bytes(
                (Byte)Basics.RoundUint(R),
                (Byte)Basics.RoundUint(G),
                (Byte)Basics.RoundUint(B),
                (Byte)Basics.RoundUint(A));
        }
    };

    //================================================recursive_blur_calc_gray
    public sealed class RecursiveBlurCalcGray : RecursizeBlurCalculator
    {
        public override RecursizeBlurCalculator CreateNew()
        {
            return new RecursiveBlurCalcGray();
        }

        public override void FromPix(RGBA_Bytes c)
        {
            R = c.R;
        }

        public override void Calc(double b1, double b2, double b3, double b4,
            RecursizeBlurCalculator c1, RecursizeBlurCalculator c2, RecursizeBlurCalculator c3, RecursizeBlurCalculator c4)
        {
            R = b1 * c1.R + b2 * c2.R + b3 * c3.R + b4 * c4.R;
        }

        public override void ToPix(ref RGBA_Bytes c)
        {
            c = new RGBA_Bytes((Byte)Basics.RoundUint(R), c.G, c.B, c.A); 
            //c.R = (Byte)Basics.RoundUint(R);
        }
    };
}
