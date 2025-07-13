using Newtonsoft.Json;
using NohitBot.Database;

namespace NohitBot.Data;

[JsonObject(MemberSerialization.OptOut)]
public class Boss()
{
    public string Name { get; } = "";
    
    public List<string> Aliases { get; } = [];
    
    public TimeSpan MinimumNohitLength { get; set; } = TimeSpan.Zero;

    [JsonIgnore] public int ProgressionWeight { get; private set; }

    private Boss(string name) : this()
    {
        Name = name;
    }

    private static int bossWeight = 0;
    
    private static Boss Make(string name, int? weightOverride = null)
    {
        if (!DataBase.Bosses.TryGetValue(name, out var boss))
        {
            boss = new(name);
            DataBase.Bosses.Add(name, boss);
        }
        
        boss.ProgressionWeight = weightOverride ?? (bossWeight++ << 24) + 0b00001000_00000000_00000000_00000000;
        return boss;
    }

    private static int GetShift(int weight)
    {
        int shift = 24;

        while (weight << (32 - shift) > 0 && shift > 0)
            shift -= 8;

        return shift;
    }
    
    private static Boss MakeSub(Boss other, string name)
    {
        int shift = GetShift(other.ProgressionWeight) - 8;

        if (shift <= 0)
            shift = 0;

        int weight = 1 << (shift + 3);
        return Make(name, other.ProgressionWeight + weight);
    }

    private static Boss MakeAfter(Boss other, string name) => Make(name, other.ProgressionWeight + (1 << GetShift(other.ProgressionWeight)));
    private static Boss MakeBefore(Boss other, string name) => Make(name, other.ProgressionWeight - (1 << GetShift(other.ProgressionWeight)));
    private static Boss MakeAlternate(Boss other, string name) => Make(name, other.ProgressionWeight);
    
    #region Bosses

    public static class Vanilla
    {
        public static readonly Boss KingSlime = Make(nameof(KingSlime));

        public static readonly Boss EyeOfCthulhu = Make(nameof(EyeOfCthulhu));

        public static readonly Boss EaterOfWorlds = Make(nameof(EaterOfWorlds));

        public static readonly Boss BrainOfCthulhu = MakeAlternate(EaterOfWorlds, nameof(BrainOfCthulhu));

        public static readonly Boss QueenBee = Make(nameof(QueenBee));

        public static readonly Boss Deerclops = Make(nameof(Deerclops));

        public static readonly Boss Skeletron = Make(nameof(Skeletron));

        public static readonly Boss WallOfFlesh = Make(nameof(WallOfFlesh));
        
        public static readonly Boss QueenSlime = Make(nameof(QueenSlime));
        
        public static readonly Boss Destroyer = Make(nameof(Destroyer));
        
        public static readonly Boss Twins = MakeAlternate(Destroyer, nameof(Twins));
        
        public static readonly Boss SkeletronPrime = MakeAlternate(Destroyer, nameof(SkeletronPrime));
        
        public static readonly Boss Plantera = Make(nameof(Plantera));
        
        public static readonly Boss Golem = Make(nameof(Golem));
        
        public static readonly Boss EmpressOfLight = Make(nameof(EmpressOfLight));
        
        public static readonly Boss DukeFishron = Make(nameof(DukeFishron));
        
        public static readonly Boss LunaticCultist = Make(nameof(LunaticCultist));
        
        public static readonly Boss MoonLord = Make(nameof(MoonLord));
    }

    public static class Calamity
    {
        public static readonly Boss DesertScourge = MakeSub(Vanilla.KingSlime, nameof(DesertScourge));
        
        public static readonly Boss Crabulon = MakeSub(Vanilla.EyeOfCthulhu, nameof(Crabulon));
        
        public static readonly Boss HiveMind = MakeSub(Vanilla.EaterOfWorlds, nameof(HiveMind));
        
        public static readonly Boss Perforators = MakeAlternate(HiveMind, nameof(Perforators));
        
        public static readonly Boss SlimeGod = MakeSub(Vanilla.Skeletron, nameof(SlimeGod));
        
        public static readonly Boss Cryogen = MakeAlternate(Vanilla.Destroyer, nameof(Cryogen));
        
        public static readonly Boss AquaticScourge = MakeAlternate(Vanilla.Destroyer, nameof(AquaticScourge));
        
        public static readonly Boss BrimstoneElemental = MakeAlternate(Vanilla.Destroyer, nameof(BrimstoneElemental));
        
        public static readonly Boss CalamitasClone = MakeAlternate(Vanilla.Plantera, nameof(CalamitasClone));
        
        public static readonly Boss LeviathanAndAnahita = MakeSub(Vanilla.Plantera, nameof(LeviathanAndAnahita));
        
        public static readonly Boss AstrumAureus = MakeAfter(LeviathanAndAnahita, nameof(AstrumAureus));
        
        public static readonly Boss PlaguebringerGoliath = MakeSub(Vanilla.Golem, nameof(PlaguebringerGoliath));
        
        public static readonly Boss Ravager = MakeSub(Vanilla.DukeFishron, nameof(Ravager));
        
        public static readonly Boss AstrumDeus = MakeSub(Vanilla.LunaticCultist, nameof(AstrumDeus));
        
        public static readonly Boss ProfanedGuardians = MakeSub(Vanilla.MoonLord, nameof(ProfanedGuardians));
        
        public static readonly Boss Dragonfolly = MakeAfter(ProfanedGuardians, nameof(Dragonfolly));
        
        public static readonly Boss Providence = MakeAfter(Dragonfolly, nameof(Providence));
        
        public static readonly Boss StormWeaver = MakeAfter(Providence, nameof(StormWeaver));
        
        public static readonly Boss CeaselessVoid = MakeAlternate(StormWeaver, nameof(CeaselessVoid));
        
        public static readonly Boss Signus = MakeAlternate(StormWeaver, nameof(Signus));
        
        public static readonly Boss Polterghast = MakeAfter(Signus, nameof(Polterghast));
        
        public static readonly Boss OldDuke = MakeAfter(Polterghast, nameof(OldDuke));
        
        public static readonly Boss DevourerOfGods = MakeAfter(OldDuke, nameof(DevourerOfGods));
        
        public static readonly Boss Yharon = MakeAfter(DevourerOfGods, nameof(Yharon));
        
        public static readonly Boss ExoMechs = MakeAfter(Yharon, nameof(ExoMechs));
        
        public static readonly Boss SupremeCalamitas = MakeAlternate(ExoMechs, nameof(SupremeCalamitas));
    }

    public static class Infernum
    {
        public static readonly Boss Dreadnautilus = MakeSub(Vanilla.WallOfFlesh, nameof(Dreadnautilus));
        
        public static readonly Boss BereftVassal = MakeBefore(Calamity.AstrumDeus, nameof(BereftVassal));

        public static readonly Boss AdultEidolonWyrm = MakeAfter(Calamity.Yharon, nameof(AdultEidolonWyrm));
    }

    public static class Empyreal
    {
        public static readonly Boss AncientDoomsayer = Make(nameof(AncientDoomsayer));
        
        public static readonly Boss DevourerOfUniverses = Make(nameof(DevourerOfUniverses));
        
        public static readonly Boss DragonGodYharon = Make(nameof(DragonGodYharon));
    }

    public static class Thorium
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