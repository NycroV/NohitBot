using NohitBot.Database;

namespace NohitBot.Data;

public class DiscordConfiguration
{
    public ulong SubmissionChannelId { get; set; } = 0uL;
    
    public ulong LogChannelId { get; set; } = 0uL;
    
    public ulong JourneyChannelId { get; set; } = 0uL;
    
    public readonly Dictionary<Boss, List<string>> BossAliases = [];

    public readonly List<ulong> JudgeIDs = [];

    private DiscordConfiguration(ulong submissionChannelId, ulong logChannelId, ulong journeyChannelId)
    {
        SubmissionChannelId = submissionChannelId;
        LogChannelId = logChannelId;
        JourneyChannelId = journeyChannelId;
    }

    public DiscordConfiguration Make(ulong guildId, ulong submissionChannelId, ulong logChannelId, ulong journeyChannelId)
    {
        if (DataBase.DiscordConfigurations.TryGetValue(guildId, out var configuration))
            return configuration;
        
        configuration = new(submissionChannelId, logChannelId, journeyChannelId);
        DataBase.DiscordConfigurations.Add(guildId, configuration);
        return configuration;
    }
}