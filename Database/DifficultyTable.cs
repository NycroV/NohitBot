using NohitBot.Data;

namespace NohitBot.Database;

public partial class DataBase
{
    public static readonly Dictionary<string, Difficulty> Difficulties = [];
    
    public static readonly Dictionary<string, string> DifficultyModifiers = [];
}