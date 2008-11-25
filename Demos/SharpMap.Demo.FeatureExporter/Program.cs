/*
 *	This file is part of SharpMap.Demo.FeatureExporter
 *  SharpMap.Demo.FeatureExporter is free software © 2008 Newgrove Consultants Limited, 
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
using System;

namespace SharpMap.Demo.FeatureExporter
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("\nApplication demonstrating exporting features \nfrom a shapefile into new shapefiles using SharpMap v2\nCopyright Newgrove Consultants Ltd 2008");

            while (ShapeExporter.Run())
            {
            }
        }
    }
}