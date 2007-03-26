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
using System.Runtime.Serialization;

namespace SharpMap.Data
{
    public class SharpMapDataException : Exception
    {
        public SharpMapDataException() : base() { }
        public SharpMapDataException(string message) : base(message) { }
        public SharpMapDataException(string message, Exception inner) : base(message, inner) { }
        public SharpMapDataException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    public class LayerNotFoundException : SharpMapDataException
    {
        private string _layerName;
        private string _connectionId;

        public LayerNotFoundException(string layerName, string connectionId) : this(layerName, connectionId, null) { }
        public LayerNotFoundException(string layerName, string connectionId, string message) : this(layerName, connectionId, message, null) { }
        public LayerNotFoundException(string layerName, string connectionId, string message, Exception inner)
            : base(message, inner) 
        {
            _layerName = layerName;
            _connectionId = connectionId;
        }
        
        public LayerNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public string LayerName
        {
            get { return _layerName; }
        }

        public string ConnectionId
        {
            get { return _connectionId; }
        }
    }
}
