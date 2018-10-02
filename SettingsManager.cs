using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;

namespace vMonitor
{
    public class SettingsManager
    {
        public string SettingsFilePath { get; private set; }

        public SettingsManager()
        {
            this.SettingsFilePath = Path.Combine(Utilities.GetLocalAppDataLowPath(), "Valsto");
            this.SettingsFilePath = Path.Combine(this.SettingsFilePath, "vsetting");
        }

        private IFormatter GetFormatter()
        {
            return new BinaryFormatter();
        }

        public Settings Load()
        {
            if (File.Exists(this.SettingsFilePath))
            {
                using (var stream = new MemoryStream(ProtectedData.Unprotect(
                    File.ReadAllBytes(this.SettingsFilePath),
                    null,
                    DataProtectionScope.CurrentUser)))
                {
                    return (Settings)this.GetFormatter().Deserialize(stream);
                }
            }

            return new Settings()
            {
                 Email = ""
            };
        }

        public void Save(Settings settings)
        {
            string path = Path.GetDirectoryName(this.SettingsFilePath);

            if (path != null)
            {
                Directory.CreateDirectory(path);
            }

            using (var stream = new MemoryStream())
            {
                this.GetFormatter().Serialize(stream, settings);
                File.WriteAllBytes(this.SettingsFilePath, ProtectedData.Protect(
                    stream.GetBuffer(),
                    null,
                    DataProtectionScope.CurrentUser));
            }
        }
    }
}
