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

        public ObsoleteIndexFileFormatException(System.Version versionExpected, System.Version versionEncountered, string message)
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

        public override string ToString()
        {
            return String.Format("Spatial index file version was not compatible with the version expected. Expected: {0}; Encountered: {1}.", VersionExpected, VersionEncountered);
        }
    }
}
