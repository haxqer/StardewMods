using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Objects;
using StardewValley.Monsters;
using StardewValley.Tools;
using SObject = StardewValley.Object;
using System.Collections.Generic;
using System;
using StardewValley.Menus;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

public class ModEntry : Mod
{
    // 资源类物品ID白名单 / Resource item ID whitelist
    private static readonly HashSet<int> ResourceItemIds = new()
    {
        388, // 木头 / Wood
        390, // 石头 / Stone
        382, // 煤炭 / Coal
        771, // 纤维 / Fiber
        330, // 粘土 / Clay
        378, // 铜矿 / Copper Ore
        380, // 铁矿 / Iron Ore
        384, // 金矿 / Gold Ore
        386, // 铱矿 / Iridium Ore
        709, // 硬木 / Hardwood
        92,  // 树液 / Sap
        766, // 史莱姆 / Slime
        80,  // 石英 / Quartz
        338, // 精炼石英 / Refined Quartz
        767, // 蝙蝠翅膀 / Bat Wing
        684, // 虫肉 / Bug Meat
        768, // 太阳精华 / Solar Essence
        769, // 虚空精华 / Void Essence
        
        // 宝石 / Gems
        60,  // 绿宝石 / Emerald
        62,  // 海蓝宝石 / Aquamarine
        64,  // 红宝石 / Ruby
        66,  // 紫水晶 / Amethyst
        68,  // 黄玉 / Topaz
        70,  // 翡翠 / Jade
        72,  // 钻石 / Diamond
        74,  // 五彩碎片 / Prismatic Shard
        82,  // 火石英 / Fire Quartz
        84,  // 冰冻泪滴 / Frozen Tear
        86,  // 地晶 / Earth Crystal
        
        // 矿物 / Minerals
        538, // 铝矿 / Alamite
        539, // 铋矿 / Bixite
        540, // 重晶石 / Baryte
        541, // 蓝晶石 / Aerinite
        542, // 方解石 / Calcite
        543, // 白云石 / Dolomite
        544, // 钙长石 / Esperite
        545, // 氟磷灰石 / Fluorapatite
        546, // 宝石矿 / Geminite
        547, // 日光榴石 / Helvite
        548, // 蓝方石 / Jamborite
        549, // 绿帘石 / Jagoite
        550, // 蓝晶石 / Kyanite
        551, // 月长石 / Lunarite
        552, // 孔雀石 / Malachite
        553, // 海王石 / Neptunite
        554, // 柠檬石 / Lemon Stone
        555, // 猫眼石 / Nekoite
        556, // 雌黄 / Orpiment
        557, // 石化史莱姆 / Petrified Slime
        558, // 雷蛋 / Thunder Egg
        559, // 黄铁矿 / Pyrite
        560, // 海洋石 / Ocean Stone
        561, // 幽灵水晶 / Ghost Crystal
        562, // 虎眼石 / Tigerseye
        563, // 碧玉 / Jasper
        564, // 蛋白石 / Opal
        565, // 火蛋白石 / Fire Opal
        566, // 天青石 / Celestine
        567, // 大理石 / Marble
        568, // 砂岩 / Sandstone
        569, // 花岗岩 / Granite
        570, // 玄武岩 / Basalt
        571, // 石灰岩 / Limestone
        572, // 皂石 / Soapstone
        573, // 赤铁矿 / Hematite
        574, // 泥岩 / Mudstone
        575, // 黑曜石 / Obsidian
        576, // 板岩 / Slate
        577, // 仙石 / Fairy Stone
        578  // 星之碎片 / Star Shards
    };

    // 防递归锁
    private bool isGivingBonus = false;
    // Suppress CS8618: Config is initialized in Entry() by SMAPI
    private ModConfig Config = null!;
    private int EffectiveMultiplier =>
        Config.Multiplier < 3 ? 3 :
        Config.Multiplier > 100 ? 100 :
        Config.Multiplier;
    private readonly PerScreen<bool> BeganFishingGame = new();
    private readonly PerScreen<int> UpdateIndex = new();
    private int timeSkipCounter = 0;

    public override void Entry(IModHelper helper)
    {
        // CommonHelper.RemoveObsoleteFiles(this, "FishingMod.pdb"); // Not needed
        Config = helper.ReadConfig<ModConfig>();
        helper.Events.Player.InventoryChanged += OnInventoryChanged;
        helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
        helper.Events.Input.ButtonsChanged += OnButtonsChanged;
        helper.Events.GameLoop.TimeChanged += OnTimeChanged;
        helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        helper.Events.Display.MenuChanged += OnMenuChanged;
        this.Monitor.Log($"Loaded Multiplier: {Config.Multiplier}", LogLevel.Info);

        // 注册快捷键监听，按K键打开自定义菜单
        helper.Events.Input.ButtonPressed += OnButtonPressed;
    }

    private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
    {
        if (!Context.IsWorldReady || !Context.IsPlayerFree)
            return;
        if (Config.ConfigMenuKey.JustPressed())
        {
            Game1.activeClickableMenu = new MultiplierConfigMenu(Config, SaveConfig, Helper);
        }
    }

