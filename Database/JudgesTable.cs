using Newtonsoft.Json;
using NohitBot.Data;

namespace NohitBot.Database;

public partial class DataBase
{
    [JsonProperty] private readonly Dictionary<ulong, JudgeProfile> judgeRegistry = [];
    
    public static Dictionary<ulong, JudgeProfile> Judges => instance.judgeRegistry;
}