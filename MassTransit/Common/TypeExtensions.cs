using System;
using System.Linq;

namespace Common
{
    public static class TypeExtensions
    {
        public static bool Implements(this Type type, Type interfaceType)
        {
            return type.GetInterfaces().Contains(interfaceType);
        }

        public static bool Implements<T>(this Type type)
        {
            return type.GetInterfaces().Contains(typeof (T));
        }
    }
}
