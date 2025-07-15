using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Objects;
using StardewValley.Monsters;
using StardewValley.Tools;
using System.Collections.Generic;

namespace modtools;

public class ModEntry : Mod
{
    public override void Entry(IModHelper helper)
    {
        helper.Events.Player.InventoryChanged += OnInventoryChanged;
    }

    private void OnInventoryChanged(object? sender, InventoryChangedEventArgs e)
    {
        if (!Context.IsWorldReady || !e.IsLocalPlayer)
            return;

        foreach (var item in e.Added)
        {
            if (item == null || item.Stack <= 0)
                continue;

            // 只对获得的物品进行处理
            int extra = item.Stack * 2; // 已获得1份，再加2份=3倍
            if (extra > 0)
            {
                var cloned = item.getOne();
                cloned.Stack = extra;
                Game1.player.addItemToInventory(cloned);
            }
        }
    }
}
