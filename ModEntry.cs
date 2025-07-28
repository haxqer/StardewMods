using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using SObject = StardewValley.Object;

namespace modtools;

public class ModEntry : Mod
{
    // Suppress CS8618: Config is initialized in Entry() by SMAPI
    private ModConfig Config = null!;
    
    // 处理器实例 / Handler instances
    private FishingHandler fishingHandler = null!;
    private ResourceMultiplier resourceMultiplier = null!;
    private TimeHandler timeHandler = null!;

    public override void Entry(IModHelper helper)
    {
        Config = helper.ReadConfig<ModConfig>();
        
        // 初始化处理器 / Initialize handlers
        fishingHandler = new FishingHandler(Config, helper, Monitor);
        resourceMultiplier = new ResourceMultiplier(Config, Monitor);
        timeHandler = new TimeHandler(Config);
        
        // 注册事件处理器 / Register event handlers
        helper.Events.Player.InventoryChanged += OnInventoryChanged;
        helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
        helper.Events.Input.ButtonsChanged += OnButtonsChanged;
        helper.Events.GameLoop.TimeChanged += OnTimeChanged;
        helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        helper.Events.Display.MenuChanged += OnMenuChanged;
        helper.Events.Input.ButtonPressed += OnButtonPressed;
        
        Monitor.Log($"ModTools loaded successfully - Multiplier: {Config.Multiplier}", LogLevel.Info);
        Monitor.Log($"Resource items count: {ItemDefinitions.ResourceItemIds.Count}", LogLevel.Info);
        Monitor.Log($"Fish items count: {ItemDefinitions.FishItemIds.Count}", LogLevel.Info);
    }

    /// <summary>
    /// 打印物品ID信息 / Print item ID information
    /// </summary>
    /// <param name="item">物品 / Item</param>
    /// <param name="context">上下文 / Context</param>
    private void LogItemId(Item item, string context)
    {
        if (item is SObject obj)
        {
            bool isResource = ItemDefinitions.IsResourceItem(obj);
            bool isFish = ItemDefinitions.IsFish(obj);
            string flags = "";
            if (isResource) flags += " [Resource]";
            if (isFish) flags += " [Fish]";
            
            Monitor.Log($"[ItemID] {context}: {obj.Name} (ID: {obj.ItemId}){flags}", LogLevel.Info);
        }
        else
        {
            Monitor.Log($"[ItemID] {context}: {item.Name} (Type: {item.GetType().Name})", LogLevel.Info);
        }
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
        Monitor.Log($"Configuration saved - Multiplier: {Config.Multiplier}", LogLevel.Info);
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        Monitor.Log("Game launched - initializing GMCM integration", LogLevel.Info);
        var api = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
        if (api is null)
        {
            Monitor.Log("GMCM API not found - using custom config menu only", LogLevel.Warn);
            return;
        }
        Monitor.Log("GMCM API found - registering configuration options", LogLevel.Info);
        
        RegisterGMCMOptions(api);
    }

