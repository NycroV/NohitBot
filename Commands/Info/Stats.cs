using DSharpPlus.Commands;
using DSharpPlus.Entities;
using NohitBot.Database;
using NohitBot.DataStructures;

namespace NohitBot.Commands.Info;

public class Stats
{
    [Command(nameof(Stats))]
    public async ValueTask StatsAsync(CommandContext ctx)
    {
        var nohits = DataBase.Nohits.Values;
        var journeys = DataBase.Journeys.Values.SelectMany(j => j.Values).ToArray();
        
        int totalNohits = nohits.Count(n => !n.Deleted);
        int totalVerified = nohits.Count(n => n.Verification.ReviewStatus == VerificationStatus.Verified);
        int totalDQd = nohits.Count(n => n.Verification.ReviewStatus == VerificationStatus.DQ);
        int totalPending = totalNohits - totalVerified - totalDQd;
        int totalEntries = DataBase.Nohits.Count;
        int totalNohitters = nohits.Select(n => n.UserID).Distinct().Count() - 1; // -1 for 0uL / deleted nohits
        
        TimeSpan timeSinceFirst = DateTime.Now - nohits.First(n => !n.Deleted).TimeStamp;
        float avgPerDay = (float)Math.Round(totalNohits / timeSinceFirst.TotalDays, 2);
        float hoursFootage = (float)Math.Round(totalNohits * 90f / 3600f, 2);

        int completedJourneys = journeys.Count(j => j.Complete);
        int incompleteJourneys = journeys.Length - completedJourneys;
        double avgCompletion = journeys
            .Average(j => (float)j.Nohits.Count(n => n.Value.Verification.ReviewStatus != VerificationStatus.DQ) / 
                j.Difficulty.Mode.Progression.RequiredBosses.Count);

        var embed = new DiscordEmbedBuilder()
            .WithTitle("Nohit Submission Stats")
            .WithColor(DiscordColor.Purple)
            .WithDescription("Statistic for the current nohit databse");

        embed.AddField("Bot Stats",
            $"**{timeSinceFirst.Days} days, {timeSinceFirst.Hours} hours, {timeSinceFirst.Minutes}, and {timeSinceFirst.Seconds} seconds** since the first nohit was submitted\n" +
            $"------------------------------\n" +
            $"There has been an average of **{avgPerDay}** nohits submitted per-day\n" +
            $"------------------------------\n" +
            $"Estimating 1.5 minutes per-nohit, this is well over **{hoursFootage} hours** of footage!\n" +
            $"------------------------------\n" +
            $"**{totalNohitters}** users have submitted at least 1 nohit to the bot" +
            $"------------------------------\n", true);
        
        embed.AddField("Submission Stats",
            $"**{totalNohits}** total nohits\n" +
            $"------------------------------\n" +
            $"*{totalEntries}* entries - {totalEntries - totalNohits} deleted*\n" +
            $"------------------------------\n" +
            $"**{totalVerified}** verified nohits,\n" +
            $"**{totalDQd}** DQ'd nohits, and\n" +
            $"**{totalPending}** pending nohits\n" +
            $"------------------------------\n", true);

        embed.AddField("Journey Stats",
            $"**{completedJourneys}** journeys have been completed\n" +
            $"------------------------------\n" +
            $"There is an average journey completion rate of **{avgCompletion}%**\n" +
            $"------------------------------\n", true);
        
        await ctx.RespondAsync(embed);
    }
}