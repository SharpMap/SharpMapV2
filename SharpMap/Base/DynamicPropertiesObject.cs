using System;
using System.Collections.Generic;
using System.ComponentModel;
using SharpMap.Expressions;

namespace SharpMap.Base
{
    /// <summary>
    /// Base implementation for an object that has dynamic properties
    /// </summary>
    public abstract class DynamicPropertiesObject : CustomTypeDescriptor, IHasDynamicProperties, INotifyPropertyChanging
    {
        protected static readonly PropertyDescriptorCollection PropertyDescriptors;

        static DynamicPropertiesObject()
        {
            //var propertyCollection = TypeDescriptor.GetProperties(typeof(DynamicPropertiesObject));
            //var properties = new PropertyDescriptor[propertyCollection.Count];
            //propertyCollection.CopyTo(properties, 0);

            //StaticProperties = new PropertyDescriptorCollection(properties, false);
            PropertyDescriptors = new PropertyDescriptorCollection(new PropertyDescriptor[0]);
        }

        private Dictionary<PropertyDescriptor, Object> _propertyValues;
        private PropertyDescriptorCollection _instanceProperties;

        protected DynamicPropertiesObject()
        {
        }

        protected DynamicPropertiesObject(ICustomTypeDescriptor desciptor)
            : base(desciptor)
        {
        }

        ///<summary>
        /// Event indicating that a property has hanged
        ///</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Event indicating that a property is about to change
        /// </summary>
        public event PropertyChangingEventHandler PropertyChanging;

        protected void OnPropertyChanging(string propertyName)
        {
            if (PropertyChanging != null)
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
        }

        protected void OnPropertyChanging(PropertyDescriptor property)
        {
            if (property == null)
                throw new ArgumentNullException("property");

            OnPropertyChanging(property.Name);
        }

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void OnPropertyChanged(PropertyDescriptor property)
        {
            if (property == null)
                throw new ArgumentNullException("property");

            OnPropertyChanged(property.Name);
        }

        #region Implementation of IHasDynamicProperties

        public Int32 AddProperty(PropertyDescriptor property)
        {
            EnsureInstanceProperties();
            return _instanceProperties.Add(property);
        }

        public Int32 AddProperty<TValue>(PropertyDescriptor property, TValue value)
        {
            EnsureInstanceProperties();
            Int32 index = _instanceProperties.Add(property);
            SetPropertyValueInternal(property, value);
            return index;
        }

        public TValue GetPropertyValue<TValue>(PropertyDescriptor property)
        {
            if (property == null) { throw new ArgumentNullException("property"); }

            return GetPropertyValueInternal<TValue>(property);
        }

        public Object GetPropertyValue(PropertyDescriptor property)
        {
            if (property == null) { throw new ArgumentNullException("property"); }

            return GetPropertyValueInternal(property);
        }

        public void SetPropertyValue<TValue>(PropertyDescriptor property, TValue value)
        {
            CheckSetValueType<TValue>(property);
            CheckPropertyParameter(property);
            SetPropertyValueInternal(property, value);
        }

        public void SetPropertyValue(PropertyDescriptor property, Object value)
        {
            CheckPropertyParameter(property);
            SetPropertyValueInternal(property, value);
        }

        public virtual Boolean HasProperty(PropertyDescriptor property)
        {
            PropertyDescriptorCollection properties = _instanceProperties ?? GetClassProperties();
            return properties.Contains(property) || (_propertyValues != null &&
                                                     _propertyValues.ContainsKey(property));
        }

        protected virtual void SetObjectProperty(string propertyName, object value)
        {
            var pd = PropertyDescriptors.Find(propertyName, false);
            if (pd != null)
                SetPropertyValue(pd, value);
            throw new ArgumentException(string.Format("Property '{0}' is not a member of '{1}'", propertyName, GetType()));
        }

