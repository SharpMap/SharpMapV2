// Copyright 2006 - 2008: Rory Plaire (codekaizen@gmail.com)
//
// This file is part of SharpMap.
// SharpMap is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// SharpMap is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with SharpMap; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using System.IO;
using SharpMap.Expressions;

namespace SharpMap.Data
{
    /// <summary>
    /// Defines the interface to a provider of raster data.
    /// </summary>
    public interface ICoverageProvider : IProvider
    {
        /// <summary>
        /// Retrieves a <see cref="Stream"/> for the raster data that 
        /// are selected by <paramref name="query"/>.
        /// </summary>
        /// <param name="query">Query select with.</param>
        /// <returns>A Stream to access the raster data of the result.</returns>
        Stream ExecuteCoverageQuery(CoverageQueryExpression query);
    }
}
