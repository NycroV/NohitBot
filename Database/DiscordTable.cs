using Newtonsoft.Json;
using NohitBot.DataStructures;

namespace NohitBot.Database;

public partial class DataBase
{
    [JsonProperty] private readonly Dictionary<ulong, DiscordConfig> discordConfigs = [];
    
    public static Dictionary<ulong, DiscordConfig> DiscordConfigs => instance.discordConfigs;
}