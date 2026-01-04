namespace Orc.DynamicObjects
{
    using System;
    using System.Dynamic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Catel.Caching;
    using Catel.Data;
    using Catel.Logging;
    using Catel.Reflection;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Class containing metadata for the <see cref="DynamicModelBase"/>.
    /// </summary>
    public class DynamicModelBaseMetaObject : DynamicMetaObject
    {
        private static readonly ILogger Logger = LogManager.GetLogger(typeof(DynamicModelBaseMetaObject));

        private static readonly MethodInfo _getValueFastMethodInfo;
        private static readonly MethodInfo _setValueFastMethodInfo;

        private static readonly CacheStorage<Type, MethodInfo?> _registerSimplePropertyCache = new CacheStorage<Type, MethodInfo?>();

        /// <summary>
        /// Initializes static members of the <see cref="DynamicModelBaseMetaObject"/> class.
        /// </summary>
        static DynamicModelBaseMetaObject()
        {
            var bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;

            var modelBaseType = typeof(ModelBase);

            var getMethods = modelBaseType.GetMethodsEx(bindingFlags).Where(x => x.Name == "GetValueFromPropertyBag");
            _getValueFastMethodInfo = getMethods.First().MakeGenericMethod(new[] { typeof(object) });

            var setMethods = modelBaseType.GetMethodsEx(bindingFlags).Where(x => x.Name == "SetValueToPropertyBag");
            _setValueFastMethodInfo = setMethods.First(x => x.IsGenericMethod).MakeGenericMethod(new[] { typeof(object) });
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicModelBaseMetaObject"/> class.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="model">The model.</param>
        public DynamicModelBaseMetaObject(Expression parameter, DynamicModelBase model)
            : base(parameter, BindingRestrictions.Empty, model)
        {

        }

        /// <summary>
        /// Performs the binding of the dynamic set member operation.
        /// </summary>
        /// <param name="binder">An instance of the <see cref="T:System.Dynamic.SetMemberBinder" /> that represents the details of the dynamic operation.</param>
        /// <param name="value">The <see cref="T:System.Dynamic.DynamicMetaObject" /> representing the value for the set member operation.</param>
        /// <returns>The new <see cref="T:System.Dynamic.DynamicMetaObject" /> representing the result of the binding.</returns>
        public override DynamicMetaObject BindSetMember(SetMemberBinder binder, DynamicMetaObject value)
        {
            ArgumentNullException.ThrowIfNull(binder);
            ArgumentNullException.ThrowIfNull(value);

            var propertyName = binder.Name;
            var propertyType = binder.ReturnType;

            RegisterPropertyIfNotYetRegistered(propertyName, propertyType);

            var valueExpression = Expression.Convert(value.Expression, typeof(object));
            var parameters = new Expression[]
            {
                Expression.Constant(propertyName),
                valueExpression
            };

            var self = Expression.Convert(Expression, LimitType);
            var methodCall = Expression.Call(self, _setValueFastMethodInfo, parameters);

            // Note: required because we must return a value as well
            var finalExpression = Expression.Block(methodCall, valueExpression);
            var restrictions = BindingRestrictions.GetTypeRestriction(Expression, LimitType);

            var setValue = new DynamicMetaObject(finalExpression, restrictions);
            return setValue;
        }

        /// <summary>
        /// Performs the binding of the dynamic get member operation.
        /// </summary>
        /// <param name="binder">An instance of the <see cref="T:System.Dynamic.GetMemberBinder" /> that represents the details of the dynamic operation.</param>
        /// <returns>The new <see cref="T:System.Dynamic.DynamicMetaObject" /> representing the result of the binding.</returns>
        public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
        {
            ArgumentNullException.ThrowIfNull(binder);

            var propertyName = binder.Name;
            var propertyType = binder.ReturnType;

            RegisterPropertyIfNotYetRegistered(propertyName, propertyType);

            var parameters = new Expression[]
            {
                Expression.Constant(propertyName)
            };

            var self = Expression.Convert(Expression, LimitType);
            var methodCall = Expression.Call(self, _getValueFastMethodInfo, parameters);
            var restrictions = BindingRestrictions.GetTypeRestriction(Expression, LimitType);

            var getValue = new DynamicMetaObject(methodCall, restrictions);
            return getValue;
        }

        private void RegisterPropertyIfNotYetRegistered(string propertyName, Type propertyType)
        {
            ArgumentNullException.ThrowIfNull(propertyName);
            ArgumentNullException.ThrowIfNull(propertyType);

            var model = Value as ModelBase;
            if (model is null)
            {
                return;
            }

            if (model.IsPropertyRegistered(propertyName))
            {
                return;
            }

            var modelType = model.GetType();
            Logger.LogDebug("Register dynamic property '{0}.{1}' of type '{2}'", modelType.GetSafeFullName(false), propertyName, propertyType.GetSafeFullName(false));

            var registerPropertyMethodInfo = GetRegisterSimplePropertyMethodInfo(modelType);
            if (registerPropertyMethodInfo is null)
            {
                throw Logger.LogErrorAndCreateException<InvalidCastException>($"Cannot find register property method on ModelBase");
            }

            registerPropertyMethodInfo.Invoke(model, new object[] { propertyName, propertyType });
        }

        private static MethodInfo? GetRegisterSimplePropertyMethodInfo(Type modelBaseType)
        {
            ArgumentNullException.ThrowIfNull(modelBaseType);

            return _registerSimplePropertyCache.GetFromCacheOrFetch(modelBaseType, () =>
            {
                var bindingFlags = BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.NonPublic;
                var methodInfo = modelBaseType.GetMethodEx("RegisterDynamicProperty", bindingFlags);
                return methodInfo;
            });
        }
    }
}
