// Copyright 2005, 2006 - Christian Gräfe (www.sharptools.de)
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

namespace SharpMap.Extensions.Layers.GdalRasterLayer
{
	/// <summary>
	/// Field data type
	/// </summary>
	public enum DataType
	{
		/// <summary>
		/// unknown data type
		/// </summary>
		Unknown = 0,
		/// <summary>
		/// byte
		/// </summary>
		Byte = 1,
		/// <summary>
		/// unsigned int 16 bit
		/// </summary>
		UInt16 = 2,
		/// <summary>
		/// signed int 16 bit
		/// </summary>
		Int16 = 3,
		/// <summary>
		/// unsigned int 32 bit
		/// </summary>
		UInt32 = 4,
		/// <summary>
		/// signed int 32 bit
		/// </summary>
		Int32 = 5,
		/// <summary>
		/// float 32 bit
		/// </summary>
		Float32 = 6,
		/// <summary>
		/// float 64 bit
		/// </summary>
		Float64 = 7,
		/// <summary>
		/// c int 16 bit
		/// </summary>
		CInt16 = 8,
		/// <summary>
		/// c int 32 bit
		/// </summary>
		CInt32 = 9,
		/// <summary>
		/// c float 16 bit
		/// </summary>
		CFloat32 = 10,
		/// <summary>
		/// c float 64 bit
		/// </summary>
		CFloat64 = 11,
		/// <summary>
		/// type count
		/// </summary>
		TypeCount = 12
	};
}
