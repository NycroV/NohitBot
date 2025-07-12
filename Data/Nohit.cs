using Newtonsoft.Json;
using NohitBot.Registries;

namespace NohitBot.Data;

[JsonObject(MemberSerialization.OptOut)]
public class Nohit(ulong userId, Boss boss, BossProgression bossProgression, string url)
{
    public uint ID { get; set; } = 0;
    public ulong UserID { get; } = userId;
    
    public Boss Boss { get; } = boss;
    public BossProgression BossProgression { get; } = bossProgression;
    [JsonIgnore] public Journey Journey => NohitTable.Journeys[UserID][BossProgression];

    public string Url { get; } = url;
    public DateTime TimeStamp { get; } = DateTime.UtcNow;
    
    public string? UserComment { get; private set; } = null;
    public Verification Verification { get; private set; } = Verification.Default;

    public void Review(VerificationStatus verdict, ulong judgeId, string? judgeComment = null)
    {
        Verification.ReviewStatus = verdict;
        Verification.JudgeID = judgeId;
        Verification.JudgeComment = judgeComment;
        NohitTable.Save();
    }

    public void Recomment(string? userComment)
    {
        UserComment = userComment;
        NohitTable.Save();
    }
    
    public void JudgeRecomment(string? judgeComment)
    {
        Verification.JudgeComment = judgeComment;
        NohitTable.Save();
    }
}