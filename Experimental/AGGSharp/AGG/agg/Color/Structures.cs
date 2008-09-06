
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

namespace AGG.Color
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
  

    //===================================================================rgba8
  
}