    private void SaveConfig()
    {
        Helper.WriteConfig(Config);
        this.Monitor.Log($"Multiplier set to: {Config.Multiplier}", LogLevel.Info);
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        this.Monitor.Log("OnGameLaunched called", LogLevel.Info);
        var api = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
        if (api is null)
        {
            this.Monitor.Log("GMCM API not found", LogLevel.Warn);
            return;
        }
        this.Monitor.Log("GMCM API found, registering config", LogLevel.Info);
        api.RegisterModConfig(
            this.ModManifest,
            () => Config = new ModConfig(),
            () => Helper.WriteConfig(Config)
        );
        
        // Resource multiplier
        api.RegisterClampedOption(
            this.ModManifest,
            Helper.Translation.Get("config.gmcm.resource-multiplier"),
            Helper.Translation.Get("config.gmcm.resource-multiplier-desc"),
            () => Config.Multiplier,
            val => { Config.Multiplier = val; Helper.WriteConfig(Config); },
            3, 50
        );
        
        // Fishing difficulty options
        api.RegisterSimpleOption(
            this.ModManifest,
            Helper.Translation.Get("config.gmcm.easier-fishing"),
            Helper.Translation.Get("config.gmcm.easier-fishing-desc"),
            () => Config.EasierFishing,
            val => { Config.EasierFishing = val; Helper.WriteConfig(Config); }
        );
        
        api.RegisterClampedOption(
            this.ModManifest,
            Helper.Translation.Get("config.gmcm.fish-difficulty-multiplier"),
            Helper.Translation.Get("config.gmcm.fish-difficulty-multiplier-desc"),
            () => (int)(Config.FishDifficultyMultiplier * 100),
            val => { Config.FishDifficultyMultiplier = val / 100f; Helper.WriteConfig(Config); },
            10, 200
        );
        
        api.RegisterClampedOption(
            this.ModManifest,
            Helper.Translation.Get("config.gmcm.fish-difficulty-additive"),
            Helper.Translation.Get("config.gmcm.fish-difficulty-additive-desc"),
            () => (int)(Config.FishDifficultyAdditive + 50),
            val => { Config.FishDifficultyAdditive = val - 50; Helper.WriteConfig(Config); },
            0, 100
        );
        
        api.RegisterSimpleOption(
            this.ModManifest,
            Helper.Translation.Get("config.gmcm.larger-fishing-bar"),
            Helper.Translation.Get("config.gmcm.larger-fishing-bar-desc"),
            () => Config.LargerFishingBar,
            val => { Config.LargerFishingBar = val; Helper.WriteConfig(Config); }
        );
        
        api.RegisterClampedOption(
            this.ModManifest,
            Helper.Translation.Get("config.gmcm.fishing-bar-height"),
            Helper.Translation.Get("config.gmcm.fishing-bar-height-desc"),
            () => Config.FishingBarHeight,
            val => { Config.FishingBarHeight = val; Helper.WriteConfig(Config); },
            60, 568
        );
        
        api.RegisterSimpleOption(
            this.ModManifest,
            Helper.Translation.Get("config.gmcm.slower-fish-movement"),
            Helper.Translation.Get("config.gmcm.slower-fish-movement-desc"),
            () => Config.SlowerFishMovement,
            val => { Config.SlowerFishMovement = val; Helper.WriteConfig(Config); }
        );
        
        api.RegisterClampedOption(
            this.ModManifest,
            Helper.Translation.Get("config.gmcm.fish-movement-speed"),
            Helper.Translation.Get("config.gmcm.fish-movement-speed-desc"),
            () => (int)(Config.FishMovementSpeedMultiplier * 100),
            val => { Config.FishMovementSpeedMultiplier = val / 100f; Helper.WriteConfig(Config); },
            10, 100
        );
        
        // Fishing convenience options
        api.RegisterSimpleOption(
            this.ModManifest,
            Helper.Translation.Get("config.gmcm.always-perfect"),
            Helper.Translation.Get("config.gmcm.always-perfect-desc"),
            () => Config.AlwaysPerfect,
            val => { Config.AlwaysPerfect = val; Helper.WriteConfig(Config); }
        );
        
        api.RegisterSimpleOption(
            this.ModManifest,
            Helper.Translation.Get("config.gmcm.always-find-treasure"),
            Helper.Translation.Get("config.gmcm.always-find-treasure-desc"),
            () => Config.AlwaysFindTreasure,
            val => { Config.AlwaysFindTreasure = val; Helper.WriteConfig(Config); }
        );
        
        api.RegisterSimpleOption(
            this.ModManifest,
            Helper.Translation.Get("config.gmcm.instant-catch-fish"),
            Helper.Translation.Get("config.gmcm.instant-catch-fish-desc"),
            () => Config.InstantCatchFish,
            val => { Config.InstantCatchFish = val; Helper.WriteConfig(Config); }
        );
        
        api.RegisterSimpleOption(
            this.ModManifest,
            Helper.Translation.Get("config.gmcm.instant-catch-treasure"),
            Helper.Translation.Get("config.gmcm.instant-catch-treasure-desc"),
            () => Config.InstantCatchTreasure,
            val => { Config.InstantCatchTreasure = val; Helper.WriteConfig(Config); }
        );
        
        api.RegisterSimpleOption(
            this.ModManifest,
            Helper.Translation.Get("config.gmcm.infinite-bait"),
            Helper.Translation.Get("config.gmcm.infinite-bait-desc"),
            () => Config.InfiniteBait,
            val => { Config.InfiniteBait = val; Helper.WriteConfig(Config); }
        );
        
        api.RegisterSimpleOption(
            this.ModManifest,
            Helper.Translation.Get("config.gmcm.infinite-tackle"),
            Helper.Translation.Get("config.gmcm.infinite-tackle-desc"),
            () => Config.InfiniteTackle,
            val => { Config.InfiniteTackle = val; Helper.WriteConfig(Config); }
        );
        
        api.RegisterSimpleOption(
            this.ModManifest,
            Helper.Translation.Get("config.gmcm.auto-hook"),
            Helper.Translation.Get("config.gmcm.auto-hook-desc"),
            () => Config.AutoHook,
            val => { Config.AutoHook = val; Helper.WriteConfig(Config); }
        );
        
        // Time rate multiplier
        api.RegisterClampedOption(
            this.ModManifest,
            Helper.Translation.Get("config.gmcm.time-rate-multiplier"),
            Helper.Translation.Get("config.gmcm.time-rate-multiplier-desc"),
            () => (int)(Config.TimeRateMultiplier * 100),
            val => { Config.TimeRateMultiplier = val / 100f; Helper.WriteConfig(Config); },
            10, 200
        );
    }

    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        if (!Context.IsWorldReady)
            return;
        
