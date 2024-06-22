using System.IO;
using System.Reflection;
using System.Windows.Input;
using Newtonsoft.Json;

public class SettingsManager
{
    private readonly string _settingsFilePath;

    [JsonProperty]
    public String StartHotkey { get; set; } = Key.None.ToString();

    [JsonProperty]
    public String StopHotkey { get; set; } = Key.None.ToString();
    public int Jitter { get; set; } = 0;
    public int Interval { get; set; } = 100;

    public SettingsManager()
    {
        string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        string appName = Assembly.GetEntryAssembly().GetName().Name;
        _settingsFilePath = Path.Combine(appDataFolder, appName, "settings.json");
    }

    public void SaveSettings()
    {
        string settingsJson = JsonConvert.SerializeObject(this, Formatting.Indented);
        Directory.CreateDirectory(Path.GetDirectoryName(_settingsFilePath));
        File.WriteAllText(_settingsFilePath, settingsJson);
    }

    public void LoadSettings()
    {
        if (File.Exists(_settingsFilePath))
        {
            string settingsJson = File.ReadAllText(_settingsFilePath);
            JsonConvert.PopulateObject(settingsJson, this);
        }
    }
}