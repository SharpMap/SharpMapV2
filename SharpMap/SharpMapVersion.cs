// Portions copyright 2005 - 2006: Morten Nielsen (www.iter.dk)
// Portions copyright 2006 - 2008: Rory Plaire (codekaizen@gmail.com)
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

using System;
using System.Reflection;

namespace SharpMap
{
	/// <summary>
	/// Version information helper class.
	/// </summary>
	public static class SharpMapVersion
	{
        public static readonly Version _version = Assembly.GetExecutingAssembly().GetName().Version;

		/// <summary>
		/// Returns the current build version of SharpMap.
		/// </summary>
		public static Version CurrentVersion
		{
            get { return _version; }
		}
	}
}
