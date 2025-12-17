using System.Collections.Frozen;
using DSharpPlus.Entities;
using NohitBot.Database;

namespace NohitBot.DataStructures;

public class DiscordConfig
{
    public ulong SubmissionChannelId { get; private set; } = 0uL;
    
    public ulong LogChannelId { get; private set; } = 0uL;
    
    public ulong JourneyChannelId { get; private set; } = 0uL;

    public KeyValuePair<ulong, ulong>? JudgeInfoPinId { get; private set; } = null;

    public KeyValuePair<ulong, ulong>? JourneyTrackingPinId { get; private set; } = null;

    private List<ulong> judgeIds { get; init; } = [];
    
    public FrozenSet<ulong> JudgeIds => judgeIds.ToFrozenSet();

    public string? DocMessage { get; private set; } = null;

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

    public void SetChannels(ulong? submissionId = null, ulong? logId = null, ulong? journeyId = null)
    {
        bool save = false;

        if (submissionId != null)
        {
            SubmissionChannelId = submissionId.Value;
            save = true;
        }

        if (logId != null)
        {
            LogChannelId = logId.Value;
            save = true;
        }

        if (journeyId != null)
        {
            JourneyChannelId = journeyId.Value;
            save = true;
        }
        
        if (save)
            DataBase.Save();
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

    public void SetJudgeInfoPin()
    {
        JudgeInfoPinId = null;
        DataBase.Save();
    }
    
    public void SetJudgeInfoPin(ulong channelId, ulong messageId)
    {
        JudgeInfoPinId = new(channelId, messageId);
        DataBase.Save();
    }

    public void SetJourneyTrackingPin()
    {
        JourneyTrackingPinId = null;
        DataBase.Save();
    }
    
    public void SetJourneyTrackingPin(ulong channelId, ulong messageId)
    {
        JourneyTrackingPinId = new(channelId, messageId);
        DataBase.Save();
    }

    public async Task UpdateJudgeInfoPin()
    {
        
    }

    public async Task UpdateJourneyTrackingInfoPin()
    {
        
    }

    public void SetDocMessage(string message)
    {
        DocMessage = message;
        DataBase.Save();
    }
}