using Newtonsoft.Json;
using NohitBot.Database;

namespace NohitBot.Data;

[JsonObject(MemberSerialization.OptOut)]
public class Boss()
{
    public string Name { get; } = "";
    
    public List<string> Aliases { get; } = [];
    
    public TimeSpan MinimumNohitLength { get; set; } = TimeSpan.Zero;

    private Boss(string name) : this()
    {
        Name = name;
    }

    public static Boss Make(string name)
    {
        if (DataBase.Bosses.TryGetValue(name, out var boss))
            return boss;

        boss = new(name);
        DataBase.Bosses.Add(name, boss);
        return boss;
    }
    
    #region Bosses

    public static class Vanilla
    {
        public static readonly Boss KingSlime = Make(nameof(KingSlime).ToLower());

        public static readonly Boss EyeOfCthulhu = Make(nameof(EyeOfCthulhu).ToLower());

        public static readonly Boss EaterOfWorld = Make(nameof(EaterOfWorld).ToLower());

        public static readonly Boss BrainOfCthulhu = Make(nameof(BrainOfCthulhu).ToLower());

        public static readonly Boss QueenBee = Make(nameof(QueenBee).ToLower());

        public static readonly Boss Deerclops = Make(nameof(Deerclops).ToLower());

        public static readonly Boss Skeletron = Make(nameof(Skeletron).ToLower());

        public static readonly Boss WallOfFlesh = Make(nameof(WallOfFlesh).ToLower());
        
        public static readonly Boss QueenSlime = Make(nameof(QueenSlime).ToLower());
        
        public static readonly Boss Destroyer = Make(nameof(Destroyer).ToLower());
        
        public static readonly Boss Twins = Make(nameof(Twins).ToLower());
        
        public static readonly Boss SkeletronPrime = Make(nameof(SkeletronPrime).ToLower());
        
        public static readonly Boss Plantera = Make(nameof(Plantera).ToLower());
        
        public static readonly Boss Golem = Make(nameof(Golem).ToLower());
        
        public static readonly Boss EmpressOfLight = Make(nameof(EmpressOfLight).ToLower());
        
        public static readonly Boss DukeFishron = Make(nameof(DukeFishron).ToLower());
        
        public static readonly Boss LunaticCultist = Make(nameof(LunaticCultist).ToLower());
        
        public static readonly Boss MoonLord = Make(nameof(MoonLord).ToLower());
    }

    public static class Calamity
    {
        public static readonly Boss DesertScourge = Make(nameof(DesertScourge).ToLower());
        
        public static readonly Boss Crabulon = Make(nameof(Crabulon).ToLower());
        
        public static readonly Boss HiveMind = Make(nameof(HiveMind).ToLower());
        
        public static readonly Boss Perforators = Make(nameof(Perforators).ToLower());
        
        public static readonly Boss SlimeGod = Make(nameof(SlimeGod).ToLower());
        
        public static readonly Boss Cryogen = Make(nameof(Cryogen).ToLower());
        
        public static readonly Boss AquaticScourge = Make(nameof(AquaticScourge).ToLower());
        
        public static readonly Boss BrimstoneElemental = Make(nameof(BrimstoneElemental).ToLower());
        
        public static readonly Boss CalamitasClone = Make(nameof(CalamitasClone).ToLower());
        
        public static readonly Boss LeviathanAndAnahita = Make(nameof(LeviathanAndAnahita).ToLower());
        
        public static readonly Boss AstrumAureus = Make(nameof(AstrumAureus).ToLower());
        
        public static readonly Boss PlaguebringerGoliath = Make(nameof(PlaguebringerGoliath).ToLower());
        
        public static readonly Boss Ravager = Make(nameof(Ravager).ToLower());
        
        public static readonly Boss AstrumDeus = Make(nameof(AstrumDeus).ToLower());
        
        public static readonly Boss ProfanedGuardians = Make(nameof(ProfanedGuardians).ToLower());
        
        public static readonly Boss Dragonfolly = Make(nameof(Dragonfolly).ToLower());
        
        public static readonly Boss Providence = Make(nameof(Providence).ToLower());
        
        public static readonly Boss StormWeaver = Make(nameof(StormWeaver).ToLower());
        
        public static readonly Boss CeaselessVoid = Make(nameof(CeaselessVoid).ToLower());
        
        public static readonly Boss Signus = Make(nameof(DevourerOfGods).ToLower());
        
        public static readonly Boss Polterghast = Make(nameof(DevourerOfGods).ToLower());
        
        public static readonly Boss OldDuke = Make(nameof(DevourerOfGods).ToLower());
        
        public static readonly Boss DevourerOfGods = Make(nameof(DevourerOfGods).ToLower());
        
        public static readonly Boss Yharon = Make(nameof(Yharon).ToLower());
        
        public static readonly Boss ExoMechs = Make(nameof(ExoMechs).ToLower());
        
        public static readonly Boss SupremeCalamitas = Make(nameof(SupremeCalamitas).ToLower());
    }

    public static class Infernum
    {
        public static readonly Boss Dreadnautilus = Make(nameof(Dreadnautilus).ToLower());
        
        public static readonly Boss BereftVassal = Make(nameof(BereftVassal).ToLower());

        public static readonly Boss AdultEidolonWyrm = Make(nameof(AdultEidolonWyrm).ToLower());
    }

    public static class Empyreal
    {
        public static readonly Boss AncientDoomsayer = Make(nameof(AncientDoomsayer).ToLower());
        
        public static readonly Boss DevourerOfUniverses = Make(nameof(DevourerOfUniverses).ToLower());
        
        public static readonly Boss DragonGodYharon = Make(nameof(DragonGodYharon).ToLower());
    }

    public static class Thorium
    {
        public static readonly Boss GrandThunderBird = Make(nameof(GrandThunderBird).ToLower());
        
        public static readonly Boss QueenJellyfish = Make(nameof(QueenJellyfish).ToLower());
        
        public static readonly Boss Viscount = Make(nameof(Viscount).ToLower());
        
        public static readonly Boss GraniteEnergyStorm = Make(nameof(GraniteEnergyStorm).ToLower());
        
        public static readonly Boss BuriedChampion = Make(nameof(BuriedChampion).ToLower());
        
        public static readonly Boss StarScouter = Make(nameof(StarScouter).ToLower());
        
        public static readonly Boss BoreanStrider = Make(nameof(BoreanStrider).ToLower());
        
        public static readonly Boss Coznix = Make(nameof(Coznix).ToLower());
        
        public static readonly Boss Lich = Make(nameof(Lich).ToLower());
        
        public static readonly Boss Abyssion = Make(nameof(Abyssion).ToLower());
        
        public static readonly Boss Primordials = Make(nameof(Primordials).ToLower());
    }

    #endregion
}