using System.Configuration;

namespace DictionarySearcher.Utils
{
    public class AppSettingConfigurationHandler
    {
        public static string GetAppSettingConfiguration(string key, string defaultValue)
        {
            return !string.IsNullOrEmpty(ConfigurationManager.AppSettings[key]) ?
                    ConfigurationManager.AppSettings[key] : defaultValue;
        }
    }
}
