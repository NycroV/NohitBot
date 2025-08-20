using System.Collections.Frozen;
using NohitBot.Database;

using static NohitBot.Data.Boss.VanillaTerraria;
using static NohitBot.Data.Boss.CalamityMod;
using static NohitBot.Data.Boss.CalamityEmpyreal;
using static NohitBot.Data.Boss.InfernumMod;
using static NohitBot.Data.Boss.ThoriumMod;

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
        BossProgression progression = new(identifier, bosses, optionalBosses);
        Registry.Add(identifier, progression);
        return progression;
    }

    public static readonly Dictionary<string, BossProgression> Registry = [];

    public static readonly BossProgression Calamity = Make(nameof(Calamity), [
        KingSlime, DesertScourge, EyeOfCthulhu, Crabulon, EaterOfWorlds, BrainOfCthulhu, HiveMind, Perforators, QueenBee, Deerclops, Skeletron, SlimeGod, WallOfFlesh,
        QueenSlime, Cryogen, Destroyer, AquaticScourge, Twins, BrimstoneElemental, SkeletronPrime, CalamitasClone, Plantera, LeviathanAndAnahita, AstrumAureus, Golem, PlaguebringerGoliath, EmpressOfLight, DukeFishron, Ravager, LunaticCultist, AstrumDeus, MoonLord,
        ProfanedGuardians, Dragonfolly, Providence, StormWeaver, CeaselessVoid, Signus, Polterghast, OldDuke, DevourerOfGods, Yharon, ExoMechs, SupremeCalamitas, BossRush],
        [Deerclops, BossRush]);
    
    public static readonly BossProgression Infernum = Make(nameof(Infernum), [
            KingSlime, DesertScourge, EyeOfCthulhu, Crabulon, EaterOfWorlds, BrainOfCthulhu, HiveMind, Perforators, QueenBee, Deerclops, Skeletron, SlimeGod, WallOfFlesh,
            Dreadnautilus, QueenSlime, Cryogen, Destroyer, AquaticScourge, Twins, BrimstoneElemental, SkeletronPrime, CalamitasClone, Plantera, LeviathanAndAnahita, AstrumAureus, Golem, PlaguebringerGoliath, EmpressOfLight, DukeFishron, Ravager, LunaticCultist, BereftVassal, AstrumDeus, MoonLord,
            ProfanedGuardians, Dragonfolly, Providence, StormWeaver, CeaselessVoid, Signus, Polterghast, OldDuke, DevourerOfGods, Yharon, AdultEidolonWyrm, ExoMechs, SupremeCalamitas],
        [Deerclops, BossRush]);
    
    public static readonly BossProgression Empyreal = Make(nameof(Empyreal), [
        AncientDoomsayer, DevourerOfUniverses, DragonGodYharon], []);
    
    public static readonly BossProgression Thorium = Make(nameof(Thorium), [
        GrandThunderBird, QueenJellyfish, Viscount, GraniteEnergyStorm, BuriedChampion, StarScouter, BoreanStrider, Coznix, Lich, Abyssion, Primordials], []);
}