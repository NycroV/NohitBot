using Newtonsoft.Json;
using NohitBot.DataStructures;

namespace NohitBot.Database;

public partial class DataBase
{
    [JsonProperty] private readonly Dictionary<uint, Nohit> nohitRegistry = [];
    
    public static Dictionary<uint, Nohit> Nohits => instance.nohitRegistry;
}