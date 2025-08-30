using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace PinkyToeInstallWizard
{
    public static class SettingsManager
    {
        private static string settingsFile = System.IO.Path.Combine(
            System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData),
            "PinkyToeInstallWizard", "settings.json");

        private class SettingsData
        {
            public string DefaultInstallPath { get; set; }
            public List<string> Passwords { get; set; }
        }

        private static SettingsData data;

        static SettingsManager()
        {
            LoadSettings();
        }

        public static void LoadSettings()
        {
            if (System.IO.File.Exists(settingsFile))
            {
                var json = System.IO.File.ReadAllText(settingsFile);
                data = Newtonsoft.Json.JsonConvert.DeserializeObject<SettingsData>(json);
            }
            else
            {
                data = new SettingsData
                {
                    DefaultInstallPath = "",
                    Passwords = new List<string>(DefaultPasswords.Passwords)
                };
            }
        }

        public static void SaveSettings()
        {
            var dir = System.IO.Path.GetDirectoryName(settingsFile);
            if (!System.IO.Directory.Exists(dir))
                System.IO.Directory.CreateDirectory(dir);
            System.IO.File.WriteAllText(settingsFile, Newtonsoft.Json.JsonConvert.SerializeObject(data));
        }

        public static string GetDefaultInstallPath() => data.DefaultInstallPath ?? "";
        public static void SetDefaultInstallPath(string path)
        {
            data.DefaultInstallPath = path;
            SaveSettings();
        }

        public static List<string> GetSavedPasswords() => data.Passwords;
        public static void SavePasswords(List<string> pwds)
        {
            data.Passwords = pwds;
            SaveSettings();
        }
    }
}