using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace SharpMap.Data.Providers
{
    /// <summary>
    /// Exception thrown during shapefile operations
    /// </summary>
    public class ShapefileException : SharpMapDataException
    {
        public ShapefileException() : base() { }
        public ShapefileException(string message) : base(message) { }
        public ShapefileException(string message, Exception inner) : base(message, inner) { }
        public ShapefileException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }

    /// <summary>
    /// Exception thrown when the shapefile is invalid or corrupt.
    /// </summary>
    public class InvalidShapeFileException : ShapefileException
    {
        public InvalidShapeFileException() : base() { }
        public InvalidShapeFileException(string message) : base(message) { }
        public InvalidShapeFileException(string message, Exception inner) : base(message, inner) { }
        public InvalidShapeFileException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }

    /// <summary>
    /// Exception thrown when an operation is attempted which is not defined for the state of the <see cref="ShapeFile"/>
    /// </summary>
    public class InvalidShapeFileOperationException : ShapefileException
    {
        public InvalidShapeFileOperationException() : base() { }
        public InvalidShapeFileOperationException(string message) : base(message) { }
        public InvalidShapeFileOperationException(string message, Exception inner) : base(message, inner) { }
        public InvalidShapeFileOperationException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }

    /// <summary>
    /// Exception thrown when a geometry type exists in a shapefile which is not currently supported.
    /// </summary>
    public class UnsupportedShapefileGeometryException : ShapefileException
    {
        public UnsupportedShapefileGeometryException() : base() { }
        public UnsupportedShapefileGeometryException(string message) : base(message) { }
        public UnsupportedShapefileGeometryException(string message, Exception inner) : base(message, inner) { }
        public UnsupportedShapefileGeometryException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
