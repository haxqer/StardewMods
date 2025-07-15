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
}

public class ModConfig
{
    public int Multiplier { get; set; } = 3;
    // Fishing options
    public bool AlwaysPerfect { get; set; } = false;
    public bool AlwaysFindTreasure { get; set; } = false;
    public bool InstantCatchFish { get; set; } = false;
    public bool InstantCatchTreasure { get; set; } = false;
    public bool EasierFishing { get; set; } = false;
    public float FishDifficultyMultiplier { get; set; } = 1.0f;
    public float FishDifficultyAdditive { get; set; } = 0.0f;
    public float LossAdditive { get; set; } = 2 / 1000f;
    public bool InfiniteTackle { get; set; } = true;
    public bool InfiniteBait { get; set; } = true;
    public KeybindList ReloadKey { get; set; } = new(SButton.F5);
}

public class ModEntry : Mod
{
    // 资源类物品ID白名单
    private static readonly HashSet<int> ResourceItemIds = new()
    {
        388, // 木头
        390, // 石头
        382, // 煤炭
        771, // 纤维
        330, // 粘土
        378, // 铜矿
        380, // 铁矿
        384, // 金矿
        386, // 铱矿
        709, // 硬木
        92,  // 树液
        766, // 史莱姆
        80,  // 石英
        338, // 精炼石英
        767, // 蝙蝠翅膀
        684, // 虫肉
        768, // 太阳精华
        769  // 虚空精华
    };

    // 防递归锁
    private bool isGivingBonus = false;
    // Suppress CS8618: Config is initialized in Entry() by SMAPI
    private ModConfig Config = null!;
    private int EffectiveMultiplier =>
        Config.Multiplier < 3 ? 3 :
        Config.Multiplier > 50 ? 50 :
        Config.Multiplier;
    private readonly PerScreen<bool> BeganFishingGame = new();
    private readonly PerScreen<int> UpdateIndex = new();

    public override void Entry(IModHelper helper)
    {
        // CommonHelper.RemoveObsoleteFiles(this, "FishingMod.pdb"); // Not needed
        Config = helper.ReadConfig<ModConfig>();
        helper.Events.Player.InventoryChanged += OnInventoryChanged;
        helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
        helper.Events.Input.ButtonsChanged += OnButtonsChanged;
        this.Monitor.Log($"Loaded Multiplier: {Config.Multiplier}", LogLevel.Info);

        // 注册快捷键监听，按K键打开自定义菜单
        helper.Events.Input.ButtonPressed += OnButtonPressed;
    }

    private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
    {
        if (!Context.IsWorldReady || !Context.IsPlayerFree)
            return;
        if (e.Button == SButton.K)
        {
            Game1.activeClickableMenu = new MultiplierConfigMenu(Config, SaveConfig);
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
        api.RegisterClampedOption(
            this.ModManifest,
            "Multiplier",
            "3-50",
            () => Config.Multiplier,
            val => { Config.Multiplier = val; Helper.WriteConfig(Config); },
            3, 50
        );
        // Add fish difficulty multiplier config (scaled by 100 for int slider)
        api.RegisterClampedOption(
            this.ModManifest,
            "FishDifficultyMultiplier",
            "0.1-2.0 (lower = easier)",
            () => (int)(Config.FishDifficultyMultiplier * 100),
            val => { Config.FishDifficultyMultiplier = val / 100f; Helper.WriteConfig(Config); },
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
            // Begin fishing game
            if (!BeganFishingGame.Value && UpdateIndex.Value > 15)
            {
                // Do these things once per fishing minigame, 1/4 second after it updates
                bobber.difficulty *= Config.FishDifficultyMultiplier;
                bobber.difficulty += Config.FishDifficultyAdditive;
                if (Config.AlwaysFindTreasure)
                    bobber.treasure = true;
                if (Config.InstantCatchFish)
                {
                    if (bobber.treasure)
                        bobber.treasureCaught = true;
                    bobber.distanceFromCatching += 100;
                }
                if (Config.InstantCatchTreasure)
                    if (bobber.treasure || Config.AlwaysFindTreasure)
                        bobber.treasureCaught = true;
                if (Config.EasierFishing)
                {
                    bobber.difficulty = Math.Max(15, Math.Max(bobber.difficulty, 60));
                    bobber.motionType = 2;
                }
                BeganFishingGame.Value = true;
            }
            if (UpdateIndex.Value < 20)
                UpdateIndex.Value++;
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
                // 1. 降低难度（可配置）
                var fishField = this.Helper.Reflection.GetField<object>(bobberBar, "fish");
                var fish = fishField.GetValue();
                var fishType = fish.GetType();
                var difficultyProp = fishType.GetProperty("difficulty");
                if (difficultyProp != null)
                {
                    object? value = difficultyProp.GetValue(fish);
                    if (value is int originalDifficulty)
                    {
                        int newDifficulty = Math.Max(1, (int)(originalDifficulty * Config.FishDifficultyMultiplier));
                        difficultyProp.SetValue(fish, newDifficulty);
                    }
                }
                // 2. 增大绿色条
                var barHeightField = this.Helper.Reflection.GetField<int>(bobberBar, "bobberBarHeight");
                barHeightField.SetValue(400); // 最大568，默认60-300
                // 3. 可选：自动完成小游戏
                // var distanceFromCatchingField = this.Helper.Reflection.GetField<float>(bobberBar, "distanceFromCatching");
                // distanceFromCatchingField.SetValue(1f); // 直接满进度
            }
            catch (Exception ex)
            {
                this.Monitor.Log($"Error simplifying fishing: {ex}", LogLevel.Error);
            }
        }
    }
}

