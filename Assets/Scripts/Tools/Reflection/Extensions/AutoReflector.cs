// Put this under another namespace so that it has to be explicitly imported to not pollute every object
namespace Platformer.Tools.Reflection
{
    public static class AutoReflector
    {
        public static bool TryGetValue<T>(this object instance, string query, out T value)
        {
            return Reflector.TryGetValue(instance, query, out value);
        }

        public static bool TrySetValue(this object instance, string query, object value)
        {
            return Reflector.TrySetValue(instance, query, value);
        }
    }
}
