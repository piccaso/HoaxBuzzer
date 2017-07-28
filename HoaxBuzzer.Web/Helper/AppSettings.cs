using System;
using System.ComponentModel;
using System.Configuration;

namespace HoaxBuzzer.Web.Helper
{
    public static class AppSettings
    {
        public static T Get<T>(string key, T defaultValue)
        {
            var appSetting = GetValue(key);
            return appSetting == null ? defaultValue : ConvertValue<T>(appSetting);
        }

        public static T Get<T>(string key)
        {
            var appSetting = GetValue(key);
            if (appSetting == null) throw new Exception($"AppSetting not found for: '{key ?? "<null>"}'");
            return ConvertValue<T>(appSetting);
        }

        private static string GetValue(string key)
        {
            return Environment.GetEnvironmentVariable(key) ?? ConfigurationManager.AppSettings[key];
        }

        private static T ConvertValue<T>(string value)
        {
            var converter = TypeDescriptor.GetConverter(typeof(T));
            return (T) converter.ConvertFromInvariantString(value);
        }

        public static string Get(string key) => Get<string>(key);
    }
}