        // Infinite bait/tackle
        if (e.IsOneSecond && (Config.InfiniteBait || Config.InfiniteTackle))
        {
            if (Game1.player.CurrentTool is FishingRod rod)
            {
                if (Config.InfiniteBait && rod.attachments?.Length > 0 && rod.attachments[0] != null)
                    rod.attachments[0].Stack = rod.attachments[0].maximumStackSize();
                if (Config.InfiniteTackle && rod.attachments?.Length > 1 && rod.attachments[1] != null)
                    rod.attachments[1].uses.Value = 0;
            }
        }
        
        // Fishing minigame logic
        if (Game1.activeClickableMenu is BobberBar bobber)
        {
            // Begin fishing game - only apply modifications once
            if (!BeganFishingGame.Value && UpdateIndex.Value > 15)
            {
                // Store original difficulty for reward calculation
                var originalDifficulty = bobber.difficulty;
                
                // Apply difficulty modifications (these affect minigame only)
                if (Config.EasierFishing)
                {
                    bobber.motionType = 2; // Easier motion pattern
                }
                
                // Apply difficulty multiplier and additive (clamped to reasonable values)
                float newDifficulty = originalDifficulty * Config.FishDifficultyMultiplier + Config.FishDifficultyAdditive;
                bobber.difficulty = Math.Max(1, Math.Min(100, (int)newDifficulty));
                
                // Apply convenience options
                if (Config.AlwaysFindTreasure)
                    bobber.treasure = true;
                    
                if (Config.InstantCatchFish)
                {
                    if (bobber.treasure)
                        bobber.treasureCaught = true;
                    bobber.distanceFromCatching += 100;
                }
                
                if (Config.InstantCatchTreasure)
                {
                    if (bobber.treasure || Config.AlwaysFindTreasure)
                        bobber.treasureCaught = true;
                }
                
                BeganFishingGame.Value = true;
            }
            
            if (UpdateIndex.Value < 20)
                UpdateIndex.Value++;
                
            // Continuous effects during minigame
            if (Config.AlwaysPerfect)
                bobber.perfect = true;
                
            if (!bobber.bobberInBar)
                bobber.distanceFromCatching += Config.LossAdditive;
        }
        else
        {
            // End fishing game
            BeganFishingGame.Value = false;
            UpdateIndex.Value = 0;
        }
    }
    private void OnTimeChanged(object? sender, TimeChangedEventArgs e)
    {
        if (Config.TimeRateMultiplier >= 0.99f)
        {
            timeSkipCounter = 0;
            return; // normal speed
        }
        // Only allow time to advance every Nth event
        int skip = (int)Math.Round(1.0 / Config.TimeRateMultiplier);
        timeSkipCounter++;
        if (timeSkipCounter < skip)
        {
            // Revert time
            Game1.timeOfDay = e.OldTime;
        }
        else
        {
            timeSkipCounter = 0;
        }
    }
    private void OnButtonsChanged(object? sender, ButtonsChangedEventArgs e)
    {
        if (Config.ReloadKey.JustPressed())
        {
            Config = Helper.ReadConfig<ModConfig>();
            Monitor.Log("Config reloaded", LogLevel.Info);
        }
    }

    private void OnInventoryChanged(object? sender, InventoryChangedEventArgs e)
    {
        if (!Context.IsWorldReady || !e.IsLocalPlayer)
            return;

        if (isGivingBonus)
            return;

        // 处理堆叠数量变化
        foreach (var entry in e.QuantityChanged)
        {
            var item = entry.Item;
            if (item == null || entry.NewSize <= entry.OldSize || entry.NewSize - entry.OldSize > 1)
                continue;

            if (item is SObject obj && ResourceItemIds.Contains(obj.ParentSheetIndex))
            {
                int delta = entry.NewSize - entry.OldSize;
                int extra = delta * (EffectiveMultiplier - 1); // N获得，额外加(N*(倍数-1))
                if (extra > 0)
                {
                    var cloned = item.getOne();
                    cloned.Stack = extra;
                    isGivingBonus = true;
                    Game1.player.addItemToInventory(cloned);
                    isGivingBonus = false;
                }
            }
        }

        // 处理新增物品堆叠
        foreach (var item in e.Added)
        {
            if (item is SObject obj && ResourceItemIds.Contains(obj.ParentSheetIndex))
            {
                if (item.Stack > 1)
                {
                    continue;
                }

                int extra = item.Stack * (EffectiveMultiplier - 1); // N获得，额外加(N*(倍数-1))
                if (extra > 0)
                {
                    var cloned = item.getOne();
                    cloned.Stack = extra;
                    isGivingBonus = true;
                    Game1.player.addItemToInventory(cloned);
                    isGivingBonus = false;
                }
            }
        }
    }

    private void OnMenuChanged(object? sender, MenuChangedEventArgs e)
    {
        if (e.NewMenu is BobberBar bobberBar)
        {
            try
            {
                // Apply fishing bar height modification
                if (Config.LargerFishingBar)
                {
                    var barHeightField = this.Helper.Reflection.GetField<int>(bobberBar, "bobberBarHeight");
                    if (barHeightField != null)
                    {
                        barHeightField.SetValue(Config.FishingBarHeight);
                    }
                }
                
                // Apply fish movement speed modification
                if (Config.SlowerFishMovement)
                {
                    var fishField = this.Helper.Reflection.GetField<object>(bobberBar, "fish");
                    if (fishField != null)
                    {
                        var fish = fishField.GetValue();
                        if (fish != null)
                        {
                            var fishType = fish.GetType();
                            
                            // Try to modify fish movement speed
                            var speedField = fishType.GetField("speed");
                            if (speedField != null)
                            {
                                var currentSpeed = speedField.GetValue(fish);
                                if (currentSpeed is float speed)
                                {
                                    speedField.SetValue(fish, speed * Config.FishMovementSpeedMultiplier);
                                }
                            }
                            
                            // Try to modify fish movement pattern speed
                            var patternSpeedField = fishType.GetField("patternSpeed");
                            if (patternSpeedField != null)
                            {
                                var currentPatternSpeed = patternSpeedField.GetValue(fish);
                                if (currentPatternSpeed is float patternSpeed)
                                {
                                    patternSpeedField.SetValue(fish, patternSpeed * Config.FishMovementSpeedMultiplier);
                                }
                            }
                        }
                    }
                }
                
                // Auto-hook functionality
                if (Config.AutoHook)
                {
                    // This would need to be implemented in the UpdateTicked method
                    // as we need to continuously check for fish biting
                }
            }
            catch (Exception ex)
            {
                this.Monitor.Log($"Error modifying fishing minigame: {ex}", LogLevel.Error);
            }
        }
    }
}

