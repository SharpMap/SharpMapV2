// Copyright 2007, 2008 - Rory Plaire (codekaizen@gmail.com)
//
// This file is part of Proj.Net.
// Proj.Net is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// Proj.Net is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with Proj.Net; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using System;
using System.Runtime.Serialization;

namespace ProjNet.CoordinateSystems
{
    /// <summary>
    /// Exception thrown when a computation doesn't converge during iteration.
    /// </summary>
    [Serializable]
    public class ComputationConvergenceException : ComputationException
    {
        /// <summary>
        /// Creates a new instance of a <see cref="ComputationConvergenceException"/>.
        /// </summary>
        public ComputationConvergenceException() {}

        /// <summary>
        /// Creates a new <see cref="ComputationConvergenceException"/> instance with the given 
        /// <paramref name="message"/>.
        /// </summary>
        /// <param name="message">Information about the exception.</param>
        public ComputationConvergenceException(String message) : base(message) {}

        /// <summary>
        /// Creates a new <see cref="ComputationConvergenceException"/> instance with the given 
        /// <paramref name="message"/> and causal <paramref name="inner"/> <see cref="Exception"/>.
        /// </summary>
        /// <param name="message">Information about the exception.</param>
        /// <param name="inner">The <see cref="Exception"/> which caused this exception.</param>
        public ComputationConvergenceException(String message, Exception inner) : base(message, inner) {}

        protected ComputationConvergenceException(SerializationInfo info, StreamingContext context)
            : base(info, context) {}
    }
}