using Newtonsoft.Json;
using NohitBot.Registries;

namespace NohitBot.Data;

public class Boss
{
    public string Name { get; }
    public List<string> Aliases { get; }
    public TimeSpan MinimumNohitLength { get; set; }

    public Boss(string name, IEnumerable<string> aliases)
    {
        Name = name;
        Aliases = aliases.ToList();
        MinimumNohitLength = BossTable.Registry.TryGetValue(name, out Boss? boss) ? boss.MinimumNohitLength : TimeSpan.Zero;
        BossTable.Registry[name] = this;
    }

    public void AddAlias(string alias)
    {
        Aliases.Add(alias);
        BossTable.Save();
    }

    public void RemoveAlias(string alias)
    {
        Aliases.Remove(alias);
        BossTable.Save();
    }
    
    public void SetMNL(TimeSpan minNohitLength)
    {
        MinimumNohitLength = minNohitLength;
        BossTable.Save();
    }
    
    #region Bosses

    #endregion
}