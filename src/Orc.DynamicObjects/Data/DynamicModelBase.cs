namespace Orc.DynamicObjects
{
    using System;
    using System.Dynamic;
    using System.Linq.Expressions;
    using Catel.Data;
    using Catel.Logging;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Dynamic model base implementing the <see cref="IDynamicMetaObjectProvider"/>.
    /// </summary>
    [Serializable]
    public class DynamicModelBase : ModelBase, IDynamicMetaObjectProvider
    {
        private static readonly ILogger Logger = LogManager.GetLogger(typeof(DynamicModelBase));

        /// <summary>
        /// Registers a simple property, which means only the name and type are required.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        /// <returns>The <see cref="PropertyData"/>.</returns>
        internal protected void RegisterDynamicProperty(string name, Type type)
        {
            ArgumentNullException.ThrowIfNull(name);
            ArgumentNullException.ThrowIfNull(type);

            var modelType = GetType();

            if (IsPropertyRegistered(modelType, name))
            {
                return;
            }

            Logger.LogDebug("Registering dynamic property '{0}.{1}'", modelType.FullName, name);

            var propertyData = RegisterPropertyNonGeneric(name, type);

            InitializePropertyAfterConstruction(propertyData);
        }

        /// <summary>
        /// Returns the <see cref="T:System.Dynamic.DynamicMetaObject" /> responsible for binding operations performed on this object.
        /// </summary>
        /// <param name="parameter">The expression tree representation of the runtime value.</param>
        /// <returns>The <see cref="T:System.Dynamic.DynamicMetaObject" /> to bind this object.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public DynamicMetaObject GetMetaObject(Expression parameter)
        {
            ArgumentNullException.ThrowIfNull(parameter);

            var metaObject = new DynamicModelBaseMetaObject(parameter, this);
            return metaObject;
        }
    }
}
