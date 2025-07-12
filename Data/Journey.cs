using Newtonsoft.Json;

namespace NohitBot.Data;

[JsonObject(MemberSerialization.OptOut)]
public class Journey(ulong userId, BossProgression bossProgression)
{
    public ulong UserID { get; } = userId;
    
    public string? PlaylistUrl { get; set; } = null;

    public BossProgression BossProgression { get; } = bossProgression;

    public Dictionary<Boss, Nohit> Nohits { get; } = [];

    public Dictionary<Boss, List<Nohit>> OldNohits { get; } = [];

    [JsonIgnore] public bool Complete => BossProgression.RequiredBosses.All(BossVerified);

    public bool BossVerified(Boss boss)
    {
        return Nohits.TryGetValue(boss, out Nohit? nohit) &&
           (nohit.Verification.ReviewStatus == VerificationStatus.Verified ||
           (OldNohits.TryGetValue(boss, out var oldNohits) && oldNohits.Any(oldNohit => oldNohit.Verification.ReviewStatus == VerificationStatus.Verified)));
    }
}