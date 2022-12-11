using System.Dynamic;

namespace PostsTesting.Utility.Extensions
{
    public static class ObjectExtensions
    {
        public static bool CheckForProperty(this ExpandoObject obj, string name)
        {
            return ((IDictionary<string, object>)obj).ContainsKey(name);
        }

        public static object GetProperty(this ExpandoObject obj, string name)
        {
            ((IDictionary<string, object>)obj).TryGetValue(name, out object? value);
            return value;
        }
    }
}
