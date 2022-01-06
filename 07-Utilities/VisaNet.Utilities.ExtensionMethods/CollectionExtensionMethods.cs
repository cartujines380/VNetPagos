using System;
using System.Collections.Generic;

namespace VisaNet.Utilities.ExtensionMethods
{
    public static class CollectionExtensionMethods
    {
        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            var enumerator = collection.GetEnumerator();
            while (enumerator.MoveNext())
                action.Invoke(enumerator.Current);
        }
    }
}
