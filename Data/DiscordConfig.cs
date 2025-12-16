using System.Collections.Frozen;
using NohitBot.Database;

namespace NohitBot.Data;

public class DiscordConfig
{
    public ulong SubmissionChannelId { get; set; } = 0uL;
    
    public ulong LogChannelId { get; set; } = 0uL;
    
    public ulong JourneyChannelId { get; set; } = 0uL;

    private List<ulong> judgeIds { get; init; } = [];
    
    public FrozenSet<ulong> JudgeIds => judgeIds.ToFrozenSet();

    private DiscordConfig() { }

    private DiscordConfig(ulong submissionChannelId, ulong logChannelId, ulong journeyChannelId)
    {
        SubmissionChannelId = submissionChannelId;
        LogChannelId = logChannelId;
        JourneyChannelId = journeyChannelId;
    }

    public static DiscordConfig Make(ulong guildId, ulong submissionChannelId, ulong logChannelId, ulong journeyChannelId)
    {
        DiscordConfig config = new(submissionChannelId, logChannelId, journeyChannelId);
        DataBase.DiscordConfigs.Add(guildId, config);
        DataBase.Save();
        return config;
    }

    public void AddJudge(ulong judgeId)
    {
        judgeIds.Add(judgeId);
        DataBase.Save();
    }

    public bool RemoveJudge(ulong judgeId)
    {
        if (!judgeIds.Remove(judgeId))
            return false;
        
        DataBase.Save();
        return true;

    }
}