using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Tools;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Utilities;
using SObject = StardewValley.Object;

namespace modtools;

/// <summary>
/// 钓鱼游戏处理器 / Fishing game handler
/// </summary>
public class FishingHandler
{
    private readonly ModConfig config;
    private readonly IModHelper helper;
    private readonly IMonitor monitor;
    private readonly PerScreen<bool> beganFishingGame = new();
    private readonly PerScreen<int> updateIndex = new();

    public FishingHandler(ModConfig config, IModHelper helper, IMonitor monitor)
    {
        this.config = config;
        this.helper = helper;
        this.monitor = monitor;
    }

    /// <summary>
    /// 打印钓鱼获得的物品信息 / Print fishing catch item information
    /// </summary>
    /// <param name="item">获得的物品 / Caught item</param>
    /// <param name="isTreasure">是否为宝藏 / Whether it's a treasure</param>
    private void LogFishingCatch(Item item, bool isTreasure = false)
    {
        if (item is SObject obj)
        {
            string itemType = isTreasure ? "Treasure" : "Fish";
            monitor.Log($"[FishingLog] Caught {itemType}: {obj.Name} (ID: {obj.ItemId}, QualifiedID: {obj.QualifiedItemId}), Stack: {obj.Stack}", LogLevel.Info);
        }
        else
        {
            string itemType = isTreasure ? "Treasure" : "Fish";
            monitor.Log($"[FishingLog] Caught {itemType}: {item.Name} (Type: {item.GetType().Name}), Stack: {item.Stack}", LogLevel.Info);
        }
    }

    /// <summary>
    /// 处理钓鱼游戏更新 / Handle fishing game updates
    /// </summary>
    public void OnUpdateTicked(UpdateTickedEventArgs e)
    {
        if (!Context.IsWorldReady)
            return;
        
        // 无限鱼饵和鱼钩 / Infinite bait and tackle
        if (e.IsOneSecond && (config.InfiniteBait || config.InfiniteTackle))
        {
            HandleInfiniteBaitAndTackle();
        }
        
        // 钓鱼小游戏逻辑 / Fishing minigame logic
        if (Game1.activeClickableMenu is BobberBar bobber)
        {
            HandleFishingMinigame(bobber);
        }
        else
        {
            // 结束钓鱼游戏 / End fishing game
            beganFishingGame.Value = false;
            updateIndex.Value = 0;
        }
    }

    /// <summary>
    /// 处理无限鱼饵和鱼钩 / Handle infinite bait and tackle
    /// </summary>
    private void HandleInfiniteBaitAndTackle()
    {
        if (Game1.player.CurrentTool is FishingRod rod)
        {
            if (config.InfiniteBait && rod.attachments?.Length > 0 && rod.attachments[0] != null)
            {
                var bait = rod.attachments[0];
                monitor.Log($"[FishingLog] Infinite bait active: {bait.Name} (ID: {bait.ItemId}, QualifiedID: {bait.QualifiedItemId})", LogLevel.Debug);
                rod.attachments[0].Stack = rod.attachments[0].maximumStackSize();
            }
            if (config.InfiniteTackle && rod.attachments?.Length > 1 && rod.attachments[1] != null)
            {
                var tackle = rod.attachments[1];
                monitor.Log($"[FishingLog] Infinite tackle active: {tackle.Name} (ID: {tackle.ItemId}, QualifiedID: {tackle.QualifiedItemId})", LogLevel.Debug);
                rod.attachments[1].uses.Value = 0;
            }
        }
    }

