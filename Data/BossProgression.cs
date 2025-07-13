using System.Collections.Frozen;
using NohitBot.Database;

using static NohitBot.Data.Boss.Vanilla;
using static NohitBot.Data.Boss.Calamity;
using static NohitBot.Data.Boss.Empyreal;
using static NohitBot.Data.Boss.Infernum;
using static NohitBot.Data.Boss.Thorium;

namespace NohitBot.Data;

public class BossProgression
{
    public string Identifier { get; init; }
    
    public FrozenSet<Boss> Bosses { get; init; }
    
    public FrozenSet<Boss> RequiredBosses { get; init; }
    
    public FrozenSet<Boss> OptionalBosses { get; init; }
    
    private BossProgression(string identifier, IEnumerable<Boss> bosses, IEnumerable<Boss>? optionalBosses = null)
    {
        Identifier = identifier;
        Bosses = bosses.ToFrozenSet();
        OptionalBosses = optionalBosses?.ToFrozenSet() ?? [];
        RequiredBosses = Bosses.Except(OptionalBosses).ToFrozenSet();
    }

    private static BossProgression Make(string identifier, IEnumerable<Boss> bosses, IEnumerable<Boss>? optionalBosses = null)
    {
        if (DataBase.Progressions.TryGetValue(identifier, out var progression))
            return progression;
        
        progression = new(identifier, bosses, optionalBosses);
        DataBase.Progressions.Add(identifier, progression);
        return progression;
    }

    public static readonly BossProgression Calamity = Make(nameof(Calamity), [
        KingSlime, DesertScourge, EyeOfCthulhu, Crabulon, EaterOfWorlds, BrainOfCthulhu, HiveMind, Perforators, QueenBee, Deerclops, Skeletron, SlimeGod, WallOfFlesh,
        QueenSlime, Cryogen, Destroyer, AquaticScourge, Twins, BrimstoneElemental, SkeletronPrime, CalamitasClone, Plantera, LeviathanAndAnahita, AstrumAureus, Golem, PlaguebringerGoliath, EmpressOfLight, DukeFishron, Ravager, LunaticCultist, AstrumDeus, MoonLord,
        ProfanedGuardians, Dragonfolly, Providence, StormWeaver, CeaselessVoid, Signus, Polterghast, OldDuke, DevourerOfGods, Yharon, ExoMechs, SupremeCalamitas],
        [Deerclops]);
    
    public static readonly BossProgression Infernum = Make(nameof(Infernum), [
            KingSlime, DesertScourge, EyeOfCthulhu, Crabulon, EaterOfWorlds, BrainOfCthulhu, HiveMind, Perforators, QueenBee, Deerclops, Skeletron, SlimeGod, WallOfFlesh,
            Dreadnautilus, QueenSlime, Cryogen, Destroyer, AquaticScourge, Twins, BrimstoneElemental, SkeletronPrime, CalamitasClone, Plantera, LeviathanAndAnahita, AstrumAureus, Golem, PlaguebringerGoliath, EmpressOfLight, DukeFishron, Ravager, LunaticCultist, BereftVassal, AstrumDeus, MoonLord,
            ProfanedGuardians, Dragonfolly, Providence, StormWeaver, CeaselessVoid, Signus, Polterghast, OldDuke, DevourerOfGods, Yharon, AdultEidolonWyrm, ExoMechs, SupremeCalamitas],
        [Deerclops]);
    
    public static readonly BossProgression Empyreal = Make(nameof(Empyreal), [], []);
    
    public static readonly BossProgression Thorium = Make(nameof(Thorium), [], []);
}