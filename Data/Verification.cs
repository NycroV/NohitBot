namespace NohitBot.Data;

public class Verification(VerificationStatus reviewStatus)
{
    public VerificationStatus ReviewStatus { get; set; } = reviewStatus;

    public ulong? JudgeID { get; set; } = null;

    public string? JudgeComment { get; set; } = null;

    public static readonly Verification Default = new(VerificationStatus.Submitted);
}

public enum VerificationStatus
{
    Submitted,
    DQ,
    Verified
}