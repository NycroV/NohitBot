using System.Collections;
using System.Collections.Frozen;
using NohitBot.Database;

namespace NohitBot.Data;

public class BossProgression
{
    public string Name { get; private set; } = null!;
    
    private List<BossContainer> progression { get; init; } = null!;

    public FrozenSet<BossContainer> Progression => progression.ToFrozenSet();

    public FrozenSet<Boss> Bosses => Progression.Select(c => c.Boss).ToFrozenSet();

    public FrozenSet<Boss> RequiredBosses => Progression.Where(c => !c.Optional).Select(c => c.Boss).ToFrozenSet();
    
    public FrozenSet<Boss> OptionalBosses => Progression.Where(c => c.Optional).Select(c => c.Boss).ToFrozenSet();
    
    public ulong ManagementServer { get; init; }
    
    private BossProgression() { }
    
    private BossProgression(string name, ulong managementServer)
    {
        Name = name;
        progression = [];
        ManagementServer = managementServer;
    }

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