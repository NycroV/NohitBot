using Newtonsoft.Json;
using NohitBot.Database;

namespace NohitBot.DataStructures;

[JsonObject(MemberSerialization.OptOut)]
public class Nohit
{
    public uint ID { get; private set; }
    
    public ulong UserID { get; init; }
    
    public string Url { get; init; } = null!;
    
    public Difficulty Difficulty { get; init; }
    
    public Boss Boss { get; init; } = null!;

    public DateTime TimeStamp { get; init; }
    
    public string? UserComment { get; private set; } = null;
    
    public Verification Verification { get; set; } = Verification.Default;
    
    [JsonIgnore] public Journey Journey => DataBase.Journeys[UserID][Difficulty];

    private Nohit() { }

    public Nohit(ulong userId, Boss boss, Difficulty difficulty, string url)
    {
        UserID = userId;
        Boss = boss;
        Difficulty = difficulty;
        Url = url;
    }
}