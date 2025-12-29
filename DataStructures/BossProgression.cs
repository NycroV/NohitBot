using System.Collections.ObjectModel;
using Newtonsoft.Json;
using NohitBot.Database;

namespace NohitBot.DataStructures;

public class BossProgression
{
    private BossProgression()
    {
    }

    private BossProgression(string name, ulong managementServer)
    {
        Name = name;
        progression = [];
        ManagementServer = managementServer;
    }

    public string Name { get; private set; } = null!;

    private List<BossContainer> progression { get; } = null!;

    [JsonIgnore] public ReadOnlyCollection<BossContainer> Progression => progression.AsReadOnly();

    [JsonIgnore] public ReadOnlyCollection<Boss> Bosses => Progression.Select(c => c.Boss).ToArray().AsReadOnly();

    [JsonIgnore] public ReadOnlyCollection<Boss> RequiredBosses => Progression.Where(c => !c.Optional).Select(c => c.Boss).ToArray().AsReadOnly();

    [JsonIgnore] public ReadOnlyCollection<Boss> OptionalBosses => Progression.Where(c => c.Optional).Select(c => c.Boss).ToArray().AsReadOnly();

    public ulong ManagementServer { get; init; }

    public static BossProgression Make(string identifier, ulong managementServer, BossProgression? copy = null)
    {
        BossProgression progression = new(identifier, managementServer);

        if (copy is not null)
            progression.progression.AddRange(copy.Progression);

        DataBase.Progressions.Add(progression);
        DataBase.Save();
        return progression;
    }

    public void Delete()
    {
        DataBase.Progressions.Remove(this);
        DataBase.Save();
    }

    public record struct BossContainer(Boss Boss, Boss? EquivalentBoss = null, bool Optional = false);
}