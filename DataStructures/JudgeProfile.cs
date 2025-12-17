
using System.Collections.Frozen;
using NohitBot.Database;

namespace NohitBot.DataStructures;

public class JudgeProfile
{
    public ulong UserId { get; init; }

    public string Name { get; private set; } = null!;

    public string JourneyMessage { get; private set; } = null!;
    
    private List<string> aliases { get; set; } = null!;
    
    public FrozenSet<string> Aliases => aliases.ToFrozenSet();
    
    private JudgeProfile() { }

    private JudgeProfile(ulong userId, string name, string journeyMessage, IEnumerable<string>? judgeAliases = null)
    {
        UserId = userId;
        Name = name;
        JourneyMessage = journeyMessage;
        aliases = judgeAliases?.ToList() ?? [];
    }

    public static JudgeProfile Make(ulong userId, string name, string journeyMessage, IEnumerable<string>? judgeAliases = null)
    {
        JudgeProfile profile = new(userId, name, journeyMessage, judgeAliases);
        DataBase.Judges.Add(userId, profile);
        DataBase.Save();
        return profile;
    }

    public void Update(string name, string journeyMessage, IEnumerable<string>? judgeAliases = null)
    {
        Name = name;
        JourneyMessage = journeyMessage;
        aliases = judgeAliases?.ToList() ?? [];
        DataBase.Save();
    }
}