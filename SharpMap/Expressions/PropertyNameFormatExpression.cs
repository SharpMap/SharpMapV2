using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using GeoAPI.DataStructures;

namespace SharpMap.Expressions
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class PropertyNameFormatExpression : CollectionExpression<PropertyNameExpression>
    {

        /// <summary>
        /// Creates an instance of this class
        /// </summary>
        /// <param name="format">The format string to format the provided property value</param>
        /// <param name="propertyName">The name of the property to decorate via the format string</param>
        public PropertyNameFormatExpression(string format, PropertyNameExpression propertyName)
            : this(format, new [] {propertyName})
        {
        }

        /// <summary>
        /// Creates an instance of this class
        /// </summary>
        /// <param name="format">The format string to format the provided property values</param>
        /// <param name="collection">The names of the properties to decorate within the provided format string</param>
        public PropertyNameFormatExpression(string format, IEnumerable<PropertyNameExpression> collection)
            : base(collection)
        {
            Format = format;
        }



        /// <summary>
        /// Creates an instance of this class
        /// </summary>
        /// <param name="format">The format string to format the provided property values</param>
        /// <param name="collection">The names of the properties to decorate within the provided format string</param>
        /// <param name="comparer">An equality comparer for the <see cref name="collection"/>.</param>
        public PropertyNameFormatExpression(string format, IEnumerable<PropertyNameExpression> collection, IEqualityComparer<PropertyNameExpression> comparer)
            : base(collection, comparer)
        {
            Format = format;
        }

        /// <summary>
        /// Creates an instance of this class
        /// </summary>
        protected PropertyNameFormatExpression(SerializationInfo info, StreamingContext context)
            :base(info, context)
        {
            Format = info.GetString("format");
        }


        /// <summary>
        /// The Format string 
        /// </summary>
        public string Format { get; set; }

        /// <summary>
        /// The provided property names as array for usage in <c>string.Format(..., ...)</c> function
        /// </summary>
        public PropertyNameExpression[] PropertyNames
        {
            get { return Enumerable.ToArray(Collection); }
        }

        public override bool Equals(Expression other)
        {
            if (other == null || !(other is PropertyNameFormatExpression))
                return false;

            var pnf = other as PropertyNameFormatExpression;
            if (Format != pnf.Format)
                return false;

            return base.Equals(pnf);
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("format", Format);
        }
    }
}