// 自定义Multiplier配置菜单
public class MultiplierConfigMenu : IClickableMenu
{
    private ModConfig config;
    private Action saveAction;
    private IModHelper helper;
    
    // Resource multiplier buttons
    private ClickableComponent plusButton;
    private ClickableComponent minusButton;
    
    // Fish difficulty buttons
    private ClickableComponent plusFishButton;
    private ClickableComponent minusFishButton;
    private ClickableComponent plusFishAddButton;
    private ClickableComponent minusFishAddButton;
    private ClickableComponent plusBarHeightButton;
    private ClickableComponent minusBarHeightButton;
    private ClickableComponent plusFishSpeedButton;
    private ClickableComponent minusFishSpeedButton;
    
    // Time rate buttons
    private ClickableComponent plusTimeButton;
    private ClickableComponent minusTimeButton;
    
    // Toggle buttons
    private ClickableComponent easierFishingButton;
    private ClickableComponent largerBarButton;
    private ClickableComponent slowerFishButton;
    private ClickableComponent alwaysPerfectButton;
    private ClickableComponent alwaysTreasureButton;
    private ClickableComponent instantCatchButton;
    private ClickableComponent instantTreasureButton;
    private ClickableComponent infiniteBaitButton;
    private ClickableComponent infiniteTackleButton;
    private ClickableComponent autoHookButton;

    // Layout constants
    private const int WindowWidth = 1200;
    private const int WindowHeight = 800;
    private const int SectionSpacing = 100;
    private const int ButtonSize = 60;
    private const int ToggleButtonWidth = 200;
    private const int ToggleButtonHeight = 40;

