using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Objects;
using StardewValley.Monsters;
using StardewValley.Tools;
using SObject = StardewValley.Object;
using System.Collections.Generic;
using System;

namespace modtools;

public class ModConfig
{
    public int Multiplier { get; set; } = 3;
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
    private ModConfig Config = null!;
    private int EffectiveMultiplier =>
        Config.Multiplier < 2 ? 2 :
        Config.Multiplier > 50 ? 50 :
        Config.Multiplier;

    public override void Entry(IModHelper helper)
    {
        Config = helper.ReadConfig<ModConfig>();
        helper.Events.Player.InventoryChanged += OnInventoryChanged;
        this.Monitor.Log($"Loaded Multiplier: {Config.Multiplier}", LogLevel.Info);
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
                int extra = delta * EffectiveMultiplier ; // N获得，额外加(N*(倍数-1))
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

                int extra = item.Stack * EffectiveMultiplier; // N获得，额外加(N*(倍数-1))
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
}
