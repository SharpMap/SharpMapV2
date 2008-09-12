
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
// AGG-Sharp - Version 1
// Copyright (C) 2007 Lars Brubaker http://agg-sharp.sourceforge.net/
//
// Permission to copy, use, modify, sell and distribute this software 
// is granted provided this copyright notice appears in all copies. 
// This software is provided "as is" without express or implied
// warranty, and with no claim as to its suitability for any purpose.
//
//----------------------------------------------------------------------------
// Contact: larsbrubaker@gmail.com
//          http://agg-sharp.sourceforge.net/
//----------------------------------------------------------------------------

namespace AGG.Color
{
    public interface IColorType
    {
        RGBA_Doubles GetAsRGBA_Doubles();
        RGBA_Bytes GetAsRGBA_Bytes();

        RGBA_Bytes Gradient(RGBA_Bytes c, double k);

        //uint R_Byte { get; set; }
        //uint G_Byte { get; set; }
        //uint B_Byte { get; set; }
        //uint A_Byte { get; set; }

        uint R_Byte { get; }
        uint G_Byte { get; }
        uint B_Byte { get; }
        uint A_Byte { get; }
    };
}
