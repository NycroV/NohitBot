using Newtonsoft.Json;
using NohitBot.Data;

namespace NohitBot.Database;

public partial class DataBase
{
    [JsonProperty] private readonly List<Difficulty.GameMode> gameModeRegistry = [];
    
    [JsonProperty] private readonly List<Difficulty.Modifier> modifierRegistry = [];

    public static List<Difficulty.GameMode> GameModes => instance.gameModeRegistry;
    
    public static List<Difficulty.Modifier> DifficultyModifiers => instance.modifierRegistry;
}