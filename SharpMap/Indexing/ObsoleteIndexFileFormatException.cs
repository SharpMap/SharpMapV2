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
using System.Runtime.Serialization;
using System.Text;

namespace SharpMap.Indexing
{
    /// <summary>
    /// Exception thrown when loading spatial index file fails due to imcompatible version
    /// </summary>
    public class ObsoleteIndexFileFormatException : Exception
    {
        private readonly System.Version _versionExpected;
        private readonly System.Version _versionEncountered;

        public ObsoleteIndexFileFormatException(System.Version versionExpected, System.Version versionEncountered)
            : this(versionExpected, versionEncountered, null) { }

        public ObsoleteIndexFileFormatException(System.Version versionExpected, System.Version versionEncountered, String message)
            : base(message)
        {
            _versionExpected = versionExpected;
            _versionEncountered = versionEncountered;
        }

        /// <summary>
        /// Exception thrown when layer rendering has failed. This constructor is used by the runtime during serialization.
        /// </summary>
        /// <param name="info">Data used to reconstruct instance</param>
        /// <param name="context">Serialization context</param>
        public ObsoleteIndexFileFormatException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        /// <summary>
        /// The version of the file expected
        /// </summary>
        public System.Version VersionExpected
        {
            get { return _versionExpected; }
        }

        /// <summary>
        /// The version of the file actually encountered
        /// </summary>
        public System.Version VersionEncountered
        {
            get { return _versionEncountered; }
        }

        public override String ToString()
        {
            return String.Format("Spatial index file version was not compatible with the version expected. Expected: {0}; Encountered: {1}.", VersionExpected, VersionEncountered);
        }
    }
}
