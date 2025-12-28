using NohitBot.Database;

namespace NohitBot.DataStructures;

public struct Verification(VerificationStatus reviewStatus)
{
    public VerificationStatus ReviewStatus { get; private set; } = reviewStatus;

    public ulong? JudgeID { get; private set; } = null;

    public string? JudgeComment { get; private set; } = null;

    public static Verification Default => new(VerificationStatus.Submitted);

    public void SetVerification(VerificationStatus reviewStatus, ulong judgeID, string comment)
    {
        ReviewStatus = reviewStatus;
        JudgeID = judgeID;
        JudgeComment = comment;
        DataBase.Save();
    }

    public void UpdateComment(string comment)
    {
        JudgeComment = comment;
        DataBase.Save();
    }
}

public enum VerificationStatus
{
    Submitted,
    DQ,
    Verified
}