using System.Text.Json;
using System.Text.Json.Serialization;
using LostAndFound.Models;

namespace LostAndFound.Data;

/// <summary>Stores items as JSON in %AppData%\LostAndFound\items.json.</summary>
public class ItemStore
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly string _path;

    public List<Item> Items { get; private set; } = new();

    public ItemStore()
    {
        var folder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "LostAndFound");
        Directory.CreateDirectory(folder);
        _path = Path.Combine(folder, "items.json");
        Load();
    }

    private void Load()
    {
        if (!File.Exists(_path)) return;
        try
        {
            var json = File.ReadAllText(_path);
            Items = JsonSerializer.Deserialize<List<Item>>(json, Options) ?? new();
        }
        catch (JsonException)
        {
            // Keep the unreadable file as a backup and start fresh.
            File.Move(_path, _path + ".corrupt-" + DateTime.Now.ToString("yyyyMMddHHmmss"));
            Items = new();
        }
    }

    public void Save()
    {
        var temp = _path + ".tmp";
        File.WriteAllText(temp, JsonSerializer.Serialize(Items, Options));
        File.Move(temp, _path, overwrite: true);
    }
}
