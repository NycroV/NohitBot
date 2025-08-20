namespace NohitBot.Data;

public class DiscordConfiguration
{
    public ulong SubmissionChannelId { get; set; } = 0uL;
    
    public ulong LogChannelId { get; set; } = 0uL;
    
    public ulong JourneyChannelId { get; set; } = 0uL;
    
    public readonly Dictionary<Boss, List<string>> BossAliases = [];

    public readonly List<ulong> JudgeIDs = [];

    public DiscordConfiguration(ulong submissionChannelId, ulong logChannelId, ulong journeyChannelId)
    {
        SubmissionChannelId = submissionChannelId;
        LogChannelId = logChannelId;
        JourneyChannelId = journeyChannelId;
    }
}