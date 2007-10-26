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
using System.Runtime.Serialization;

namespace SharpMap.Data
{
    /// <summary>
    /// The base class for data exceptions in SharpMap.
    /// </summary>
    [Serializable]
    public class SharpMapDataException : Exception
    {
        /// <summary>
        /// Creates a new instance of a SharpMapDataException.
        /// </summary>
        public SharpMapDataException()
        {
        }

        /// <summary>
        /// Creates a new instance of a SharpMapDataException with the given 
        /// <paramref name="message"/>.
        /// </summary>
        /// <param name="message">Text message to include in the exception.</param>
        public SharpMapDataException(String message)
            : base(message)
        {
        }

        /// <summary>
        /// Creates a new instance of a SharpMapDataException with the given 
        /// <paramref name="message"/> and causing exception, <paramref name="inner"/>.
        /// </summary>
        /// <param name="message">Text message to include in the exception.</param>
        /// <param name="inner">Exception which caused this exception.</param>
        public SharpMapDataException(String message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Creates a new instance of a SharpMapDataException with the given
        /// <see cref="info">serialization data</see>.
        /// </summary>
        /// <param name="info">Exception values which were serialized.</param>
        /// <param name="context">Serialization context information.</param>
        protected SharpMapDataException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}