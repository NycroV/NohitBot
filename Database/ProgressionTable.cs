using Newtonsoft.Json;
using NohitBot.DataStructures;

namespace NohitBot.Database;

public partial class DataBase
{
    [JsonProperty] private readonly List<BossProgression> progressionRegistry = [];

    public static List<BossProgression> Progressions => instance.progressionRegistry;
}