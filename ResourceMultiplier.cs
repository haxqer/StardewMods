using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using SObject = StardewValley.Object;

namespace modtools;

/// <summary>
/// 资源倍增处理器 / Resource multiplier handler
/// </summary>
public class ResourceMultiplier
{
    private readonly ModConfig config;
    private readonly IMonitor monitor;
    
    // 防递归锁 / Anti-recursion lock
    private bool isGivingBonus = false;

    public ResourceMultiplier(ModConfig config, IMonitor monitor)
    {
        this.config = config;
        this.monitor = monitor;
    }

    /// <summary>
    /// 获取有效倍数 / Get effective multiplier
    /// </summary>
    public int EffectiveMultiplier =>
        config.Multiplier < 3 ? 3 :
        config.Multiplier > 100 ? 100 :
        config.Multiplier;



    /// <summary>
    /// 处理库存变化 / Handle inventory changes
    /// </summary>
    public void OnInventoryChanged(InventoryChangedEventArgs e)
    {
        if (!Context.IsWorldReady || !e.IsLocalPlayer)
            return;

        if (isGivingBonus)
            return;

        // 处理堆叠数量变化 / Handle stack quantity changes
        foreach (var entry in e.QuantityChanged)
        {
            var item = entry.Item;
            if (item == null || entry.NewSize <= entry.OldSize || entry.NewSize - entry.OldSize > 1)
                continue;

            if (item is SObject obj && ItemDefinitions.IsResourceItem(obj))
            {
                int delta = entry.NewSize - entry.OldSize;
                int extra = delta * (EffectiveMultiplier - 1); // N获得，额外加(N*(倍数-1)) / N obtained, add extra (N*(multiplier-1))
                if (extra > 0)
                {
                    monitor.Log($"[ResourceMultiplier] Adding {extra} extra {obj.Name} (multiplier: {EffectiveMultiplier})", LogLevel.Info);
                    
                    var cloned = item.getOne();
                    cloned.Stack = extra;
                    isGivingBonus = true;
                    Game1.player.addItemToInventory(cloned);
                    isGivingBonus = false;
                }
            }
        }

        // 处理新增物品堆叠 / Handle new item stacks
        foreach (var item in e.Added)
        {
            if (item is SObject obj && ItemDefinitions.IsResourceItem(obj))
            {
                if (item.Stack > 1)
                {
                    continue;
                }

                int extra = item.Stack * (EffectiveMultiplier - 1); // N获得，额外加(N*(倍数-1)) / N obtained, add extra (N*(multiplier-1))
                if (extra > 0)
                {
                    monitor.Log($"[ResourceMultiplier] Adding {extra} extra {obj.Name} (multiplier: {EffectiveMultiplier})", LogLevel.Info);
                    
                    var cloned = item.getOne();
                    cloned.Stack = extra;
                    isGivingBonus = true;
                    Game1.player.addItemToInventory(cloned);
                    isGivingBonus = false;
                }
            }
        }
    }
} 