// 自定义Multiplier配置菜单
public class MultiplierConfigMenu : IClickableMenu
{
    private ModConfig config;
    private Action saveAction;
    private ClickableComponent plusButton;
    private ClickableComponent minusButton;
    private ClickableComponent plusFishButton;
    private ClickableComponent minusFishButton;

    public MultiplierConfigMenu(ModConfig config, Action saveAction)
        : base(Game1.viewport.Width / 2 - 150, Game1.viewport.Height / 2 - 75, 300, 200)
    {
        this.config = config;
        this.saveAction = saveAction;
        plusButton = new ClickableComponent(new Rectangle(xPositionOnScreen + 210, yPositionOnScreen + 70, 32, 32), "Plus");
        minusButton = new ClickableComponent(new Rectangle(xPositionOnScreen + 80, yPositionOnScreen + 70, 32, 32), "Minus");
        plusFishButton = new ClickableComponent(new Rectangle(xPositionOnScreen + 210, yPositionOnScreen + 120, 32, 32), "PlusFish");
        minusFishButton = new ClickableComponent(new Rectangle(xPositionOnScreen + 80, yPositionOnScreen + 120, 32, 32), "MinusFish");
    }

    public override void draw(Microsoft.Xna.Framework.Graphics.SpriteBatch b)
    {
        base.draw(b);
        // 背景
        IClickableMenu.drawTextureBox(b, xPositionOnScreen, yPositionOnScreen, width, height, Color.White);
        // 标题
        b.DrawString(Game1.smallFont, "Resource Multiplier", new Vector2(xPositionOnScreen + width / 2 - 110, yPositionOnScreen + 20), Color.Black);
        // 当前值
        b.DrawString(Game1.smallFont, config.Multiplier.ToString(), new Vector2(xPositionOnScreen + 140, yPositionOnScreen + 70), Color.Black);
        // 加减按钮（直接用文本）
        b.DrawString(Game1.smallFont, "+", new Vector2(xPositionOnScreen + 210, yPositionOnScreen + 70), Color.Black);
        b.DrawString(Game1.smallFont, "-", new Vector2(xPositionOnScreen + 80, yPositionOnScreen + 70), Color.Black);
        // 鱼难度倍数
        b.DrawString(Game1.smallFont, $"Fish Difficulty x{config.FishDifficultyMultiplier:F2}", new Vector2(xPositionOnScreen + 60, yPositionOnScreen + 120), Color.Black);
        b.DrawString(Game1.smallFont, "+", new Vector2(xPositionOnScreen + 210, yPositionOnScreen + 170), Color.Black);
        b.DrawString(Game1.smallFont, "-", new Vector2(xPositionOnScreen + 80, yPositionOnScreen + 170), Color.Black);
        // 鼠标
        drawMouse(b);
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
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
        else
        {
            base.receiveLeftClick(x, y, playSound);
        }
    }
}
