using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace VisaNet.Common.FrameworkExtensions
{
    public static class ObjectExtensions
    {
        public static T ToObject<T>(this IDictionary<string, object> source)
        where T : class, new()
        {
            var someObject = new T();
            var someObjectType = someObject.GetType();

            foreach (var item in source)
            {
                someObjectType
                         .GetProperty(item.Key)
                         .SetValue(someObject, item.Value, null);
            }

            return someObject;
        }

        public static IDictionary<string, object> AsDictionary(this object source, BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
        {
            return source.GetType().GetProperties(bindingAttr).ToDictionary
            (
                propInfo => propInfo.Name,
                propInfo => propInfo.GetValue(source, null)
            );

        }

        public static void CopyValues<T>(T target, T source)
        {
            Type t = typeof(T);

            var properties = t.GetProperties().Where(prop => prop.CanRead && prop.CanWrite);

            foreach (var prop in properties)
            {
                var value = prop.GetValue(source, null);
                if (value != null)
                    prop.SetValue(target, value, null);
            }
        }

        public static object Merge(object item1, object item2)
        {
            if (item1 == null || item2 == null)
            {
                return item1 ?? item2 ?? new ExpandoObject();
            }                

            var expando = new ExpandoObject();
            var result = expando as IDictionary<string, object>;
            foreach (var fi in item1.GetType().GetProperties().Where(prop => prop.CanRead))
            {
                result[fi.Name] = fi.GetValue(item1, null);
            }
            foreach (var fi in item2.GetType().GetProperties().Where(prop => prop.CanRead))
            {
                result[fi.Name] = fi.GetValue(item2, null);
            }
            return result;
        }
    }
}
