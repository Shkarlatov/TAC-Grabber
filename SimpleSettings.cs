using System.IO;
using System.Linq;
using System.Text.Json;

namespace TAC_Grabber
{
    public class SimpleSettings
    {
        public int LastValue { get; set; }
        public int[] ProxiesPort { get; set; } = Enumerable.Range(8181, 1).ToArray();


        private const string settingsFileName = "settings.json";
        public void Save()
        {
            var json = JsonSerializer.Serialize<SimpleSettings>(this);
            File.WriteAllText(settingsFileName, json);
        }
        public static SimpleSettings Load()
        {
            try
            {
                var json = File.ReadAllText(settingsFileName);
                return JsonSerializer.Deserialize<SimpleSettings>(json);
            }
            catch { return new SimpleSettings(); }
        }
    }
}
