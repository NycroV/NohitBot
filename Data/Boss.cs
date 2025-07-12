using Newtonsoft.Json;
using NohitBot.Database;

namespace NohitBot.Data;

[JsonObject(MemberSerialization.OptOut)]
public class Boss()
{
    public string Name { get; } = "";
    
    public List<string> Aliases { get; } = [];
    
    public TimeSpan MinimumNohitLength { get; set; } = TimeSpan.Zero;

    private Boss(string name) : this()
    {
        Name = name;
    }

    public static Boss Make(string name)
    {
        if (DataBase.Bosses.TryGetValue(name, out var boss))
            return boss;

        boss = new(name);
        DataBase.Bosses.Add(name, boss);
        return boss;
    }
    
    #region Bosses

    #endregion
}