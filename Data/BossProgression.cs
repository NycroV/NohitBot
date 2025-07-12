using System.Collections.Frozen;
using NohitBot.Registries;

namespace NohitBot.Data;

public class BossProgression
{
    public string Identifier { get; }
    
    public FrozenSet<Boss> Bosses { get; }
    
    public FrozenSet<Boss> RequiredBosses { get; }
    
    public FrozenSet<Boss> OptionalBosses { get; }
    
    public BossProgression(string identifier, IEnumerable<Boss> bosses, IEnumerable<Boss>? optionalBosses = null)
    {
        Identifier = identifier;
        Bosses = bosses.ToFrozenSet();
        OptionalBosses = optionalBosses?.ToFrozenSet() ?? [];
        RequiredBosses = Bosses.Except(OptionalBosses).ToFrozenSet();
        ProgressionTable.Registry.Add(identifier, this);
    }
    
    #region Boss Progressions
    
    
    
    #endregion
}