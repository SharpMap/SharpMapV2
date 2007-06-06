using System;
using System.Collections.Generic;
using System.Text;

using SharpMap.Geometries;

namespace SharpMap.Indexing.RTree
{
    /// <summary>
    /// An entry in an <see cref="RTree"/>.
    /// </summary>
    /// <typeparam name="TValue">The type of the value used in the entries.</typeparam>
    public struct RTreeIndexEntry<TValue>
        where TValue : IEquatable<TValue>
    {
        public RTreeIndexEntry(TValue value, BoundingBox box)
        {
            _value = value;
            _boundingBox = box;
        }

        /// <summary>
        /// Indexed value.
        /// </summary>
        private TValue _value;
        public TValue Value
        {
            get { return _value; }
            internal set { _value = value; }
        }

        /// <summary>
        /// <see cref="BoundingBox"/> containing the value.
        /// </summary>
        private BoundingBox _boundingBox;
        public BoundingBox BoundingBox
        {
            get { return _boundingBox; }
            internal set { _boundingBox = value; }
        }

        public override string ToString()
        {
            return String.Format("{0}; Value: {1}; BoundingBox: {2}", GetType(), Value, BoundingBox);
        }
    }
}
