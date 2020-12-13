using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace TAC_Grabber
{
    public class SimpleSettings
    {
        public Dictionary<string, int> LastValues { get; set; } = new Dictionary<string, int>();
        public int[] ProxiesPort { get; set; } = Enumerable.Range(8181, 20).ToArray();


        #region serialize
        private const string settingsFileName = "settings.json";

        private static readonly JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            AllowTrailingCommas=true
        };
        public void Save()
        {
            var json = JsonSerializer.Serialize<SimpleSettings>(this,jsonSerializerOptions);
            File.WriteAllText(settingsFileName, json);
        }
        public static SimpleSettings Load()
        {
            try
            {
                var json = File.ReadAllText(settingsFileName);
                return JsonSerializer.Deserialize<SimpleSettings>(json, jsonSerializerOptions);
            }
            catch { return new SimpleSettings(); }
        }
        #endregion
    }
}
