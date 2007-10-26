// Copyright 2006, 2007 - Rory Plaire (codekaizen@gmail.com)
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

namespace SharpMap.Data
{
    internal static class AdoNetInternalTypes
    {
        private static readonly String _adoAssemblyName =
            "System.Data, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
        internal static readonly Type DataKeyType = Type.GetType("System.Data.DataKey, " + _adoAssemblyName);
        internal static readonly Type IndexType = Type.GetType("System.Data.Index, " + _adoAssemblyName);
        internal static readonly Type MergerType = Type.GetType("System.Data.Merger, " + _adoAssemblyName);
    }
}
