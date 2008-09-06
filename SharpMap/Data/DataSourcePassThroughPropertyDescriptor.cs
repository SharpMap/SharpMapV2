using System;
using System.ComponentModel;
using SharpMap.Layers;

namespace SharpMap.Data
{
    public class DataSourcePassThroughPropertyDescriptor : PropertyDescriptor
    {
        private readonly RuntimeTypeHandle _type;

        public DataSourcePassThroughPropertyDescriptor(String name, Type type)
            : base(name, null)
        {
            _type = type.TypeHandle;
        }

        public override Boolean CanResetValue(Object component)
        {
            return false;
        }

        public override Type ComponentType
        {
            get { return typeof(ILayer); }
        }

        public override Object GetValue(Object component)
        {
            ILayer layer = component as ILayer;

            return layer == null
                       ? null
                       : layer.GetPropertyValue(this);
        }

        public override Boolean IsReadOnly
        {
            get { return true; }
        }

        public override Type PropertyType
        {
            get { return Type.GetTypeFromHandle(_type); }
        }

        public override void ResetValue(Object component)
        {
            throw new NotSupportedException();
        }

        public override void SetValue(Object component, Object value)
        {
            ILayer layer = component as ILayer;

            if (layer == null)
            {
                return;
            }

            layer.SetPropertyValue(this, value);
        }

        public override Boolean ShouldSerializeValue(Object component)
        {
            throw new NotSupportedException();
        }
    }
}
using System;
using System.ComponentModel;
using SharpMap.Layers;

namespace SharpMap.Data
{
    public class DataSourcePassThroughPropertyDescriptor : PropertyDescriptor
    {
        private readonly RuntimeTypeHandle _type;

        public DataSourcePassThroughPropertyDescriptor(String name, Type type)
            : base(name, null)
        {
            _type = type.TypeHandle;
        }

        public override Boolean CanResetValue(Object component)
        {
            return false;
        }

        public override Type ComponentType
        {
            get { return typeof(ILayer); }
        }

        public override Object GetValue(Object component)
        {
            ILayer layer = component as ILayer;

            return layer == null
                       ? null
                       : layer.GetPropertyValue(this);
        }

        public override Boolean IsReadOnly
        {
            get { return true; }
        }

        public override Type PropertyType
        {
            get { return Type.GetTypeFromHandle(_type); }
        }

        public override void ResetValue(Object component)
        {
            throw new NotSupportedException();
        }

        public override void SetValue(Object component, Object value)
        {
            ILayer layer = component as ILayer;

            if (layer == null)
            {
                return;
            }

            layer.SetPropertyValue(this, value);
        }

        public override Boolean ShouldSerializeValue(Object component)
        {
            throw new NotSupportedException();
        }
    }
}
