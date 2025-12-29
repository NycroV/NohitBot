using System.Collections.ObjectModel;
using Newtonsoft.Json;
using NohitBot.Database;

namespace NohitBot.DataStructures;

public class DiscordConfig
{
    private DiscordConfig()
    {
    }

    private DiscordConfig(ulong submissionChannelId, ulong logChannelId, ulong journeyChannelId)
    {
        SubmissionChannelId = submissionChannelId;
        LogChannelId = logChannelId;
        JourneyChannelId = journeyChannelId;
    }

    public ulong SubmissionChannelId { get; private set; }

    public ulong LogChannelId { get; private set; }

    public ulong JourneyChannelId { get; private set; }

    public KeyValuePair<ulong, ulong>? JudgeInfoPinId { get; private set; }

    public KeyValuePair<ulong, ulong>? JourneyTrackingPinId { get; private set; }

    private List<ulong> judgeIds { get; } = [];

    [JsonIgnore] public ReadOnlyCollection<ulong> JudgeIds => judgeIds.AsReadOnly();

    private Dictionary<ulong, List<Journey>> ignoredJourneys { get; } = [];

    [JsonIgnore] public ReadOnlyDictionary<ulong, ReadOnlyCollection<Journey>> IgnoredJourneys => ignoredJourneys.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.AsReadOnly()).AsReadOnly();

    public string? DocMessage { get; private set; }

    private List<Journey> journeyQueue { get; } = [];

    [JsonIgnore] public ReadOnlyCollection<Journey> JourneyQueue => journeyQueue.AsReadOnly();

    public static DiscordConfig Make(ulong guildId, ulong submissionChannelId, ulong logChannelId, ulong journeyChannelId)
    {
        DiscordConfig config = new(submissionChannelId, logChannelId, journeyChannelId);
        DataBase.DiscordConfigs.Add(guildId, config);
        DataBase.Save();
        return config;
    }

    public void SetChannels(ulong? submissionId = null, ulong? logId = null, ulong? journeyId = null)
    {
        var save = false;

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

    /// <returns>True if the journey is now being ignored; otherwise false</returns>
    public bool ToggleIgnoreJourney(ulong userId, Journey journey)
    {
        if (!ignoredJourneys.TryGetValue(userId, out var journeys))
        {
            ignoredJourneys.Add(userId, [journey]);
            DataBase.Save();
            return true;
        }

        if (journeys.Remove(journey))
        {
            DataBase.Save();
            return false;
        }

        journeys.Add(journey);
        DataBase.Save();
        return true;
    }

    public void QueueJourney(Journey journey)
    {
        if (journeyQueue.Contains(journey))
            return;

        journeyQueue.Add(journey);
        DataBase.Save();
    }

    public void DequeueJourney(Journey journey)
    {
        journeyQueue.Remove(journey);
        DataBase.Save();
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