    public MultiplierConfigMenu(ModConfig config, Action saveAction, IModHelper helper)
        : base(Game1.viewport.Width / 2 - WindowWidth / 2, Game1.viewport.Height / 2 - WindowHeight / 2, WindowWidth, WindowHeight)
    {
        this.config = config;
        this.saveAction = saveAction;
        this.helper = helper;
        
        // Initialize all buttons with placeholder positions
        // They will be positioned properly in the draw method
        plusButton = new ClickableComponent(new Rectangle(0, 0, ButtonSize, ButtonSize), "Plus");
        minusButton = new ClickableComponent(new Rectangle(0, 0, ButtonSize, ButtonSize), "Minus");
        plusFishButton = new ClickableComponent(new Rectangle(0, 0, ButtonSize, ButtonSize), "PlusFish");
        minusFishButton = new ClickableComponent(new Rectangle(0, 0, ButtonSize, ButtonSize), "MinusFish");
        plusFishAddButton = new ClickableComponent(new Rectangle(0, 0, ButtonSize, ButtonSize), "PlusFishAdd");
        minusFishAddButton = new ClickableComponent(new Rectangle(0, 0, ButtonSize, ButtonSize), "MinusFishAdd");
        plusBarHeightButton = new ClickableComponent(new Rectangle(0, 0, ButtonSize, ButtonSize), "PlusBarHeight");
        minusBarHeightButton = new ClickableComponent(new Rectangle(0, 0, ButtonSize, ButtonSize), "MinusBarHeight");
        plusFishSpeedButton = new ClickableComponent(new Rectangle(0, 0, ButtonSize, ButtonSize), "PlusFishSpeed");
        minusFishSpeedButton = new ClickableComponent(new Rectangle(0, 0, ButtonSize, ButtonSize), "MinusFishSpeed");
        plusTimeButton = new ClickableComponent(new Rectangle(0, 0, ButtonSize, ButtonSize), "PlusTime");
        minusTimeButton = new ClickableComponent(new Rectangle(0, 0, ButtonSize, ButtonSize), "MinusTime");
        
        // Toggle buttons
        easierFishingButton = new ClickableComponent(new Rectangle(0, 0, ToggleButtonWidth, ToggleButtonHeight), "EasierFishing");
        largerBarButton = new ClickableComponent(new Rectangle(0, 0, ToggleButtonWidth, ToggleButtonHeight), "LargerBar");
        slowerFishButton = new ClickableComponent(new Rectangle(0, 0, ToggleButtonWidth, ToggleButtonHeight), "SlowerFish");
        alwaysPerfectButton = new ClickableComponent(new Rectangle(0, 0, ToggleButtonWidth, ToggleButtonHeight), "AlwaysPerfect");
        alwaysTreasureButton = new ClickableComponent(new Rectangle(0, 0, ToggleButtonWidth, ToggleButtonHeight), "AlwaysTreasure");
        instantCatchButton = new ClickableComponent(new Rectangle(0, 0, ToggleButtonWidth, ToggleButtonHeight), "InstantCatch");
        instantTreasureButton = new ClickableComponent(new Rectangle(0, 0, ToggleButtonWidth, ToggleButtonHeight), "InstantTreasure");
        infiniteBaitButton = new ClickableComponent(new Rectangle(0, 0, ToggleButtonWidth, ToggleButtonHeight), "InfiniteBait");
        infiniteTackleButton = new ClickableComponent(new Rectangle(0, 0, ToggleButtonWidth, ToggleButtonHeight), "InfiniteTackle");
        autoHookButton = new ClickableComponent(new Rectangle(0, 0, ToggleButtonWidth, ToggleButtonHeight), "AutoHook");
    }

