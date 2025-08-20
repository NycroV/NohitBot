using Newtonsoft.Json;
using NohitBot.Data;

namespace NohitBot.Database;

public partial class DataBase
{
    [JsonProperty] private readonly Dictionary<string, Boss> bossRegistry = [];
    
    public static Dictionary<string, Boss> Bosses => instance.bossRegistry;
}