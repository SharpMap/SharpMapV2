using System;
using System.Collections.Generic;
using System.Text;

namespace SharpMap.Rendering
{
    public struct ContextItemKey : IEquatable<ContextItemKey>
    {
        private RuntimeTypeHandle _typeHandle;
        private string _id;

        public ContextItemKey(Type type)
            : this(type, null) { }

        public ContextItemKey(Type type, string id)
        {
            _typeHandle = type.TypeHandle;
            _id = id;
        }

        public string Id
        {
            get { return _id; }
        }

        public RuntimeTypeHandle TypeHandle
        {
            get { return _typeHandle; }
        }

        #region IEquatable<ContextItemKey> Members

        public bool Equals(ContextItemKey other)
        {
            return TypeHandle.Equals(other.TypeHandle) && Id == other.Id;
        }

        #endregion

        public override bool Equals(object obj)
        {
            if (!(obj is ContextItemKey))
                return false;

            return this.Equals((ContextItemKey)obj);
        }

        public override int GetHashCode()
        {
            return unchecked(TypeHandle.GetHashCode() ^ Id.GetHashCode());
        }

        public override string ToString()
        {
            return String.Format("ContextItemKey - Type: {0}; Id: {1}", Type.GetTypeFromHandle(TypeHandle), Id ?? "<NULL>");
        }
    }
}
