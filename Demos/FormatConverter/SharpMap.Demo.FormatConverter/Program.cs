﻿/*
 *	This file is part of SharpMap.Demo.FormatConverter
 *  SharpMap.Demo.FormatConverter is free software © 2008 Newgrove Consultants Limited, 
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
 */
using SharpMap.Utilities;
using SharpMap.Utilities.SridUtility;

namespace SharpMap.Demo.FormatConverter
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            SridMap.DefaultInstance =
                new SridMap(new[] {new SridProj4Strategy(0, new GeometryServices().CoordinateSystemFactory)});

            Converter c = new Converter();
            while (c.Run())
            {
                //do nothing;
            }
        }
    }
}