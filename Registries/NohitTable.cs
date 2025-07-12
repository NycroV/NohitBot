using Newtonsoft.Json;
using NohitBot.Data;

namespace NohitBot.Registries;

public class NohitTable
{
    private static readonly NohitTable instance;
    [JsonProperty] private readonly Dictionary<uint, Nohit> nohitRegistry = [];
    [JsonProperty] private readonly Dictionary<ulong, Dictionary<BossProgression, Journey>> journeyRegistry = [];
    
    public static Dictionary<uint, Nohit> Registry => instance.nohitRegistry;
    public static Dictionary<ulong, Dictionary<BossProgression, Journey>> Journeys => instance.journeyRegistry;

    public const string SavePath = "NohitTable.json;";
    public static readonly JsonSerializerSettings SerializerSettings = new()
    {
        Formatting = Formatting.Indented,
        ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
        PreserveReferencesHandling = PreserveReferencesHandling.All
    };

    public static void Register(Nohit nohit)
    {
        nohit.ID = (uint)Registry.Count;
        Registry.Add(nohit.ID, nohit);

        Journeys.TryAdd(nohit.UserID, []);
        var userJourneys = Journeys[nohit.UserID];

        userJourneys.TryAdd(nohit.BossProgression, new(nohit.UserID, nohit.BossProgression));
        Journey journey = userJourneys[nohit.BossProgression];

        if (journey.Nohits.TryGetValue(nohit.Boss, out Nohit? oldNohit))
        {
            journey.OldNohits.TryAdd(nohit.Boss, []);
            journey.OldNohits[nohit.Boss].Add(oldNohit);
        }
        
        journey.Nohits[nohit.Boss] = nohit;
        Save();
    }
    
    // Ensures we do not attempt to access the file more than once at the same time.
    private static readonly SemaphoreSlim ioHandle = new(1, 1);
    
    static NohitTable()
    {
        if (!File.Exists(SavePath))
        {
            instance = new();
            Save();
            return;
        }
        
        ioHandle.Wait();
        string serialized = File.ReadAllText(SavePath);
        instance = JsonConvert.DeserializeObject<NohitTable>(serialized, SerializerSettings)!;
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