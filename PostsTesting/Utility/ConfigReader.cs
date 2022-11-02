using System.Diagnostics;
using System.Xml.Linq;

namespace PostsTesting.Utility
{
    public static class ConfigReader
    {
        private static XDocument configuration = null;
        private static string configurationPath = GetConfigurationFile(AppDomain.CurrentDomain.BaseDirectory);

        //HELPER METHODS
        public static IDictionary<string, string> GetConfigurationEntries()
        {
            try
            {
                configuration = XDocument.Load(configurationPath, LoadOptions.None);
                return configuration.Descendants("configEntry").Select(e => new KeyValuePair<string, string>(
                            GetAttributeValue(e, "key"),
                            GetAttributeValue(e, "value"))).ToDictionary(x => x.Key, y => y.Value);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message + " " + ex.StackTrace);
            }

            return null;
        }

        private static string GetConfigurationFile(string path)
        {
            if (path.Length < 3) return null;

            string searchResult = Directory.EnumerateFiles(path, "*.*", SearchOption.TopDirectoryOnly)
                    .FirstOrDefault(x => x.ToLowerInvariant().EndsWith("posts-testing.config"));

            return string.IsNullOrWhiteSpace(searchResult)
                ? GetConfigurationFile(GetParentFolderPath(path))
                : searchResult;
        }

        private static string GetParentFolderPath(string folderPath)
        {
            return folderPath.Remove(folderPath.LastIndexOf('\\'));
        }

        private static string GetAttributeValue(XElement node, string attribute)
        {
            return node.Attribute(XName.Get(attribute, string.Empty)).Value;
        }
    }
}

