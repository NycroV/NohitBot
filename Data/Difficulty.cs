using Newtonsoft.Json;
using NohitBot.Data;
using NohitBot.Database;

namespace NohitBot.Data;

public readonly struct Difficulty
{
    public readonly string Mode = null!;
    
    public readonly string[] Modifiers = null!;

    [JsonIgnore] public string ProgressionTrack { get; init; } = null!;
    
    [JsonIgnore] public Dictionary<string, string> AllowedModifiers { get; init; } = null!;
    
    [JsonIgnore] public BossProgression Progression => BossProgression.Registry[ProgressionTrack];
    
    private Difficulty(string name)
    {
        Mode = name;
    }

    private static Difficulty Make(string identifier, string name, BossProgression progression, Dictionary<string, string> allowedModifiers)
    {
        Difficulty difficulty = new Difficulty(name)
        {
            ProgressionTrack = progression.Identifier,
            AllowedModifiers = allowedModifiers
        };
        
        Registry.Add(identifier, difficulty);
        return difficulty;
    }

    public static readonly Dictionary<string, Difficulty> Registry = [];

    public static readonly Difficulty Revengeance = Make("r", nameof(Revengeance), BossProgression.Calamity, Modifier.DefaultAllowed);
    
    public static readonly Difficulty Death = Make("d", nameof(Death), BossProgression.Calamity, Modifier.DefaultAllowed);
    
    public static readonly Difficulty Malice = Make("m", nameof(Malice), BossProgression.Calamity, Modifier.DefaultAllowed);
    
    public static readonly Difficulty Infernum = Make("i", nameof(Infernum), BossProgression.Infernum, Modifier.DefaultAllowed);

    public static readonly Difficulty Empyreal = Make("e", nameof(Empyreal), BossProgression.Empyreal, Modifier.DefaultAllowed);
    
    public static readonly Difficulty Thorium = Make("t", nameof(Thorium), BossProgression.Thorium, Modifier.NoneAllowed);

    public static class Modifier
    {
        public static readonly KeyValuePair<string, string> Defiled = new("d", nameof(Defiled));
        
        public static readonly KeyValuePair<string, string> Shroomed = new("s", nameof(Shroomed));

        public static readonly Dictionary<string, string> DefaultAllowed = new([Defiled, Shroomed]);

        public static readonly Dictionary<string, string> NoneAllowed = [];
    }
}