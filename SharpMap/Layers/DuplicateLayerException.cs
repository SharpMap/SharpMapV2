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

using System;
using System.Runtime.Serialization;
using SharpMap;

namespace SharpMap.Layers
{
    /// <summary>
    /// Exception thrown when a layer with a name which is the same
    /// as an existing layer is added to a <see cref="Map.LayerCollection"/>.
    /// </summary>
    [Serializable]
    public class DuplicateLayerException : InvalidOperationException
    {
        private readonly String _duplicateLayerName;

        /// <summary>
        /// Creates a new instance of DuplicateLayerException, indicating
        /// the duplicate layer name by <paramref name="duplicateLayerName"/>.
        /// </summary>
        /// <param name="duplicateLayerName">
        /// The existing layer name which was duplicated.
        /// </param>
        public DuplicateLayerException(String duplicateLayerName)
            : this(duplicateLayerName, null) { }

        /// <summary>
        /// Creates a new instance of DuplicateLayerException, indicating
        /// the duplicate layer name by <paramref name="duplicateLayerName"/>
        /// and including a message.
        /// </summary>
        /// <param name="duplicateLayerName">
        /// The existing layer name which was duplicated.
        /// </param>
        /// <param name="message">Additional information about the exception.</param>
        public DuplicateLayerException(String duplicateLayerName, String message)
            : this(duplicateLayerName, message, null) { }

        /// <summary>
        /// Creates a new instance of DuplicateLayerException, indicating
        /// the duplicate layer name by <paramref name="duplicateLayerName"/>
        /// and including a message.
        /// </summary>
        /// <param name="duplicateLayerName">
        /// The existing layer name which was duplicated.
        /// </param>
        /// <param name="message">
        /// Additional information about the exception.
        /// </param>
        /// <param name="inner">
        /// An exception which caused this exception, if any.
        /// </param>
        public DuplicateLayerException(String duplicateLayerName, String message, Exception inner)
            : base(message, inner)
        {
            _duplicateLayerName = duplicateLayerName;
        }

        /// <summary>
        /// Creates a new instance of DuplicateLayerException from serialized data,
        /// <paramref name="info"/>.
        /// </summary>
        /// <param name="info">The serialization data.</param>
        /// <param name="context">Serialzation context.</param>
        protected DuplicateLayerException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _duplicateLayerName = info.GetString("_duplicateLayerName");
        }

        /// <summary>
        /// Gets the existing layer name which was duplicated.
        /// </summary>
        public String DuplicateLayerName
        {
            get { return _duplicateLayerName; }
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("_duplicateLayerName", _duplicateLayerName);
        }
    }
}
