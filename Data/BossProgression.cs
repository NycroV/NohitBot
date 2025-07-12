using System.Collections.Frozen;
using NohitBot.Database;

namespace NohitBot.Data;

public class BossProgression
{
    public string Identifier { get; init; }
    
    public FrozenSet<Boss> Bosses { get; init; }
    
    public FrozenSet<Boss> RequiredBosses { get; init; }
    
    public FrozenSet<Boss> OptionalBosses { get; init; }
    
    private BossProgression(string identifier, IEnumerable<Boss> bosses, IEnumerable<Boss>? optionalBosses = null)
    {
        Identifier = identifier;
        Bosses = bosses.ToFrozenSet();
        OptionalBosses = optionalBosses?.ToFrozenSet() ?? [];
        RequiredBosses = Bosses.Except(OptionalBosses).ToFrozenSet();
    }

    public static BossProgression Make(string identifier, IEnumerable<Boss> bosses, IEnumerable<Boss>? optionalBosses = null)
    {
        if (DataBase.Progressions.TryGetValue(identifier, out var progression))
            return progression;
        
        progression = new(identifier, bosses, optionalBosses);
        DataBase.Progressions.Add(identifier, progression);
        return progression;
    }
    
    #region Boss Progressions
    
    
    
    #endregion
}