        protected virtual object GetObjectProperty(string propertyName)
        {
            var pd = PropertyDescriptors.Find(propertyName, false);
            if (pd != null)
                return GetPropertyValue(pd);
            throw new ArgumentException(string.Format("Property '{0}' is not a member of '{1}'", propertyName, GetType()));
        }

        #endregion Implementation of IHasDynamicProperties

        protected virtual PropertyDescriptorCollection GetClassProperties()
        {
            return PropertyDescriptors;
        }

        protected virtual TValue GetPropertyValueInternal<TValue>(PropertyDescriptor property)
        {
            CheckGetValueType<TValue>(property);

            return (TValue)GetPropertyValueInternal(property);
        }

        protected virtual Object GetPropertyValueInternal(PropertyDescriptor property)
        {
            var classProperties = GetClassProperties();

            if (classProperties.Contains(property))
            {
                String propertyName = property.Name;

                GetObjectProperty(propertyName);
            }

            if (_instanceProperties.Contains(property))
            {
                Object value;

                return _propertyValues != null && _propertyValues.TryGetValue(property, out value)
                           ? value
                           : null;
            }

            throw new InvalidOperationException("Property doesn't exist in this instance: " +
                                                property.Name);
        }

        protected virtual void SetPropertyValueInternal<TValue>(PropertyDescriptor property, TValue value)
        {
            CheckSetValueType<TValue>(property);

            SetPropertyValueInternal(property, (Object)value);
        }

        protected virtual void SetPropertyValueInternal(PropertyDescriptor property, Object value)
        {
            PropertyDescriptorCollection classProperties = GetClassProperties();

            if (classProperties.Contains(property))
            {
                String propertyName = property.Name;

                SetObjectProperty(propertyName, value);
            }
            else if (_instanceProperties.Contains(property))
            {
                if (_propertyValues == null)
                {
                    _propertyValues = new Dictionary<PropertyDescriptor, Object>();
                }

                _propertyValues[property] = value;
            }
            else
            {
                throw new InvalidOperationException("Property doesn't exist on this layer: " +
                                                    property.Name);
            }
        }

        protected static void AddDerivedProperties(Type derivedType)
        {
            if (!typeof(DynamicPropertiesObject).IsAssignableFrom(derivedType)) return;

            var properties = TypeDescriptor.GetProperties(derivedType);
            foreach (PropertyDescriptor property in properties)
            {
                if (!PropertyDescriptors.Contains(property))
                {
                    PropertyDescriptors.Add(property);
                }
            }
        }

        #region Private helper methods

        private static void CheckGetValueType<TValue>(PropertyDescriptor property)
        {
            if (!typeof(TValue).IsAssignableFrom(property.PropertyType))
            {
                throw new ArgumentException("The type of the property isn't " +
                                            "assignable to the value variable.");
            }
        }

        private static void CheckSetValueType<TValue>(PropertyDescriptor property)
        {
            if (!property.PropertyType.IsAssignableFrom(typeof(TValue)))
            {
                throw new ArgumentException("The type of the value isn't " +
                                            "assignable to the property.");
            }
        }

        private void EnsureInstanceProperties()
        {
            if (_instanceProperties != null)
            {
                return;
            }

            var classProperties = GetClassProperties();
            var propArray = new PropertyDescriptor[classProperties.Count];
            classProperties.CopyTo(propArray, 0);
            _instanceProperties = new PropertyDescriptorCollection(propArray, false);
        }

        private void CheckPropertyParameter(PropertyDescriptor property)
        {
            if (property == null) { throw new ArgumentNullException("property"); }

            if (!HasProperty(property))
            {
                throw new InvalidOperationException("Property doesn't exist for instance " +
                                                    ToString());
            }

            if (property.IsReadOnly)
            {
                throw new InvalidOperationException(String.Format("Property {0} is read only.",
                                                                  property.Name));
            }
        }

        #endregion Private helper methods
    }
}