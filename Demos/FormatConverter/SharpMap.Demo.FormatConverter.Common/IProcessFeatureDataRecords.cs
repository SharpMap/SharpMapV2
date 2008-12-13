/*
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
using System.Collections.Generic;
using SharpMap.Data;

namespace SharpMap.Demo.FormatConverter.Common
{
    /// <summary>
    /// Used to alter or filter an input enumerable of IFeatureDataRecord s
    /// </summary>
    public interface IProcessFeatureDataRecords
    {
        /// <summary>
        /// Configure the processor in here. This may involve using the console to take in parameters
        /// </summary>
        void Configure();


        /// <summary>
        /// Processes each item in the input enumerable and returns an enumerable of the processed or filtered items
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        IEnumerable<IFeatureDataRecord> Process(IEnumerable<IFeatureDataRecord> input);
    }
}