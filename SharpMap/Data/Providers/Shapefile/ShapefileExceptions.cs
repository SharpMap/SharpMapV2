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
    public class InvalidShapeFileException : ShapeFileException
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
    public class InvalidShapeFileOperationException : ShapeFileException
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
    public class UnsupportedShapeFileGeometryException : ShapeFileException
    {
        public UnsupportedShapeFileGeometryException() : base() { }
        public UnsupportedShapeFileGeometryException(string message) : base(message) { }
        public UnsupportedShapeFileGeometryException(string message, Exception inner) : base(message, inner) { }
        public UnsupportedShapeFileGeometryException(SerializationInfo info, StreamingContext context)
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
