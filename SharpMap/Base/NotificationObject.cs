using System;
using System.ComponentModel;

namespace SharpMap.Base
{
    /// <summary>
    /// SharpMap's implementation of an object implementing <see cref="INotifyPropertyChanged"/> and/or <see cref="INotifyPropertyChanging"/>
    /// </summary>
    [Serializable]
    public abstract class NotificationObject : INotifyPropertyChanged, INotifyPropertyChanging
    {
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
    }
}