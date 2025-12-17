using System.Collections.Frozen;
using Newtonsoft.Json;
using NohitBot.Database;

namespace NohitBot.DataStructures;

public class Journey
{
    public ulong UserID { get; init; }
    
    public string? PlaylistUrl { get; private set; } = null;

    public Difficulty Difficulty { get; init; }

    [JsonIgnore]
    public FrozenDictionary<Boss, Nohit> Nohits
    {
        get
        {
            Dictionary<Boss, Nohit> nohits = [];

            foreach (Nohit nohit in DataBase.Nohits.Values.Where(n => n.Difficulty == Difficulty))
                nohits[nohit.Boss] = nohit;

            return nohits.ToFrozenDictionary();
        }
    }
    
    [JsonIgnore]
    public FrozenDictionary<Boss, List<Nohit>> OldNohits
    {
        get
        {
            Dictionary<Boss, List<Nohit>> nohits = [];
            Dictionary<Boss, Nohit> newerNohits = [];

            foreach (Nohit nohit in DataBase.Nohits.Values.Where(n => n.Difficulty == Difficulty))
            {
                if (newerNohits.TryGetValue(nohit.Boss, out var newerNohit))
                {
                    nohits.TryAdd(nohit.Boss, []);
                    nohits[nohit.Boss].Add(newerNohit);
                }

                newerNohits[nohit.Boss] = nohit;
            }
            
            return nohits.ToFrozenDictionary();
        }
    }

    [JsonIgnore] public bool Complete => Difficulty.Mode.Progression.RequiredBosses.All(BossVerified);

    private Journey() { }

    public Journey(ulong userId, Difficulty difficulty)
    {
        UserID = userId;
        Difficulty = difficulty;
    }
    
    public bool BossVerified(Boss boss)
    {
        return
            (Nohits.TryGetValue(boss, out Nohit? nohit) && (nohit.Verification.ReviewStatus == VerificationStatus.Verified)) || 
            (OldNohits.TryGetValue(boss, out var oldNohits) && oldNohits.Any(oldNohit => oldNohit.Verification.ReviewStatus == VerificationStatus.Verified));
    }
}