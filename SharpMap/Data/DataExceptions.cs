using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace SharpMap.Data
{
    public class SharpMapDataException : Exception
    {
        public SharpMapDataException() : base() { }
        public SharpMapDataException(string message) : base(message) { }
        public SharpMapDataException(string message, Exception inner) : base(message, inner) { }
        public SharpMapDataException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    public class LayerNotFoundException : SharpMapDataException
    {
        private string _layerName;
        private string _connectionId;

        public LayerNotFoundException(string layerName, string connectionId) : this(layerName, connectionId, null) { }
        public LayerNotFoundException(string layerName, string connectionId, string message) : this(layerName, connectionId, message, null) { }
        public LayerNotFoundException(string layerName, string connectionId, string message, Exception inner)
            : base(message, inner) 
        {
            _layerName = layerName;
            _connectionId = connectionId;
        }
        
        public LayerNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public string LayerName
        {
            get { return _layerName; }
        }

        public string ConnectionId
        {
            get { return _connectionId; }
        }
    }
}
