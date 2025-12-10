using System.Collections.Frozen;
using NohitBot.Database;

namespace NohitBot.Data;

public class DiscordConfiguration
{
    public ulong SubmissionChannelId { get; set; } = 0uL;
    
    public ulong LogChannelId { get; set; } = 0uL;
    
    public ulong JourneyChannelId { get; set; } = 0uL;

    private List<ulong> judgeIds { get; init; } = null!;
    
    public FrozenSet<ulong> JudgeIds => judgeIds.ToFrozenSet();

    private DiscordConfiguration(ulong submissionChannelId, ulong logChannelId, ulong journeyChannelId)
    {
        SubmissionChannelId = submissionChannelId;
        LogChannelId = logChannelId;
        JourneyChannelId = journeyChannelId;
    }

    public static DiscordConfiguration Make(ulong guildId, ulong submissionChannelId, ulong logChannelId, ulong journeyChannelId)
    {
        DiscordConfiguration configuration = new(submissionChannelId, logChannelId, journeyChannelId);
        DataBase.DiscordConfigurations.Add(guildId, configuration);
        DataBase.Save();
        return configuration;
    }
}