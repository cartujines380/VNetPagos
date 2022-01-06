using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;

namespace VisaNet.Utilities.Exportation.ExtensionMethods
{
    public static class ObjectExtensionMethods
    {
        public static void TrimAllStringsProperties(this object obj)
        {
            var props = obj.GetType()
                           .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                           .Where(prop => prop.PropertyType == typeof(string))// Ignore non-string properties
                           .Where(prop => prop.GetIndexParameters().Length == 0)// Ignore indexers
                           .Where(prop => prop.CanWrite && prop.CanRead);// Must be both readable and writable

            foreach (var prop in props)
            {
                var value = (string)prop.GetValue(obj, null);
             
                if (value == null) continue;

                value = value.Trim();
                prop.SetValue(obj, value, null);
            }
        }

        public static void LowerAllStringsProperties(this object obj)
        {
            var props = obj.GetType()
                           .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                           .Where(prop => prop.PropertyType == typeof(string))// Ignore non-string properties
                           .Where(prop => prop.GetIndexParameters().Length == 0)// Ignore indexers
                           .Where(prop => prop.CanWrite && prop.CanRead);// Must be both readable and writable

            foreach (var prop in props)
            {
                var value = (string)prop.GetValue(obj, null);

                if (value == null) continue;

                value = value.ToLower();
                prop.SetValue(obj, value, null);
            }
        }

        public static byte[] SerializeToByteArray(this object obj)
        {
            if (obj == null) return null;

            var ms = new MemoryStream();
            (new BinaryFormatter()).Serialize(ms, obj);
            return ms.ToArray();
        }

        public static object DeserializeToObject(this byte[] arrBytes)
        {
            var memStream = new MemoryStream();
            memStream.Write(arrBytes, 0, arrBytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);

            return (new BinaryFormatter()).Deserialize(memStream);
        }
    }
}

