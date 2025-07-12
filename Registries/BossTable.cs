using Newtonsoft.Json;
using NohitBot.Data;

namespace NohitBot.Registries;

public class BossTable
{
    public static readonly Dictionary<string, Boss> Registry;
    public const string BossesPath = "Bosses.json";
    public static readonly JsonSerializerSettings SerializerSettings = new()
    {
        Formatting = Formatting.Indented
    };
    
    // Ensures we do not attempt to access the file more than once at the same time.
    private static readonly SemaphoreSlim ioHandle = new(1, 1);
    
    static BossTable()
    {
        if (!File.Exists(BossesPath))
        {
            Registry = new();
            Save();
            return;
        }
        
        ioHandle.Wait();
        string serialized = File.ReadAllText(BossesPath);
        Registry = JsonConvert.DeserializeObject<Dictionary<string, Boss>>(serialized, SerializerSettings)!;
        ioHandle.Release();
    }

    public static void Save()
    {
        ioHandle.Wait();
        string serialized = JsonConvert.SerializeObject(Registry, SerializerSettings);
        File.WriteAllText(BossesPath, serialized);
        ioHandle.Release();
    }
}