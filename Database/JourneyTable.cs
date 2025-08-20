using Newtonsoft.Json;
using NohitBot.Data;

namespace NohitBot.Database;

public partial class DataBase
{
    [JsonProperty] private readonly Dictionary<ulong, Dictionary<Difficulty, Journey>> journeyRegistry = [];
    
    public static Dictionary<ulong, Dictionary<Difficulty, Journey>> Journeys => instance.journeyRegistry;
}