[assembly: System.Resources.NeutralResourcesLanguage("en-US")]
[assembly: System.Runtime.Versioning.TargetFramework(".NETCoreApp,Version=v6.0", FrameworkDisplayName="")]
public static class ModuleInitializer
{
    public static void Initialize() { }
}
namespace Orc.DynamicObjects
{
    public class DynamicModelBase : Catel.Data.ModelBase, System.Dynamic.IDynamicMetaObjectProvider
    {
        public DynamicModelBase() { }
        public System.Dynamic.DynamicMetaObject GetMetaObject(System.Linq.Expressions.Expression parameter) { }
        protected void RegisterDynamicProperty(string name, System.Type type) { }
    }
    public class DynamicModelBaseMetaObject : System.Dynamic.DynamicMetaObject
    {
        public DynamicModelBaseMetaObject(System.Linq.Expressions.Expression parameter, Orc.DynamicObjects.DynamicModelBase model) { }
        public override System.Dynamic.DynamicMetaObject BindGetMember(System.Dynamic.GetMemberBinder binder) { }
        public override System.Dynamic.DynamicMetaObject BindSetMember(System.Dynamic.SetMemberBinder binder, System.Dynamic.DynamicMetaObject value) { }
    }
    public class DynamicObservableObject : Catel.Data.ObservableObject, System.Dynamic.IDynamicMetaObjectProvider
    {
        public DynamicObservableObject() { }
        public System.Dynamic.DynamicMetaObject GetMetaObject(System.Linq.Expressions.Expression parameter) { }
        protected System.Collections.Generic.IEnumerable<string> GetPropertyNames() { }
        public T GetValue<T>(string propertyName) { }
        public void SetValue(string propertyName, object value) { }
    }
    public class DynamicObservableObjectMetaObject : System.Dynamic.DynamicMetaObject
    {
        public DynamicObservableObjectMetaObject(System.Linq.Expressions.Expression parameter, Orc.DynamicObjects.DynamicObservableObject observableObject) { }
        public override System.Dynamic.DynamicMetaObject BindGetMember(System.Dynamic.GetMemberBinder binder) { }
        public override System.Dynamic.DynamicMetaObject BindSetMember(System.Dynamic.SetMemberBinder binder, System.Dynamic.DynamicMetaObject value) { }
        public override System.Collections.Generic.IEnumerable<string> GetDynamicMemberNames() { }
    }
}