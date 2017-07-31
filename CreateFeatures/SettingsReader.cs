using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CreateFeatures
{
    internal class SettingsReader
    {
        private static Dictionary<string, string> m_LastUsedValues;

        public static Dictionary<string, string> LastUsedValues
        {
            get
            {
                if (m_LastUsedValues == null)
                {
                    m_LastUsedValues = new Dictionary<string, string>();
                    if (!m_ReadSettings && File.Exists(s_SettingsFileName))
                    {
                        ReadFromDisk();
                    }
                }
                return m_LastUsedValues;
            }
            //private set { m_LastUsedValues = value; }
        }

        private static void ReadFromDisk()
        {
            var textLines = File.ReadAllLines(s_SettingsFileName);
            foreach (string textLine in textLines)
            {
                string key, value = null;
                var strArr = textLine.Split(new char[] {'='}, StringSplitOptions.RemoveEmptyEntries);
                if (strArr.Length > 1)
                {
                    value = strArr[1].Trim();
                }
                if (strArr.Length > 0)
                {
                    key = strArr[0].Trim();
                    m_LastUsedValues.Add(key, value);
                }
            }
            m_ReadSettings = true;
        }

        private static bool m_ReadSettings = false;
        private static string s_SettingsFileName = Path.GetFullPath(Path.Combine("Settings.txt"));
        public static bool SettingExists(string settingKey)
        {
            return LastUsedValues.Keys.Contains(settingKey);
        }


        public static string GetSetting(string key, string defaultValue)
        {
            if (LastUsedValues.Keys.Contains(key))
            {
                return LastUsedValues[key];
            }
            return defaultValue;
        }

        public static void SetSetting(string key, string value)
        {
            if (!LastUsedValues.Keys.Contains(key))
            {
                LastUsedValues.Add(key, null);
            }
            LastUsedValues[key] = value;
            FlushToDisk();
        }

        private static void FlushToDisk()
        {
            StringBuilder str = new StringBuilder();
            foreach (var kvp in LastUsedValues)
            {
                str.AppendLine($"{kvp.Key}={kvp.Value}");
            }
            File.WriteAllText(s_SettingsFileName, str.ToString());
        }
    }
}