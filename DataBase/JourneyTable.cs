using NohitBot.Data;

namespace NohitBot.Database;

public partial class DataBase
{
    private readonly Dictionary<ulong, Dictionary<Difficulty, Journey>> journeyRegistry = [];
    
    public static Dictionary<ulong, Dictionary<Difficulty, Journey>> Journeys => instance.journeyRegistry;
}