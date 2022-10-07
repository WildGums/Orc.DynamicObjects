﻿namespace Orc.DynamicObjects
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq.Expressions;
    using Catel;
    using Catel.Data;

    /// <summary>
    /// Dynamic observable object implementing the <see cref="IDynamicMetaObjectProvider"/>.
    /// </summary>
    public class DynamicObservableObject : ObservableObject, IDynamicMetaObjectProvider
    {
        /// <summary>
        /// Dynamic properties.
        /// </summary>
        private readonly Dictionary<string, object?> _dynamicProperties = new Dictionary<string, object?>();

        /// <summary>
        /// Get dynamic property value.
        /// </summary>
        /// <typeparam name="T">Type of property value.</typeparam>
        /// <param name="propertyName">Property name.</param>
        /// <returns>Property value.</returns>
        public T? GetValue<T>(string propertyName)
        {
            Argument.IsNotNullOrWhitespace(() => propertyName);

            _dynamicProperties.TryGetValue(propertyName, out var value);

            if (value is null)
            {
                return default;
            }

            return (T)value;
        }

        /// <summary>
        /// Set dynamic property value.
        /// </summary>
        /// <param name="propertyName">Property name.</param>
        /// <param name="value">Property value.</param>
        public void SetValue(string propertyName, object value)
        {
            Argument.IsNotNullOrWhitespace(() => propertyName);

            _dynamicProperties[propertyName] = value;

            RaisePropertyChanged(propertyName);
        }

        /// <summary>
        /// Returns the <see cref="T:System.Dynamic.DynamicMetaObject" /> responsible for binding operations performed on this object.
        /// </summary>
        /// <param name="parameter">The expression tree representation of the runtime value.</param>
        /// <returns>The <see cref="T:System.Dynamic.DynamicMetaObject" /> to bind this object.</returns>
        public DynamicMetaObject GetMetaObject(Expression parameter)
        {
            ArgumentNullException.ThrowIfNull(parameter);

            return new DynamicObservableObjectMetaObject(parameter, this);
        }

        /// <summary>
        /// Returns the enumeration of all property names.
        /// </summary>
        /// <returns>The list of property names.</returns>
        protected internal IEnumerable<string> GetPropertyNames()
        {
            return new List<string>(_dynamicProperties.Keys);
        }
    }
}