    public override void draw(Microsoft.Xna.Framework.Graphics.SpriteBatch b)
    {
        // Draw background window
        IClickableMenu.drawTextureBox(b, xPositionOnScreen, yPositionOnScreen, width, height, Color.White);

        // Title
        string title = helper.Translation.Get("config.title");
        Vector2 titleSize = Game1.dialogueFont.MeasureString(title);
        b.DrawString(Game1.dialogueFont, title, new Vector2(xPositionOnScreen + width / 2 - titleSize.X / 2, yPositionOnScreen + 20), Color.Black);

        // Layout constants
        int startY = yPositionOnScreen + 80;
        int leftColumnX = xPositionOnScreen + 50;
        int rightColumnX = xPositionOnScreen + width / 2 + 50;
        int labelX = leftColumnX;
        int minusX = labelX + 250;
        int valueX = minusX + ButtonSize + 20;
        int plusX = valueX + 80;
        int toggleX = rightColumnX;
        int toggleY = startY;

        // Section 1: Resource Multiplier
        string resLabel = helper.Translation.Get("config.resource-multiplier");
        b.DrawString(Game1.smallFont, resLabel, new Vector2(labelX, startY + 15), Color.Black);
        minusButton.bounds = new Rectangle(minusX, startY, ButtonSize, ButtonSize);
        plusButton.bounds = new Rectangle(plusX, startY, ButtonSize, ButtonSize);
        drawButton(b, minusButton, "-");
        string resValue = config.Multiplier.ToString();
        Vector2 resValueSize = Game1.smallFont.MeasureString(resValue);
        b.DrawString(Game1.smallFont, resValue, new Vector2(valueX + 30 - resValueSize.X / 2, startY + 15), Color.DarkBlue);
        drawButton(b, plusButton, "+");

        // Section 2: Fish Difficulty Multiplier
        startY += 60;
        string fishLabel = helper.Translation.Get("config.fish-difficulty-multiplier");
        b.DrawString(Game1.smallFont, fishLabel, new Vector2(labelX, startY + 15), Color.Black);
        minusFishButton.bounds = new Rectangle(minusX, startY, ButtonSize, ButtonSize);
        plusFishButton.bounds = new Rectangle(plusX, startY, ButtonSize, ButtonSize);
        drawButton(b, minusFishButton, "-");
        string fishValue = $"x{config.FishDifficultyMultiplier:F2}";
        Vector2 fishValueSize = Game1.smallFont.MeasureString(fishValue);
        b.DrawString(Game1.smallFont, fishValue, new Vector2(valueX + 30 - fishValueSize.X / 2, startY + 15), Color.DarkGreen);
        drawButton(b, plusFishButton, "+");

        // Section 3: Fish Difficulty Additive
        startY += 60;
        string fishAddLabel = helper.Translation.Get("config.fish-difficulty-additive");
        b.DrawString(Game1.smallFont, fishAddLabel, new Vector2(labelX, startY + 15), Color.Black);
        minusFishAddButton.bounds = new Rectangle(minusX, startY, ButtonSize, ButtonSize);
        plusFishAddButton.bounds = new Rectangle(plusX, startY, ButtonSize, ButtonSize);
        drawButton(b, minusFishAddButton, "-");
        string fishAddValue = $"{config.FishDifficultyAdditive:F1}";
        Vector2 fishAddValueSize = Game1.smallFont.MeasureString(fishAddValue);
        b.DrawString(Game1.smallFont, fishAddValue, new Vector2(valueX + 30 - fishAddValueSize.X / 2, startY + 15), Color.DarkGreen);
        drawButton(b, plusFishAddButton, "+");

        // Section 4: Fishing Bar Height
        startY += 60;
        string barLabel = helper.Translation.Get("config.fishing-bar-height");
        b.DrawString(Game1.smallFont, barLabel, new Vector2(labelX, startY + 15), Color.Black);
        minusBarHeightButton.bounds = new Rectangle(minusX, startY, ButtonSize, ButtonSize);
        plusBarHeightButton.bounds = new Rectangle(plusX, startY, ButtonSize, ButtonSize);
        drawButton(b, minusBarHeightButton, "-");
        string barValue = config.FishingBarHeight.ToString();
        Vector2 barValueSize = Game1.smallFont.MeasureString(barValue);
        b.DrawString(Game1.smallFont, barValue, new Vector2(valueX + 30 - barValueSize.X / 2, startY + 15), Color.DarkGreen);
        drawButton(b, plusBarHeightButton, "+");

        // Section 5: Fish Movement Speed
        startY += 60;
        string speedLabel = helper.Translation.Get("config.fish-movement-speed");
        b.DrawString(Game1.smallFont, speedLabel, new Vector2(labelX, startY + 15), Color.Black);
        minusFishSpeedButton.bounds = new Rectangle(minusX, startY, ButtonSize, ButtonSize);
        plusFishSpeedButton.bounds = new Rectangle(plusX, startY, ButtonSize, ButtonSize);
        drawButton(b, minusFishSpeedButton, "-");
        string speedValue = $"x{config.FishMovementSpeedMultiplier:F2}";
        Vector2 speedValueSize = Game1.smallFont.MeasureString(speedValue);
        b.DrawString(Game1.smallFont, speedValue, new Vector2(valueX + 30 - speedValueSize.X / 2, startY + 15), Color.DarkGreen);
        drawButton(b, plusFishSpeedButton, "+");

        // Section 6: Time Rate Multiplier
        startY += 60;
        string timeLabel = helper.Translation.Get("config.time-rate-multiplier");
        b.DrawString(Game1.smallFont, timeLabel, new Vector2(labelX, startY + 15), Color.Black);
        minusTimeButton.bounds = new Rectangle(minusX, startY, ButtonSize, ButtonSize);
        plusTimeButton.bounds = new Rectangle(plusX, startY, ButtonSize, ButtonSize);
        drawButton(b, minusTimeButton, "-");
        string timeValue = $"x{config.TimeRateMultiplier:F2}";
        Vector2 timeValueSize = Game1.smallFont.MeasureString(timeValue);
        b.DrawString(Game1.smallFont, timeValue, new Vector2(valueX + 30 - timeValueSize.X / 2, startY + 15), Color.Purple);
        drawButton(b, plusTimeButton, "+");

        // Right column: Toggle options
        // Section 1: Difficulty toggles
        string difficultyTitle = helper.Translation.Get("config.difficulty-options");
        b.DrawString(Game1.smallFont, difficultyTitle, new Vector2(toggleX, toggleY), Color.Black);
        toggleY += 30;

        easierFishingButton.bounds = new Rectangle(toggleX, toggleY, ToggleButtonWidth, ToggleButtonHeight);
        drawToggleButton(b, easierFishingButton, helper.Translation.Get("config.easier-fishing"), config.EasierFishing);
        toggleY += 50;

        largerBarButton.bounds = new Rectangle(toggleX, toggleY, ToggleButtonWidth, ToggleButtonHeight);
        drawToggleButton(b, largerBarButton, helper.Translation.Get("config.larger-bar"), config.LargerFishingBar);
        toggleY += 50;

        slowerFishButton.bounds = new Rectangle(toggleX, toggleY, ToggleButtonWidth, ToggleButtonHeight);
        drawToggleButton(b, slowerFishButton, helper.Translation.Get("config.slower-fish"), config.SlowerFishMovement);
        toggleY += 50;

        // Section 2: Convenience toggles
        toggleY += 20;
        string convenienceTitle = helper.Translation.Get("config.convenience-options");
        b.DrawString(Game1.smallFont, convenienceTitle, new Vector2(toggleX, toggleY), Color.Black);
        toggleY += 30;

        alwaysPerfectButton.bounds = new Rectangle(toggleX, toggleY, ToggleButtonWidth, ToggleButtonHeight);
        drawToggleButton(b, alwaysPerfectButton, helper.Translation.Get("config.always-perfect"), config.AlwaysPerfect);
        toggleY += 50;

        alwaysTreasureButton.bounds = new Rectangle(toggleX, toggleY, ToggleButtonWidth, ToggleButtonHeight);
        drawToggleButton(b, alwaysTreasureButton, helper.Translation.Get("config.always-treasure"), config.AlwaysFindTreasure);
        toggleY += 50;

        instantCatchButton.bounds = new Rectangle(toggleX, toggleY, ToggleButtonWidth, ToggleButtonHeight);
        drawToggleButton(b, instantCatchButton, helper.Translation.Get("config.instant-catch"), config.InstantCatchFish);
        toggleY += 50;

        instantTreasureButton.bounds = new Rectangle(toggleX, toggleY, ToggleButtonWidth, ToggleButtonHeight);
        drawToggleButton(b, instantTreasureButton, helper.Translation.Get("config.instant-treasure"), config.InstantCatchTreasure);
        toggleY += 50;

        infiniteBaitButton.bounds = new Rectangle(toggleX, toggleY, ToggleButtonWidth, ToggleButtonHeight);
        drawToggleButton(b, infiniteBaitButton, helper.Translation.Get("config.infinite-bait"), config.InfiniteBait);
        toggleY += 50;

        infiniteTackleButton.bounds = new Rectangle(toggleX, toggleY, ToggleButtonWidth, ToggleButtonHeight);
        drawToggleButton(b, infiniteTackleButton, helper.Translation.Get("config.infinite-tackle"), config.InfiniteTackle);
        toggleY += 50;

        autoHookButton.bounds = new Rectangle(toggleX, toggleY, ToggleButtonWidth, ToggleButtonHeight);
        drawToggleButton(b, autoHookButton, helper.Translation.Get("config.auto-hook"), config.AutoHook);

        // Instructions
        string tip = helper.Translation.Get("config.close-tip");
        Vector2 tipSize = Game1.tinyFont.MeasureString(tip);
        b.DrawString(Game1.tinyFont, tip, new Vector2(xPositionOnScreen + width - tipSize.X - 32, yPositionOnScreen + height - tipSize.Y - 32), Color.Gray);

        // Draw mouse
        drawMouse(b);
    }