    private void RegisterGMCMOptions(IGenericModConfigMenuApi api)
    {
        api.RegisterModConfig(
            ModManifest,
            () => Config = new ModConfig(),
            () => Helper.WriteConfig(Config)
        );
        
        // Resource multiplier
        api.RegisterClampedOption(
            ModManifest,
            Helper.Translation.Get("config.gmcm.resource-multiplier"),
            Helper.Translation.Get("config.gmcm.resource-multiplier-desc"),
            () => Config.Multiplier,
            val => { Config.Multiplier = val; Helper.WriteConfig(Config); },
            3, 100
        );
        
        // Fishing difficulty options
        api.RegisterSimpleOption(
            ModManifest,
            Helper.Translation.Get("config.gmcm.easier-fishing"),
            Helper.Translation.Get("config.gmcm.easier-fishing-desc"),
            () => Config.EasierFishing,
            val => { Config.EasierFishing = val; Helper.WriteConfig(Config); }
        );
        
        api.RegisterClampedOption(
            ModManifest,
            Helper.Translation.Get("config.gmcm.fish-difficulty-multiplier"),
            Helper.Translation.Get("config.gmcm.fish-difficulty-multiplier-desc"),
            () => (int)(Config.FishDifficultyMultiplier * 100),
            val => { Config.FishDifficultyMultiplier = val / 100f; Helper.WriteConfig(Config); },
            10, 200
        );
        
        api.RegisterClampedOption(
            ModManifest,
            Helper.Translation.Get("config.gmcm.fish-difficulty-additive"),
            Helper.Translation.Get("config.gmcm.fish-difficulty-additive-desc"),
            () => (int)Config.FishDifficultyAdditive,
            val => { Config.FishDifficultyAdditive = val; Helper.WriteConfig(Config); },
            -50, 50
        );
        
        api.RegisterSimpleOption(
            ModManifest,
            Helper.Translation.Get("config.gmcm.larger-fishing-bar"),
            Helper.Translation.Get("config.gmcm.larger-fishing-bar-desc"),
            () => Config.LargerFishingBar,
            val => { Config.LargerFishingBar = val; Helper.WriteConfig(Config); }
        );
        
        api.RegisterClampedOption(
            ModManifest,
            Helper.Translation.Get("config.gmcm.fishing-bar-height"),
            Helper.Translation.Get("config.gmcm.fishing-bar-height-desc"),
            () => Config.FishingBarHeight,
            val => { Config.FishingBarHeight = val; Helper.WriteConfig(Config); },
            60, 568
        );
        
        api.RegisterSimpleOption(
            ModManifest,
            Helper.Translation.Get("config.gmcm.slower-fish-movement"),
            Helper.Translation.Get("config.gmcm.slower-fish-movement-desc"),
            () => Config.SlowerFishMovement,
            val => { Config.SlowerFishMovement = val; Helper.WriteConfig(Config); }
        );
        
        api.RegisterClampedOption(
            ModManifest,
            Helper.Translation.Get("config.gmcm.fish-movement-speed-multiplier"),
            Helper.Translation.Get("config.gmcm.fish-movement-speed-multiplier-desc"),
            () => (int)(Config.FishMovementSpeedMultiplier * 100),
            val => { Config.FishMovementSpeedMultiplier = val / 100f; Helper.WriteConfig(Config); },
            10, 100
        );
        
        api.RegisterSimpleOption(
            ModManifest,
            Helper.Translation.Get("config.gmcm.always-perfect"),
            Helper.Translation.Get("config.gmcm.always-perfect-desc"),
            () => Config.AlwaysPerfect,
            val => { Config.AlwaysPerfect = val; Helper.WriteConfig(Config); }
        );
        
        api.RegisterSimpleOption(
            ModManifest,
            Helper.Translation.Get("config.gmcm.always-find-treasure"),
            Helper.Translation.Get("config.gmcm.always-find-treasure-desc"),
            () => Config.AlwaysFindTreasure,
            val => { Config.AlwaysFindTreasure = val; Helper.WriteConfig(Config); }
        );
        
        api.RegisterSimpleOption(
            ModManifest,
            Helper.Translation.Get("config.gmcm.instant-catch-fish"),
            Helper.Translation.Get("config.gmcm.instant-catch-fish-desc"),
            () => Config.InstantCatchFish,
            val => { Config.InstantCatchFish = val; Helper.WriteConfig(Config); }
        );
        
        api.RegisterSimpleOption(
            ModManifest,
            Helper.Translation.Get("config.gmcm.instant-catch-treasure"),
            Helper.Translation.Get("config.gmcm.instant-catch-treasure-desc"),
            () => Config.InstantCatchTreasure,
            val => { Config.InstantCatchTreasure = val; Helper.WriteConfig(Config); }
        );
        
        api.RegisterSimpleOption(
            ModManifest,
            Helper.Translation.Get("config.gmcm.infinite-bait"),
            Helper.Translation.Get("config.gmcm.infinite-bait-desc"),
            () => Config.InfiniteBait,
            val => { Config.InfiniteBait = val; Helper.WriteConfig(Config); }
        );
        
        api.RegisterSimpleOption(
            ModManifest,
            Helper.Translation.Get("config.gmcm.infinite-tackle"),
            Helper.Translation.Get("config.gmcm.infinite-tackle-desc"),
            () => Config.InfiniteTackle,
            val => { Config.InfiniteTackle = val; Helper.WriteConfig(Config); }
        );
        
        api.RegisterSimpleOption(
            ModManifest,
            Helper.Translation.Get("config.gmcm.auto-hook"),
            Helper.Translation.Get("config.gmcm.auto-hook-desc"),
            () => Config.AutoHook,
            val => { Config.AutoHook = val; Helper.WriteConfig(Config); }
        );
        
        // Time rate multiplier
        api.RegisterClampedOption(
            ModManifest,
            Helper.Translation.Get("config.gmcm.time-rate-multiplier"),
            Helper.Translation.Get("config.gmcm.time-rate-multiplier-desc"),
            () => (int)(Config.TimeRateMultiplier * 100),
            val => { Config.TimeRateMultiplier = val / 100f; Helper.WriteConfig(Config); },
            10, 200
        );
    }

    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        fishingHandler.OnUpdateTicked(e);
    }

    private void OnTimeChanged(object? sender, TimeChangedEventArgs e)
    {
        timeHandler.OnTimeChanged(e);
    }

    private void OnButtonsChanged(object? sender, ButtonsChangedEventArgs e)
    {
        if (Config.ReloadKey.JustPressed())
        {
            Config = Helper.ReadConfig<ModConfig>();
            Monitor.Log("Configuration reloaded from file", LogLevel.Info);
        }
    }

    private void OnInventoryChanged(object? sender, InventoryChangedEventArgs e)
    {
        // 打印新增物品的ID信息 / Print ID information for new items
        foreach (var item in e.Added)
        {
            LogItemId(item, "Item Added");
        }
        
        // 打印数量变化的物品ID信息 / Print ID information for quantity changed items
        foreach (var entry in e.QuantityChanged)
        {
            if (entry.Item != null)
            {
                LogItemId(entry.Item, $"Quantity Changed ({entry.OldSize} -> {entry.NewSize})");
            }
        }
        
        resourceMultiplier.OnInventoryChanged(e);
    }

    private void OnMenuChanged(object? sender, MenuChangedEventArgs e)
    {
        fishingHandler.OnMenuChanged(e);
    }
}
