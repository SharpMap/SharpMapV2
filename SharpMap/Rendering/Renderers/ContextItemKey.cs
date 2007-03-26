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
using System.Collections.Generic;
using System.Text;

namespace SharpMap.Rendering
{
    public struct ContextItemKey : IEquatable<ContextItemKey>
    {
        private RuntimeTypeHandle _typeHandle;
        private string _id;

        public ContextItemKey(Type type)
            : this(type, null) { }

        public ContextItemKey(Type type, string id)
        {
            _typeHandle = type.TypeHandle;
            _id = id;
        }

        public string Id
        {
            get { return _id; }
        }

        public RuntimeTypeHandle TypeHandle
        {
            get { return _typeHandle; }
        }

        #region IEquatable<ContextItemKey> Members

        public bool Equals(ContextItemKey other)
        {
            return TypeHandle.Equals(other.TypeHandle) && Id == other.Id;
        }

        #endregion

        public override bool Equals(object obj)
        {
            if (!(obj is ContextItemKey))
                return false;

            return this.Equals((ContextItemKey)obj);
        }

        public override int GetHashCode()
        {
            return unchecked(TypeHandle.GetHashCode() ^ Id.GetHashCode());
        }

        public override string ToString()
        {
            return String.Format("ContextItemKey - Type: {0}; Id: {1}", Type.GetTypeFromHandle(TypeHandle), Id ?? "<NULL>");
        }
    }
}
