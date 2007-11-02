using System;
using System.Runtime.Serialization;

namespace ProjNet.CoordinateSystems
{
    /// <summary>
    /// Exception thrown when a computation doesn't converge during iteration.
    /// </summary>
    [Serializable]
    public class ComputationConvergenceException : Exception
    {
        /// <summary>
        /// Creates a new instance of a <see cref="ComputationConvergenceException"/>.
        /// </summary>
        public ComputationConvergenceException() { }

        /// <summary>
        /// Creates a new <see cref="ComputationConvergenceException"/> instance with the given 
        /// <paramref name="message"/>.
        /// </summary>
        /// <param name="message">Information about the exception.</param>
        public ComputationConvergenceException(string message) : base(message) { }

        /// <summary>
        /// Creates a new <see cref="ComputationConvergenceException"/> instance with the given 
        /// <paramref name="message"/> and causal <paramref name="inner"/> <see cref="Exception"/>.
        /// </summary>
        /// <param name="message">Information about the exception.</param>
        /// <param name="inner">The <see cref="Exception"/> which caused this exception.</param>
        public ComputationConvergenceException(string message, Exception inner) : base(message, inner) { }

        protected ComputationConvergenceException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
