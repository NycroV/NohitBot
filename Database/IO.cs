using Newtonsoft.Json;

namespace NohitBot.Database;

[JsonObject(MemberSerialization.OptOut)]
public partial class DataBase
{
    public const string SavePath = "DataBase.json;";
    
    private static readonly DataBase instance;
    
    private static readonly JsonSerializerSettings SerializerSettings = new()
    {
        Formatting = Formatting.Indented,
        ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
        PreserveReferencesHandling = PreserveReferencesHandling.All
    };

    // Ensures we do not attempt to access the file more than once at the same time.
    private static readonly SemaphoreSlim ioHandle = new(1, 1);
    
    private static string Serialize() => JsonConvert.SerializeObject(instance, SerializerSettings);
    
    private static DataBase Deserialize(string json) => JsonConvert.DeserializeObject<DataBase>(json, SerializerSettings)!;

    public static string ReadFile()
    {
        ioHandle.Wait();
        string text = File.ReadAllText(SavePath);
        ioHandle.Release();
        return text;
    }

    public static void WriteFile(string text)
    {
        ioHandle.Wait();
        File.WriteAllText(SavePath, text);
        ioHandle.Release();
    }

    static DataBase()
    {
        if (!File.Exists(SavePath))
        {
            instance = new();
            Save();
            return;
        }
        
        string serialized = ReadFile();
        instance = Deserialize(serialized);
    }

    public static void Save()
    {
        string serialized = Serialize();
        WriteFile(serialized);
    }
}