    private void drawButton(SpriteBatch b, ClickableComponent button, string text)
    {
        IClickableMenu.drawTextureBox(b, button.bounds.X, button.bounds.Y, button.bounds.Width, button.bounds.Height, Color.SandyBrown);
        Vector2 textSize = Game1.smallFont.MeasureString(text);
        b.DrawString(Game1.smallFont, text, new Vector2(button.bounds.X + button.bounds.Width / 2 - textSize.X / 2, button.bounds.Y + button.bounds.Height / 2 - textSize.Y / 2), Color.Black);
    }

    private void drawToggleButton(SpriteBatch b, ClickableComponent button, string text, bool isEnabled)
    {
        Color buttonColor = isEnabled ? Color.LightGreen : Color.LightGray;
        IClickableMenu.drawTextureBox(b, button.bounds.X, button.bounds.Y, button.bounds.Width, button.bounds.Height, buttonColor);
        Vector2 textSize = Game1.smallFont.MeasureString(text);
        b.DrawString(Game1.smallFont, text, new Vector2(button.bounds.X + button.bounds.Width / 2 - textSize.X / 2, button.bounds.Y + button.bounds.Height / 2 - textSize.Y / 2), Color.Black);
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
        // Resource multiplier
        if (plusButton.containsPoint(x, y))
        {
            if (config.Multiplier < 50)
            {
                config.Multiplier++;
                saveAction();
                Game1.playSound("drumkit6");
            }
        }
        else if (minusButton.containsPoint(x, y))
        {
            if (config.Multiplier > 3)
            {
                config.Multiplier--;
                saveAction();
                Game1.playSound("drumkit6");
            }
        }
        // Fish difficulty multiplier
        else if (plusFishButton.containsPoint(x, y))
        {
            if (config.FishDifficultyMultiplier < 2.0f)
            {
                config.FishDifficultyMultiplier = MathF.Min(2.0f, config.FishDifficultyMultiplier + 0.05f);
                saveAction();
                Game1.playSound("drumkit6");
            }
        }
        else if (minusFishButton.containsPoint(x, y))
        {
            if (config.FishDifficultyMultiplier > 0.1f)
            {
                config.FishDifficultyMultiplier = MathF.Max(0.1f, config.FishDifficultyMultiplier - 0.05f);
                saveAction();
                Game1.playSound("drumkit6");
            }
        }
        // Fish difficulty additive
        else if (plusFishAddButton.containsPoint(x, y))
        {
            if (config.FishDifficultyAdditive < 50.0f)
            {
                config.FishDifficultyAdditive = MathF.Min(50.0f, config.FishDifficultyAdditive + 5.0f);
                saveAction();
                Game1.playSound("drumkit6");
            }
        }
        else if (minusFishAddButton.containsPoint(x, y))
        {
            if (config.FishDifficultyAdditive > -50.0f)
            {
                config.FishDifficultyAdditive = MathF.Max(-50.0f, config.FishDifficultyAdditive - 5.0f);
                saveAction();
                Game1.playSound("drumkit6");
            }
        }
        // Fishing bar height
        else if (plusBarHeightButton.containsPoint(x, y))
        {
            if (config.FishingBarHeight < 568)
            {
                config.FishingBarHeight = Math.Min(568, config.FishingBarHeight + 20);
                saveAction();
                Game1.playSound("drumkit6");
            }
        }
        else if (minusBarHeightButton.containsPoint(x, y))
        {
            if (config.FishingBarHeight > 60)
            {
                config.FishingBarHeight = Math.Max(60, config.FishingBarHeight - 20);
                saveAction();
                Game1.playSound("drumkit6");
            }
        }
        // Fish movement speed
        else if (plusFishSpeedButton.containsPoint(x, y))
        {
            if (config.FishMovementSpeedMultiplier < 1.0f)
            {
                config.FishMovementSpeedMultiplier = MathF.Min(1.0f, config.FishMovementSpeedMultiplier + 0.05f);
                saveAction();
                Game1.playSound("drumkit6");
            }
        }
        else if (minusFishSpeedButton.containsPoint(x, y))
        {
            if (config.FishMovementSpeedMultiplier > 0.1f)
            {
                config.FishMovementSpeedMultiplier = MathF.Max(0.1f, config.FishMovementSpeedMultiplier - 0.05f);
                saveAction();
                Game1.playSound("drumkit6");
            }
        }
        // Time rate multiplier
        else if (plusTimeButton.containsPoint(x, y))
        {
            if (config.TimeRateMultiplier < 2.0f)
            {
                config.TimeRateMultiplier = MathF.Min(2.0f, config.TimeRateMultiplier + 0.05f);
                saveAction();
                Game1.playSound("drumkit6");
            }
        }
        else if (minusTimeButton.containsPoint(x, y))
        {
            if (config.TimeRateMultiplier > 0.1f)
            {
                config.TimeRateMultiplier = MathF.Max(0.1f, config.TimeRateMultiplier - 0.05f);
                saveAction();
                Game1.playSound("drumkit6");
            }
        }
        // Toggle buttons
        else if (easierFishingButton.containsPoint(x, y))
        {
            config.EasierFishing = !config.EasierFishing;
            saveAction();
            Game1.playSound("drumkit6");
        }
        else if (largerBarButton.containsPoint(x, y))
        {
            config.LargerFishingBar = !config.LargerFishingBar;
            saveAction();
            Game1.playSound("drumkit6");
        }
        else if (slowerFishButton.containsPoint(x, y))
        {
            config.SlowerFishMovement = !config.SlowerFishMovement;
            saveAction();
            Game1.playSound("drumkit6");
        }
        else if (alwaysPerfectButton.containsPoint(x, y))
        {
            config.AlwaysPerfect = !config.AlwaysPerfect;
            saveAction();
            Game1.playSound("drumkit6");
        }
        else if (alwaysTreasureButton.containsPoint(x, y))
        {
            config.AlwaysFindTreasure = !config.AlwaysFindTreasure;
            saveAction();
            Game1.playSound("drumkit6");
        }
        else if (instantCatchButton.containsPoint(x, y))
        {
            config.InstantCatchFish = !config.InstantCatchFish;
            saveAction();
            Game1.playSound("drumkit6");
        }
        else if (instantTreasureButton.containsPoint(x, y))
        {
            config.InstantCatchTreasure = !config.InstantCatchTreasure;
            saveAction();
            Game1.playSound("drumkit6");
        }
        else if (infiniteBaitButton.containsPoint(x, y))
        {
            config.InfiniteBait = !config.InfiniteBait;
            saveAction();
            Game1.playSound("drumkit6");
        }
        else if (infiniteTackleButton.containsPoint(x, y))
        {
            config.InfiniteTackle = !config.InfiniteTackle;
            saveAction();
            Game1.playSound("drumkit6");
        }
        else if (autoHookButton.containsPoint(x, y))
        {
            config.AutoHook = !config.AutoHook;
            saveAction();
            Game1.playSound("drumkit6");
        }
        else
        {
            base.receiveLeftClick(x, y, playSound);
        }
    }
}
