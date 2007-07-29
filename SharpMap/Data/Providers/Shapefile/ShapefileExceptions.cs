using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace SharpMap.Data.Providers
{
    /// <summary>
    /// Exception thrown during shapefile operations
    /// </summary>
    public class ShapeFileException : SharpMapDataException
    {
        public ShapeFileException() : base() { }
        public ShapeFileException(string message) : base(message) { }
        public ShapeFileException(string message, Exception inner) : base(message, inner) { }
        public ShapeFileException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }

    /// <summary>
    /// Exception thrown when the shapefile is invalid or corrupt.
    /// </summary>
    public class ShapeFileIsInvalidException : ShapeFileException
    {
        public ShapeFileIsInvalidException() : base() { }
        public ShapeFileIsInvalidException(string message) : base(message) { }
        public ShapeFileIsInvalidException(string message, Exception inner) : base(message, inner) { }
        public ShapeFileIsInvalidException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }

    /// <summary>
    /// Exception thrown when an operation is attempted which is not defined for the state of the <see cref="ShapeFile"/>
    /// </summary>
    public class ShapeFileInvalidOperationException : ShapeFileException
    {
        public ShapeFileInvalidOperationException() : base() { }
        public ShapeFileInvalidOperationException(string message) : base(message) { }
        public ShapeFileInvalidOperationException(string message, Exception inner) : base(message, inner) { }
        public ShapeFileInvalidOperationException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }

    /// <summary>
    /// Exception thrown when a geometry type exists in a shapefile which is not currently supported.
    /// </summary>
    public class ShapeFileUnsupportedGeometryException : ShapeFileException
    {
        public ShapeFileUnsupportedGeometryException() : base() { }
        public ShapeFileUnsupportedGeometryException(string message) : base(message) { }
        public ShapeFileUnsupportedGeometryException(string message, Exception inner) : base(message, inner) { }
        public ShapeFileUnsupportedGeometryException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }

    /// <summary>
    /// Exception thrown when a <see cref="FeatureDataTable"/> schema doesn't match the
    /// DBase file schema
    /// </summary>
    public class DbfSchemaMismatchException : ShapeFileException
    {
        public DbfSchemaMismatchException() : base() { }
        public DbfSchemaMismatchException(string message) : base(message) { }
        public DbfSchemaMismatchException(string message, Exception inner) : base(message, inner) { }
        public DbfSchemaMismatchException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