    /// <summary>
    /// 处理钓鱼小游戏 / Handle fishing minigame
    /// </summary>
    private void HandleFishingMinigame(BobberBar bobber)
    {
        // 开始钓鱼游戏 - 只应用修改一次 / Begin fishing game - only apply modifications once
        if (!beganFishingGame.Value && updateIndex.Value > 15)
        {
            monitor.Log($"[FishingLog] Starting fishing minigame - Difficulty: {bobber.difficulty}", LogLevel.Info);
            
            // 存储原始难度用于奖励计算 / Store original difficulty for reward calculation
            var originalDifficulty = bobber.difficulty;
            
            // 应用难度修改（这些只影响小游戏） / Apply difficulty modifications (these affect minigame only)
            if (config.EasierFishing)
            {
                bobber.motionType = 2; // 更容易的运动模式 / Easier motion pattern
                monitor.Log("[FishingLog] Easier fishing mode enabled", LogLevel.Info);
            }
            
            // 应用难度倍数和加法（限制在合理值内） / Apply difficulty multiplier and additive (clamped to reasonable values)
            float newDifficulty = originalDifficulty * config.FishDifficultyMultiplier + config.FishDifficultyAdditive;
            bobber.difficulty = Math.Max(1, Math.Min(100, (int)newDifficulty));
            
            // 应用便利选项 / Apply convenience options
            if (config.AlwaysFindTreasure)
            {
                bobber.treasure = true;
                monitor.Log("[FishingLog] Always find treasure enabled", LogLevel.Info);
            }
                
            if (config.InstantCatchFish)
            {
                if (bobber.treasure)
                    bobber.treasureCaught = true;
                bobber.distanceFromCatching += 100;
                monitor.Log("[FishingLog] Instant catch fish enabled", LogLevel.Info);
            }
            
            if (config.InstantCatchTreasure)
            {
                if (bobber.treasure || config.AlwaysFindTreasure)
                    bobber.treasureCaught = true;
                monitor.Log("[FishingLog] Instant catch treasure enabled", LogLevel.Info);
            }
            
            beganFishingGame.Value = true;
        }
        
        if (updateIndex.Value < 20)
            updateIndex.Value++;
            
        // 小游戏期间的持续效果 / Continuous effects during minigame
        if (config.AlwaysPerfect)
            bobber.perfect = true;
            
        if (!bobber.bobberInBar)
            bobber.distanceFromCatching += config.LossAdditive;
    }

    /// <summary>
    /// 处理菜单变化 / Handle menu changes
    /// </summary>
    public void OnMenuChanged(MenuChangedEventArgs e)
    {
        if (e.NewMenu is BobberBar bobberBar)
        {
            monitor.Log("[FishingLog] Fishing minigame started", LogLevel.Info);
            
            try
            {
                // 应用钓鱼条高度修改 / Apply fishing bar height modification
                if (config.LargerFishingBar)
                {
                    var barHeightField = helper.Reflection.GetField<int>(bobberBar, "bobberBarHeight");
                    if (barHeightField != null)
                    {
                        barHeightField.SetValue(config.FishingBarHeight);
                        monitor.Log($"[FishingLog] Fishing bar height set to: {config.FishingBarHeight}", LogLevel.Debug);
                    }
                }
                
                // 应用鱼类移动速度修改 / Apply fish movement speed modification
                if (config.SlowerFishMovement)
                {
                    var fishField = helper.Reflection.GetField<object>(bobberBar, "fish");
                    if (fishField != null)
                    {
                        var fish = fishField.GetValue();
                        if (fish != null)
                        {
                            var fishType = fish.GetType();
                            
                            // 尝试修改鱼类移动速度 / Try to modify fish movement speed
                            var speedField = fishType.GetField("speed");
                            if (speedField != null)
                            {
                                var currentSpeed = speedField.GetValue(fish);
                                if (currentSpeed is float speed)
                                {
                                    speedField.SetValue(fish, speed * config.FishMovementSpeedMultiplier);
                                    monitor.Log($"[FishingLog] Fish movement speed modified: {speed} -> {speed * config.FishMovementSpeedMultiplier}", LogLevel.Debug);
                                }
                            }
                            
                            // 尝试修改鱼类移动模式速度 / Try to modify fish movement pattern speed
                            var patternSpeedField = fishType.GetField("patternSpeed");
                            if (patternSpeedField != null)
                            {
                                var currentPatternSpeed = patternSpeedField.GetValue(fish);
                                if (currentPatternSpeed is float patternSpeed)
                                {
                                    patternSpeedField.SetValue(fish, patternSpeed * config.FishMovementSpeedMultiplier);
                                    monitor.Log($"[FishingLog] Fish pattern speed modified: {patternSpeed} -> {patternSpeed * config.FishMovementSpeedMultiplier}", LogLevel.Debug);
                                }
                            }
                        }
                    }
                }
                
                // 自动钓鱼功能 / Auto-hook functionality
                if (config.AutoHook)
                {
                    // 这需要在UpdateTicked方法中实现 / This would need to be implemented in the UpdateTicked method
                    // 因为我们需要持续检查鱼是否咬钩 / as we need to continuously check for fish biting
                    monitor.Log("[FishingLog] Auto-hook enabled", LogLevel.Info);
                }
            }
            catch (Exception ex)
            {
                monitor.Log($"Error modifying fishing minigame: {ex}", LogLevel.Error);
            }
        }
        else if (e.OldMenu is BobberBar)
        {
            // 钓鱼游戏结束，检查是否获得了物品 / Fishing game ended, check if items were obtained
            monitor.Log("[FishingLog] Fishing minigame ended", LogLevel.Info);
            
            // 这里可以添加检查玩家最近获得的物品的逻辑
            // 但由于事件顺序问题，最好在InventoryChanged事件中处理
        }
    }
} 