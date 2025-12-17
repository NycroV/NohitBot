using System.Collections;
using System.Collections.Frozen;
using Newtonsoft.Json;
using NohitBot.DataStructures;
using NohitBot.Database;

namespace NohitBot.DataStructures;

public readonly struct Difficulty(Difficulty.GameMode gameMode, string[] modifiers) : IEquatable<Difficulty>
{
    public readonly GameMode Mode = gameMode;

    public readonly string[] Modifiers = modifiers;

    public static bool operator ==(Difficulty mode1, Difficulty mode2) => mode1.Mode == mode2.Mode && mode1.Modifiers.SequenceEqual(mode2.Modifiers);

    public static bool operator !=(Difficulty mode1, Difficulty mode2) => !(mode1 == mode2);

    public bool Equals(Difficulty other) => this == other;

    public override bool Equals(object? obj) => obj is Difficulty other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Mode, (IStructuralComparable)Modifiers);

    public class GameMode
    {
        public string Name { get; private set; } = null!;

        public string Identifier { get; private set; } = null!;

        private Dictionary<string, string> allowedModifiers { get; init; } = null!;

        public FrozenDictionary<string, string> AllowedModifiers => allowedModifiers.ToFrozenDictionary();

        public BossProgression Progression { get; private set; } = null!;

        public ulong ManagementServer { get; private set; }

        private GameMode() {  }

        private GameMode(string name, string identifier, IDictionary<string, string> modifiers, BossProgression progression, ulong managementServer)
        {
            Name = name;
            Identifier = identifier;
            allowedModifiers = modifiers.ToDictionary();
            Progression = progression;
            ManagementServer = managementServer;
        }

        public static GameMode Make(string name, string identifier, IDictionary<string, string> modifiers, BossProgression progression, ulong managementServer)
        {
            GameMode gameMode = new GameMode(name, identifier, modifiers, progression, managementServer);
            DataBase.GameModes.Add(gameMode);
            DataBase.Save();
            return gameMode;
        }

        public void Delete()
        {
            DataBase.GameModes.Remove(this);
            DataBase.Save();
        }
    }
    
    public class Modifier
    {
        public string Name { get; private set; } = null!;

        public string Identifier { get; private set; } = null!;

        public ulong ManagementServer { get; private set; }

        private Modifier() { }

        private Modifier(string name, string identifier, ulong managementServer)
        {
            Name = name;
            Identifier = identifier;
            ManagementServer = managementServer;
        }

        public static Modifier Make(string name, string identifier, ulong managementServer)
        {
            Modifier modifier = new(name, identifier, managementServer);
            DataBase.DifficultyModifiers.Add(modifier);
            DataBase.Save();
            return modifier;
        }

        public void Delete()
        {
            DataBase.DifficultyModifiers.Remove(this);
            DataBase.Save();
        }
    }
}