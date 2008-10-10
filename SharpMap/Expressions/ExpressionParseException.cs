using System;
using System.Runtime.Serialization;
using GeoAPI;

namespace SharpMap.Expressions
{
    [Serializable]
    public class ExpressionParseException : ParseException
    {
        public ExpressionParseException() { }
        public ExpressionParseException(String message) : base(message) { }
        public ExpressionParseException(String message, Exception inner) : base(message, inner) { }
        protected ExpressionParseException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
