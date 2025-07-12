using Newtonsoft.Json;
using NohitBot.Database;

namespace NohitBot.Data;

public struct Difficulty
{
    public readonly string BossProgression = null!;
    
    public readonly string[] Modifiers = null!;
    
    [JsonIgnore] public BossProgression Progression => DataBase.Progressions[BossProgression];
    
    public Difficulty()
    { }
    
    public Difficulty(string bossProgression, string[] modifiers)
    {
        BossProgression = bossProgression;
        Modifiers = modifiers;
    }
}