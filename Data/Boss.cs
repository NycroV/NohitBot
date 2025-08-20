using Newtonsoft.Json;
using NohitBot.Database;

namespace NohitBot.Data;

[JsonObject(MemberSerialization.OptOut)]
public class Boss
{
    public string Name { get; } = "";

    private Boss()
    { }

    private Boss(string name) : this()
    {
        Name = name;
    }

    private static Boss Make(string name)
    {
        if (DataBase.Bosses.TryGetValue(name, out var boss))
            return boss;

        boss = new(name);
        DataBase.Bosses.Add(name, boss);
        return boss;
    }
    
    #region Bosses

    public static class VanillaTerraria
    {
        public static readonly Boss KingSlime = Make(nameof(KingSlime));

        public static readonly Boss EyeOfCthulhu = Make(nameof(EyeOfCthulhu));

        public static readonly Boss EaterOfWorlds = Make(nameof(EaterOfWorlds));

        public static readonly Boss BrainOfCthulhu = Make(nameof(BrainOfCthulhu));

        public static readonly Boss QueenBee = Make(nameof(QueenBee));

        public static readonly Boss Deerclops = Make(nameof(Deerclops));

        public static readonly Boss Skeletron = Make(nameof(Skeletron));

        public static readonly Boss WallOfFlesh = Make(nameof(WallOfFlesh));
        
        public static readonly Boss QueenSlime = Make(nameof(QueenSlime));
        
        public static readonly Boss Destroyer = Make(nameof(Destroyer));
        
        public static readonly Boss Twins = Make(nameof(Twins));
        
        public static readonly Boss SkeletronPrime = Make(nameof(SkeletronPrime));
        
        public static readonly Boss Plantera = Make(nameof(Plantera));
        
        public static readonly Boss Golem = Make(nameof(Golem));
        
        public static readonly Boss EmpressOfLight = Make(nameof(EmpressOfLight));
        
        public static readonly Boss DukeFishron = Make(nameof(DukeFishron));
        
        public static readonly Boss LunaticCultist = Make(nameof(LunaticCultist));
        
        public static readonly Boss MoonLord = Make(nameof(MoonLord));
    }

    public static class CalamityMod
    {
        public static readonly Boss DesertScourge = Make(nameof(DesertScourge));
        
        public static readonly Boss Crabulon = Make(nameof(Crabulon));
        
        public static readonly Boss HiveMind = Make(nameof(HiveMind));
        
        public static readonly Boss Perforators = Make(nameof(Perforators));
        
        public static readonly Boss SlimeGod = Make(nameof(SlimeGod));
        
        public static readonly Boss Cryogen = Make(nameof(Cryogen));
        
        public static readonly Boss AquaticScourge = Make(nameof(AquaticScourge));
        
        public static readonly Boss BrimstoneElemental = Make(nameof(BrimstoneElemental));
        
        public static readonly Boss CalamitasClone = Make(nameof(CalamitasClone));
        
        public static readonly Boss LeviathanAndAnahita = Make(nameof(LeviathanAndAnahita));
        
        public static readonly Boss AstrumAureus = Make(nameof(AstrumAureus));
        
        public static readonly Boss PlaguebringerGoliath = Make(nameof(PlaguebringerGoliath));
        
        public static readonly Boss Ravager = Make(nameof(Ravager));
        
        public static readonly Boss AstrumDeus = Make(nameof(AstrumDeus));
        
        public static readonly Boss ProfanedGuardians = Make(nameof(ProfanedGuardians));
        
        public static readonly Boss Dragonfolly = Make(nameof(Dragonfolly));
        
        public static readonly Boss Providence = Make(nameof(Providence));
        
        public static readonly Boss StormWeaver = Make(nameof(StormWeaver));
        
        public static readonly Boss CeaselessVoid = Make(nameof(CeaselessVoid));
        
        public static readonly Boss Signus = Make(nameof(Signus));
        
        public static readonly Boss Polterghast = Make(nameof(Polterghast));
        
        public static readonly Boss OldDuke = Make(nameof(OldDuke));
        
        public static readonly Boss DevourerOfGods = Make(nameof(DevourerOfGods));
        
        public static readonly Boss Yharon = Make(nameof(Yharon));
        
        public static readonly Boss ExoMechs = Make(nameof(ExoMechs));
        
        public static readonly Boss SupremeCalamitas = Make(nameof(SupremeCalamitas));
        
        public static readonly Boss BossRush = Make(nameof(BossRush));
    }

    public static class InfernumMod
    {
        public static readonly Boss Dreadnautilus = Make(nameof(Dreadnautilus));
        
        public static readonly Boss BereftVassal = Make(nameof(BereftVassal));

        public static readonly Boss AdultEidolonWyrm = Make(nameof(AdultEidolonWyrm));
    }

    public static class CalamityEmpyreal
    {
        public static readonly Boss AncientDoomsayer = Make(nameof(AncientDoomsayer));
        
        public static readonly Boss DevourerOfUniverses = Make(nameof(DevourerOfUniverses));
        
        public static readonly Boss DragonGodYharon = Make(nameof(DragonGodYharon));
    }

    public static class ThoriumMod
    {
        public static readonly Boss GrandThunderBird = Make(nameof(GrandThunderBird));
        
        public static readonly Boss QueenJellyfish = Make(nameof(QueenJellyfish));
        
        public static readonly Boss Viscount = Make(nameof(Viscount));
        
        public static readonly Boss GraniteEnergyStorm = Make(nameof(GraniteEnergyStorm));
        
        public static readonly Boss BuriedChampion = Make(nameof(BuriedChampion));
        
        public static readonly Boss StarScouter = Make(nameof(StarScouter));
        
        public static readonly Boss BoreanStrider = Make(nameof(BoreanStrider));
        
        public static readonly Boss Coznix = Make(nameof(Coznix));
        
        public static readonly Boss Lich = Make(nameof(Lich));
        
        public static readonly Boss Abyssion = Make(nameof(Abyssion));
        
        public static readonly Boss Primordials = Make(nameof(Primordials));
    }

    #endregion
}