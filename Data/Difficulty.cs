using Newtonsoft.Json;
using NohitBot.Data;
using NohitBot.Database;

namespace NohitBot.Data;

public readonly struct Difficulty
{
    public readonly string Mode = null!;
    
    public readonly string[] Modifiers = [];

    [JsonIgnore] public string ProgressionTrack { get; init; } = null!;
    
    [JsonIgnore] public string[] AllowedModifiers { get; init; } = null!;
    
    [JsonIgnore] public BossProgression Progression => DataBase.Progressions[ProgressionTrack];
    
    public Difficulty()
    { }
    
    public Difficulty(string name, string[] modifiers)
    {
        Mode = name;
        Modifiers = modifiers;
    }

    private static Difficulty Make(string key, string name, BossProgression progression, string[] allowedModifiers)
    {
        Difficulty difficulty = new Difficulty(name, null!)
        {
            ProgressionTrack = progression.Identifier,
            AllowedModifiers = allowedModifiers
        };
        
        DataBase.Difficulties.Add(key, difficulty);
        return difficulty;
    }

    public static readonly Difficulty Revengeance = Make("r", nameof(Revengeance), BossProgression.Calamity, Modifier.DefaultAllowed);
    
    public static readonly Difficulty Death = Make("d", nameof(Death), BossProgression.Calamity, Modifier.DefaultAllowed);
    
    public static readonly Difficulty Malice = Make("m", nameof(Malice), BossProgression.Calamity, Modifier.DefaultAllowed);
    
    public static readonly Difficulty Infernum = Make("i", nameof(Infernum), BossProgression.Infernum, Modifier.DefaultAllowed);

    public static readonly Difficulty Empyreal = Make("e", nameof(Empyreal), BossProgression.Empyreal, Modifier.DefaultAllowed);
    
    public static readonly Difficulty Thorium = Make("t", nameof(Thorium), BossProgression.Thorium, []);

    public static class Modifier
    {
        private static string Make(string key, string name)
        {
            DataBase.DifficultyModifiers.Add(key, name);
            return name;
        }

        public static readonly string Defiled = Make("d", nameof(Defiled));
        
        public static readonly string Shroomed = Make("s", nameof(Shroomed));

        public static readonly string[] DefaultAllowed = [Defiled, Shroomed];
    }
}