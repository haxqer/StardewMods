using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace modtools;

// GMCM API interface
public interface IGenericModConfigMenuApi
{
    void RegisterModConfig(IManifest mod, Action revertToDefault, Action saveToFile);
    void RegisterClampedOption(IManifest mod, string optionName, string optionDesc, Func<int> optionGet, Action<int> optionSet, int min, int max);
    void RegisterSimpleOption(IManifest mod, string optionName, string optionDesc, Func<bool> optionGet, Action<bool> optionSet);
}

public class ModConfig
{
    public int Multiplier { get; set; } = 3;
    
    // Fishing difficulty options (affects minigame, not rewards)
    public bool EasierFishing { get; set; } = false;
    public float FishDifficultyMultiplier { get; set; } = 0.5f; // 0.1-2.0, lower = easier
    public float FishDifficultyAdditive { get; set; } = -20.0f; // -50 to +50
    public bool LargerFishingBar { get; set; } = true;
    public int FishingBarHeight { get; set; } = 400; // 60-568
    public bool SlowerFishMovement { get; set; } = false;
    public float FishMovementSpeedMultiplier { get; set; } = 0.7f; // 0.1-1.0, lower = slower
    
    // Fishing convenience options
    public bool AlwaysPerfect { get; set; } = false;
    public bool AlwaysFindTreasure { get; set; } = false;
    public bool InstantCatchFish { get; set; } = false;
    public bool InstantCatchTreasure { get; set; } = false;
    public bool InfiniteTackle { get; set; } = true;
    public bool InfiniteBait { get; set; } = true;
    public bool AutoHook { get; set; } = false;
    public float LossAdditive { get; set; } = 2 / 1000f;
    
    // Time rate multiplier (1.0 = normal, <1 slower, >1 faster)
    public float TimeRateMultiplier { get; set; } = 1.0f;
    
    // Keybinds
    public KeybindList ReloadKey { get; set; } = new(SButton.F5);
    public KeybindList ConfigMenuKey { get; set; } = new(SButton.K);
} 