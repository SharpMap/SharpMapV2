using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace SharpMap
{
    [Serializable]
    public class DuplicateLayerException : InvalidOperationException
    {
        private readonly string _duplicateLayerName;

        public DuplicateLayerException(string duplicateLayerName)
            : this(duplicateLayerName, null) { }

        public DuplicateLayerException(string duplicateLayerName, string message)
            : this(duplicateLayerName, message, null) { }

        public DuplicateLayerException(string duplicateLayerName, string message, Exception inner) 
            : base(message, inner) 
        {
            _duplicateLayerName = duplicateLayerName;
        }
        
        protected DuplicateLayerException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        public string DuplicateLayerName
        {
            get { return _duplicateLayerName; }
        }
    }
}
