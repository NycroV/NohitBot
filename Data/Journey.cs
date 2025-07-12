using Newtonsoft.Json;

namespace NohitBot.Data;

[JsonObject(MemberSerialization.OptOut)]
public class Journey
{
    public ulong UserID { get; init; }
    
    public string? PlaylistUrl { get; set; } = null;

    public Difficulty Difficulty { get; init; }

    public Dictionary<Boss, Nohit> Nohits { get; init; } = [];

    public Dictionary<Boss, List<Nohit>> OldNohits { get; init; } = [];

    [JsonIgnore] public bool Complete => Difficulty.Progression.RequiredBosses.All(BossVerified);

    private Journey()
    { }

    public Journey(ulong userId, Difficulty difficulty) : this()
    {
        UserID = userId;
        Difficulty = difficulty;
    }
    
    public bool BossVerified(Boss boss)
    {
        return Nohits.TryGetValue(boss, out Nohit? nohit) &&
           (nohit.Verification.ReviewStatus == VerificationStatus.Verified ||
           (OldNohits.TryGetValue(boss, out var oldNohits) && oldNohits.Any(oldNohit => oldNohit.Verification.ReviewStatus == VerificationStatus.Verified)));
    }
}