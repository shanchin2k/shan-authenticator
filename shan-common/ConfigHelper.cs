
namespace Shan.Authentication.API.Common
{

    /// <summary>
    /// Helper class to have all the App Setting keys
    /// </summary>
    public class ConfigHelper
    {
        /// <summary>
        /// Key vault keys
        /// </summary>
        public static AppSettings appSettings = new AppSettings();

        /// <summary>
        /// AppSettings Helper
        /// </summary>
        /// <param name="appSetting"> AppSetting Keys</param>
        public ConfigHelper(AppSettings appSetting)
        {
            appSettings = appSetting;
        }
    }
}
