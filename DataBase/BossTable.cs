using NohitBot.Data;

namespace NohitBot.Database;

public partial class DataBase
{
    private readonly Dictionary<string, Boss> bossRegistry = [];
    
    public static Dictionary<string, Boss> Bosses => instance.bossRegistry;
}