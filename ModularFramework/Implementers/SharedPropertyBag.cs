using System;
using System.Collections;
using System.Linq;
using System.Reflection;

namespace ModularFramework.Implementers {
    public class SharedPropertyAttribute : Attribute {
        public SharedPropertyAttribute() : this(null) { }
        public SharedPropertyAttribute(string propertyName) {
            Name = propertyName;
        }
        public string Name { get; private set; }
    }
    public class SharedPropertyBag : MarshalByRefObject, ISharedPropertyBag {
        object ownerInstance;
        Hashtable propertiesHash = new Hashtable();
        public SharedPropertyBag(object owner) {
            ownerInstance = owner;
            CreateBag();
        }
        void CreateBag() {
            Type ownerType = ownerInstance.GetType();
            PropertyInfo[] propertyInfos = ownerType.GetProperties();
            foreach(PropertyInfo propertyInfo in propertyInfos) {
                CustomAttributeData sharedPropertyAttribute = propertyInfo.CustomAttributes.SingleOrDefault(ad => ad.AttributeType == typeof(SharedPropertyAttribute));
                if(sharedPropertyAttribute == null) continue;
                CustomAttributeNamedArgument nameArg = sharedPropertyAttribute.NamedArguments.SingleOrDefault(arg => arg.MemberName == "Name");
                string sharedPropertyName = nameArg != default(CustomAttributeNamedArgument) && nameArg.TypedValue.Value != null ? (string)nameArg.TypedValue.Value : propertyInfo.Name;
                propertiesHash.Add(sharedPropertyName, propertyInfo);
            }
        }
        public T GetProperty<T>(string name, object[] index = null) {
            PropertyInfo propInfo = (PropertyInfo)propertiesHash[name];
            return (T)propInfo.GetValue(ownerInstance, index);
        }
        public void SetProperty<T>(string name, T value, object[] index = null) {
            PropertyInfo propInfo = (PropertyInfo)propertiesHash[name];
            propInfo.SetValue(ownerInstance, value, index);
        }
        T ISharedPropertyBag.GetProperty<T>(string name) {
            return GetProperty<T>(name);
        }
        T ISharedPropertyBag.GetProperty<T>(string name, object[] index) {
            return GetProperty<T>(name, index);
        }
        void ISharedPropertyBag.SetProperty<T>(string name, T value) {
            SetProperty(name, value);
        }
        void ISharedPropertyBag.SetProperty<T>(string name, T value, object[] index) {
            SetProperty(name, value, index);
        }
    }
}
