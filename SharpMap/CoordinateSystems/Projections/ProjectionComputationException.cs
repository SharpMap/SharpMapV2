using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace SharpMap.CoordinateSystems.Projections
{
    [Serializable]
    public class ProjectionComputationException : Exception
    {
        public ProjectionComputationException() : base() { }
        public ProjectionComputationException(string message) : base(message) { }
        public ProjectionComputationException(string message, Exception inner) : base(message, inner) { }
        public ProjectionComputationException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
