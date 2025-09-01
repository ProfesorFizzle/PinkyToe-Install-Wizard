using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace PinkyToeInstallWizard
{
    public static class SettingsManager
    {
        private static readonly string AppDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "PinkyToeInstallWizard");
        private static readonly string ConfigPath = Path.Combine(AppDir, "settings.json");

        private class SettingsData
        {
            public string DefaultInstallPath { get; set; } = @"C:\Games";
            public List<string> EncryptedPasswords { get; set; } = new();
            public List<string> RecentArchives { get; set; } = new(); // reserved for future
        }

        private static SettingsData? _settings;

        private static SettingsData Settings
        {
            get
            {
                if (_settings != null) return _settings;
                try
                {
                    if (File.Exists(ConfigPath))
                        _settings = JsonConvert.DeserializeObject<SettingsData>(File.ReadAllText(ConfigPath)) ?? new SettingsData();
                    else
                        _settings = new SettingsData();
                }
                catch
                {
                    _settings = new SettingsData();
                }
                return _settings;
            }
        }

        public static string GetDefaultInstallPath() => Settings.DefaultInstallPath;

        public static void SetDefaultInstallPath(string path)
        {
            Settings.DefaultInstallPath = path;
            Save();
        }

        public static List<string> GetSavedPasswords()
        {
            var combined = new List<string>(DefaultPasswords.Passwords);
            foreach (var encrypted in Settings.EncryptedPasswords)
            {
                try
                {
                    var bytes = Convert.FromBase64String(encrypted);
                    var decrypted = ProtectedData.Unprotect(bytes, null, DataProtectionScope.CurrentUser);
                    combined.Add(Encoding.UTF8.GetString(decrypted));
                }
                catch { }
            }
            return combined;
        }

        public static void SavePasswords(List<string> plain)
        {
            Settings.EncryptedPasswords.Clear();
            foreach (var p in plain)
            {
                try
                {
                    var enc = ProtectedData.Protect(Encoding.UTF8.GetBytes(p), null, DataProtectionScope.CurrentUser);
                    Settings.EncryptedPasswords.Add(Convert.ToBase64String(enc));
                }
                catch { }
            }
            Save();
        }

        private static void Save()
        {
            try
            {
                Directory.CreateDirectory(AppDir);
                File.WriteAllText(ConfigPath, JsonConvert.SerializeObject(Settings, Formatting.Indented));
            }
            catch { }
        }
    }
}