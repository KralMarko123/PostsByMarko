using System.Dynamic;

namespace PostsTesting.Utility.Extensions
{
    public static class ObjectExtensions
    {
        public static bool CheckForProperty(this ExpandoObject obj, string name)
        {
            return (obj as IDictionary<string, object>).ContainsKey(name);
        }

        public static object GetProperty(this ExpandoObject obj, string name)
        {
            (obj as IDictionary<string, object>).TryGetValue(name, out object? value);
            return value;
        }
    }
}
