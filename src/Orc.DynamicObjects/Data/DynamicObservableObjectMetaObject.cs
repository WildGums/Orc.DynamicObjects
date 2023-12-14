namespace Orc.DynamicObjects
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq.Expressions;
    using System.Reflection;
    using Catel.Reflection;

    /// <summary>
    /// Class containing metadata for the <see cref="DynamicObservableObject"/>.
    /// </summary>
    public class DynamicObservableObjectMetaObject : DynamicMetaObject
    {
        private static readonly MethodInfo _getValueMethodInfo;
        private static readonly MethodInfo _setValueMethodInfo;

        /// <summary>
        /// Initializes static members of the <see cref=" DynamicObservableObjectMetaObject"/> class.
        /// </summary>
        static DynamicObservableObjectMetaObject()
        {
            var bindingFlags = BindingFlags.Instance | BindingFlags.Public;

            var getValueMethodInfo = typeof(DynamicObservableObject).GetMethodEx("GetValue", bindingFlags)?.MakeGenericMethod(new[] { typeof(object) });
            if (getValueMethodInfo is null)
            {
                throw new InvalidOperationException("Cannot find GetValue method on DymamicObservableObject");
            }

            _getValueMethodInfo = getValueMethodInfo;

            var setValueMethodInfo = typeof(DynamicObservableObject).GetMethodEx("SetValue", bindingFlags);
            if (setValueMethodInfo is null)
            {
                throw new InvalidOperationException("Cannot find GetValue method on DymamicObservableObject");
            }

            _setValueMethodInfo = setValueMethodInfo;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicObservableObjectMetaObject"/> class.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="observableObject">The observable object.</param>
        public DynamicObservableObjectMetaObject(Expression parameter, DynamicObservableObject observableObject)
            : base(parameter, BindingRestrictions.Empty, observableObject)
        {

        }

        /// <summary>
        /// Returns the enumeration of all dynamic member names.
        /// </summary>
        /// <returns>The list of dynamic member names.</returns>
        public override IEnumerable<string> GetDynamicMemberNames()
        {
            var obj = Value as DynamicObservableObject;
            if (obj is null)
            {
                return Array.Empty<string>();
            }

            return obj.GetPropertyNames();
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

            var valueExpression = Expression.Convert(value.Expression, typeof(object));
            var parameters = new Expression[]
            {
                Expression.Constant(propertyName),
                valueExpression
            };

            var self = Expression.Convert(Expression, LimitType);
            var methodCall = Expression.Call(self, _setValueMethodInfo, parameters);

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

            var parameters = new Expression[]
            {
                Expression.Constant(propertyName)
            };

            var self = Expression.Convert(Expression, LimitType);
            var methodCall = Expression.Call(self, _getValueMethodInfo, parameters);
            var restrictions = BindingRestrictions.GetTypeRestriction(Expression, LimitType);

            var getValue = new DynamicMetaObject(methodCall, restrictions);
            return getValue;
        }
    }
}
