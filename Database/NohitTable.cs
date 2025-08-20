using Newtonsoft.Json;
using NohitBot.Data;

namespace NohitBot.Database;

public partial class DataBase
{
    [JsonProperty] private readonly Dictionary<uint, Nohit> nohitRegistry = [];
    
    public static Dictionary<uint, Nohit> Nohits => instance.nohitRegistry;
    
    public static void RegisterNohit(Nohit nohit)
    {
        nohit.ID = (uint)Nohits.Count;
        Nohits.Add(nohit.ID, nohit);

        Journeys.TryAdd(nohit.UserID, []);
        var userJourneys = Journeys[nohit.UserID];

        userJourneys.TryAdd(nohit.Difficulty, new(nohit.UserID, nohit.Difficulty));
        Journey journey = userJourneys[nohit.Difficulty];

        if (journey.Nohits.TryGetValue(nohit.Boss, out Nohit? oldNohit))
        {
            journey.OldNohits.TryAdd(nohit.Boss, []);
            journey.OldNohits[nohit.Boss].Add(oldNohit);
        }
        
        journey.Nohits[nohit.Boss] = nohit;
        Save();
    }
}