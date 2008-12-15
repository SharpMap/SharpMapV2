using System;
using System.Runtime.Serialization;

namespace SharpMap.Expressions
{
    [Serializable]
    public class ObjectReadonlyException : Exception
    {
        public ObjectReadonlyException() {}
        public ObjectReadonlyException(string message) : base(message) {}
        public ObjectReadonlyException(string message, Exception inner) 
            : base(message, inner) {}
        protected ObjectReadonlyException(SerializationInfo info, StreamingContext context) 
            : base(info, context) {}
    }
}
