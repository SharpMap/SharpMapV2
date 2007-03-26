// Copyright 2005, 2006 - Morten Nielsen (www.iter.dk)
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
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SharpMap.Utilities
{
	/// <summary>
	/// Provider helper utilities
	/// </summary>
	public static class ProvidersEnumerator
	{
		/// <summary>
		/// Returns a list of available data providers in this assembly
		/// </summary>
		public static List<Type> GetProviders(Assembly assembly)
		{
			List<Type> providerList = new List<Type>();

            foreach (Type t in assembly.GetTypes())
			{
                Type[] interfaces = t.FindInterfaces(delegate(Type type, object criteria)
                {
                    Type providerInterfaceType = criteria as Type;
                    return (type == providerInterfaceType);

                }, typeof(SharpMap.Data.Providers.IProvider));
				
				if (interfaces.Length > 0)
					providerList.Add(t);
			}

			return providerList;
		}
	}
}
