using System.Collections.ObjectModel;
using Newtonsoft.Json;
using NohitBot.Database;

namespace NohitBot.DataStructures;

[JsonObject(MemberSerialization.OptOut)]
public class Boss
{
    private Boss()
    {
    }

    private Boss(string name, ulong managementServer, IEnumerable<string>? bossAliases = null)
    {
        Name = name;
        aliases = bossAliases?.ToList() ?? [];
        ManagementServer = managementServer;
    }

    public string Name { get; private set; } = null!;

    private List<string> aliases { get; init; } = null!;

    [JsonIgnore] public ReadOnlyCollection<string> Aliases => aliases.AsReadOnly();

    public ulong ManagementServer { get; init; }

    public static Boss Make(string name, ulong managementServer, IEnumerable<string>? bossAliases = null)
    {
        Boss boss = new(name, managementServer, bossAliases);
        DataBase.Bosses.Add(name, boss);
        DataBase.Save();
        return boss;
    }

    public void Delete()
    {
        DataBase.Bosses.Remove(Name);
        DataBase.Save();
    }
}