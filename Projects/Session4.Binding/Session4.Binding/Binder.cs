using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Session4.Binding
{
    public class Binder
    {
        private bool ValidateType(Type type)
        {
            return type.IsPrimitive || type == typeof(string);
        }

        public T BindTo<T>(IEnumerable<KeyValuePair<string, string>> pairs)
            where T : class
        {
            var type = typeof(T);
            T obj = Activator.CreateInstance(type) as T;
            foreach (var pair in pairs)
            {
            }
            return obj;
        }
    }
}
