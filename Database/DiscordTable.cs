using Newtonsoft.Json;
using NohitBot.Data;

namespace NohitBot.Database;

public partial class DataBase
{
    [JsonProperty] private readonly Dictionary<ulong, DiscordConfiguration> discordConfigurations = [];
    
    public static Dictionary<ulong, DiscordConfiguration> DiscordConfigurations => instance.discordConfigurations;
}