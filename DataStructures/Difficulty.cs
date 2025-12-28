using System.Collections;
using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using NohitBot.DataStructures;
using NohitBot.Database;

namespace NohitBot.DataStructures;

public readonly struct Difficulty(Difficulty.GameMode gameMode, Difficulty.Modifier[] modifiers) : IEquatable<Difficulty>
{
    public readonly GameMode Mode = gameMode;

    public readonly Modifier[] Modifiers = modifiers;

    public static bool operator ==(Difficulty mode1, Difficulty mode2) => mode1.Mode == mode2.Mode && mode1.Modifiers.SequenceEqual(mode2.Modifiers);

    public static bool operator !=(Difficulty mode1, Difficulty mode2) => !(mode1 == mode2);

    public bool Equals(Difficulty other) => this == other;

    public override bool Equals(object? obj) => obj is Difficulty other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Mode, (IStructuralComparable)Modifiers);

    public override string ToString() => string.Join('-', Modifiers.Select(m => m.Name).Prepend(Mode.Name));

    public static bool TryParse(string code, ulong guildId, [NotNullWhen(true)] out Difficulty? difficulty, [NotNullWhen(false)] out string? errorMessage)
    {
        var inputs = code.Split('-').ToList();
        var availableModes = DataBase.GameModes.Where(m => m.ManagementServer == guildId).ToArray();
        GameMode? gameMode = null;

        for (int i = 0; i < inputs.Count; i++)
        {
            string input = inputs[i];
            
            foreach (var mode in availableModes)
            {
                if (!mode.Identifier.Equals(input, StringComparison.OrdinalIgnoreCase) && !mode.Name.Equals(input, StringComparison.OrdinalIgnoreCase))
                    continue;
                
                gameMode = mode;
                inputs.Remove(input);
                goto DifficultySelected;
            }
        }
        
        DifficultySelected:

        if (gameMode is null)
        {
            difficulty = null;
            errorMessage = $"Could not parse difficulty mode from input `{code}`";
            return false;
        }

        var availableModifiers = gameMode.AllowedModifiers;
        List<Modifier> modifiers = [];

        LoopModifiers:
        
        while (inputs.Count > 0)
        {
            string input = inputs[0];
            
            foreach (var modifier in availableModifiers)
            {
                if (modifiers.Contains(modifier))
                    continue;
                
                if (!modifier.Identifier.Equals(input, StringComparison.OrdinalIgnoreCase) && !modifier.Name.Equals(input, StringComparison.OrdinalIgnoreCase))
                    continue;
                
                modifiers.Add(modifier);
                inputs.RemoveAt(0);
                goto LoopModifiers;
            }

            difficulty = null;
            errorMessage = $"Could not parse modifier `{input}` for difficulty `{gameMode.Name}`";
            return false;
        }

        // We order them here to ensure sequence order is always equal
        difficulty = new(gameMode, modifiers.OrderBy(m => m.Name).ToArray());
        errorMessage = null;
        return true;
    }

    public static bool TryParseAny(string code, ulong guildId, out GameMode? gameMode, out Modifier[]? modifiers, out string? errorMessage)
    {
        var inputs = code.Split('-').ToList();
        var availableModes = DataBase.GameModes.Where(m => m.ManagementServer == guildId).ToArray();
        gameMode = null;

        for (int i = 0; i < inputs.Count; i++)
        {
            string input = inputs[i];
            
            foreach (var mode in availableModes)
            {
                if (!mode.Identifier.Equals(input, StringComparison.OrdinalIgnoreCase) && !mode.Name.Equals(input, StringComparison.OrdinalIgnoreCase))
                    continue;
                
                gameMode = mode;
                inputs.Remove(input);
                goto DifficultySelected;
            }
        }
        
        DifficultySelected:

        var availableModifiers = (gameMode?.AllowedModifiers ?? DataBase.DifficultyModifiers.Where(m => m.ManagementServer == guildId)).ToArray();
        List<Modifier> selectedModifiers = [];

        LoopModifiers:
        
        while (inputs.Count > 0)
        {
            string input = inputs[0];
            
            foreach (var modifier in availableModifiers)
            {
                if (selectedModifiers.Contains(modifier))
                    continue;
                
                if (!modifier.Identifier.Equals(input, StringComparison.OrdinalIgnoreCase) && !modifier.Name.Equals(input, StringComparison.OrdinalIgnoreCase))
                    continue;
                
                selectedModifiers.Add(modifier);
                inputs.RemoveAt(0);
                goto LoopModifiers;
            }

            gameMode = null;
            modifiers = null;
            errorMessage = $"Could not parse modifier `{input}`";

            if (gameMode is not null)
                errorMessage += $" for difficulty `{gameMode.Name}`";
            
            return false;
        }

        // We order them here to ensure sequence order is always equal
        modifiers = selectedModifiers.OrderBy(m => m.Name).ToArray();
        errorMessage = null;
        return true;
    }

    public class GameMode
    {
        public string Name { get; private set; } = null!;

        public string Identifier { get; private set; } = null!;

        private List<Modifier> allowedModifiers { get; init; } = null!;

        [JsonIgnore]
        public FrozenSet<Modifier> AllowedModifiers => allowedModifiers.ToFrozenSet();

        public BossProgression Progression { get; private set; } = null!;

        public ulong ManagementServer { get; private set; }

        private GameMode() {  }

        private GameMode(string name, string identifier, IEnumerable<Modifier> modifiers, BossProgression progression, ulong managementServer)
        {
            Name = name;
            Identifier = identifier;
            allowedModifiers = modifiers.ToList();
            Progression = progression;
            ManagementServer = managementServer;
        }

        public static GameMode Make(string name, string identifier, IEnumerable<Modifier> modifiers, BossProgression progression, ulong managementServer)
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