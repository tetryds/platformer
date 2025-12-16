using System;
using System.Reflection;

namespace Platformer.Tools
{
    public static class Reflector
    {
        private static readonly BindingFlags MyFlags = BindingFlags.Public
           | BindingFlags.NonPublic
           | BindingFlags.Instance
           | BindingFlags.Static;

        public static bool TryGetValue<T>(object instance, string query, out T value)
        {
            string[] steps = query.Split('.');

            object current = instance;

            for (int i = 0; i < steps.Length; i++)
            {
                current = GetChild(current, steps[i]);
                if (current == null)
                {
                    value = default;
                    return false;
                }
            }

            try
            {
                value = (T)current;
                return true;
            }
            catch
            {
                value = default;
                return false;
            }
        }

        public static bool TrySetValue(object instance, string query, object value)
        {
            string[] steps = query.Split('.');

            object current = instance;

            for (int i = 0; i < steps.Length - 1; i++)
            {
                current = GetChild(current, steps[i]);
                if (current == null)
                {
                    return false;
                }
            }

            return TrySetChild(current, steps[^1], value);
        }

        private static object GetChild(object o, string name)
        {
            Type type = o.GetType();
            return type.GetField(name, MyFlags).GetValue(o) ?? type.GetProperty(name, MyFlags).GetValue(o);
        }

        private static bool TrySetChild(object o, string name, object value)
        {
            return TrySetField(o, name, value) || TrySetProperty(o, name, value);
        }

        private static bool TrySetField(object o, string name, object value)
        {
            Type type = o.GetType();
            type.GetField(name, MyFlags).SetValue(o, value);
            return type.GetField(name, MyFlags).GetValue(o) == value;
        }

        private static bool TrySetProperty(object o, string name, object value)
        {
            Type type = o.GetType();
            type.GetProperty(name, MyFlags).SetValue(o, value);
            return type.GetField(name, MyFlags).GetValue(o) == value;
        }
    }
}
