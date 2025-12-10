using Newtonsoft.Json;

namespace NohitBot.Database;

[JsonObject(MemberSerialization.OptOut)]
public partial class DataBase
{
    private static readonly DataBase instance;
    public const string SavePath = "DataBase.json;";
    public static readonly JsonSerializerSettings SerializerSettings = new()
    {
        Formatting = Formatting.Indented,
        ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
        PreserveReferencesHandling = PreserveReferencesHandling.All
    };

    // Ensures we do not attempt to access the file more than once at the same time.
    private static readonly SemaphoreSlim ioHandle = new(1, 1);
    
    static DataBase()
    {
        if (!File.Exists(SavePath))
        {
            instance = new();
            Save();
            return;
        }
        
        ioHandle.Wait();
        string serialized = File.ReadAllText(SavePath);
        instance = JsonConvert.DeserializeObject<DataBase>(serialized, SerializerSettings)!;
        ioHandle.Release();
    }

    public static void Save()
    {
        ioHandle.Wait();
        string serialized = JsonConvert.SerializeObject(instance, SerializerSettings);
        File.WriteAllText(SavePath, serialized);
        ioHandle.Release();
    }
}