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
        578, // 星之碎片 / Star Shards
        
        // 鱼类 / Fish (129-165)
        128, // 河豚 / Pufferfish
        129, // 凤尾鱼 / Anchovy
        130, // 金枪鱼 / Tuna
        131, // 沙丁鱼 / Sardine
        132, // 鲤科鱼 / Bream
        136, // 大口黑鲈 / Largemouth Bass
        137, // 小口黑鲈 / Smallmouth Bass
        138, // 虹鳟鱼 / Rainbow Trout
        139, // 鲑鱼 / Salmon
        140, // 梭子鱼 / Walleye
        141, // 鲈鱼 / Perch
        142, // 鲤鱼 / Carp
        143, // 鲶鱼 / Catfish
        144, // 梭鱼 / Pike
        145, // 太阳鱼 / Sunfish
        146, // 红鲻鱼 / Red Mullet
        147, // 鲱鱼 / Herring
        148, // 鳗鱼 / Eel
        149, // 章鱼 / Octopus
        150, // 红鲷鱼 / Red Snapper
        151, // 鱿鱼 / Squid
        152, // 海藻 / Seaweed
        153, // 绿藻 / Green Algae
        154, // 海参 / Sea Cucumber
        155, // 超级海参 / Super Cucumber
        156, // 鬼鱼 / Ghostfish
        157, // 白藻 / White Algae
        158, // 石鱼 / Stonefish
        159, // 绯红鱼 / Crimsonfish
        160, // 琵琶鱼 / Angler
        161, // 冰鳕鱼 / Ice Pip
        162, // 熔岩鳗鱼 / Lava Eel
        163, // 传说鱼 / Legend
        164, // 沙鱼 / Sandfish
        165, // 蝎子鲤鱼 / Scorpion Carp
        
        // 农产品 / Agricultural Products (174-190)
        174, // 大鸡蛋 / Large Egg
        176, // 鸡蛋 / Egg
        178, // 干草 / Hay
        180, // 鸡蛋 / Egg
        182, // 大鸡蛋 / Large Egg
        184, // 牛奶 / Milk
        186, // 大牛奶 / Large Milk
        188, // 青豆 / Green Bean
        190, // 花椰菜 / Cauliflower
        
        // 农作物 / Crops (192-289)
        192, // 土豆 / Potato
        194, // 煎蛋 / Fried Egg
        195, // 煎蛋卷 / Omelet
        196, // 沙拉 / Salad
        197, // 奶酪花椰菜 / Cheese Cauliflower
        198, // 烤鱼 / Baked Fish
        199, // 防风草汤 / Parsnip Soup
        200, // 蔬菜杂烩 / Vegetable Medley
        201, // 完整早餐 / Complete Breakfast
        202, // 炸鱿鱼 / Fried Calamari
        203, // 奇怪面包 / Strange Bun
        204, // 幸运午餐 / Lucky Lunch
        205, // 炸蘑菇 / Fried Mushroom
        206, // 披萨 / Pizza
        207, // 豆子火锅 / Bean Hotpot
        208, // 糖渍山药 / Glazed Yams
        209, // 鲤鱼惊喜 / Carp Surprise
        210, // 薯饼 / Hashbrowns
        211, // 煎饼 / Pancakes
        212, // 鲑鱼晚餐 / Salmon Dinner
        213, // 鱼卷饼 / Fish Taco
        214, // 脆皮鲈鱼 / Crispy Bass
        215, // 辣椒爆米花 / Pepper Poppers
        216, // 面包 / Bread
        218, // 冬阴功汤 / Tom Kha Soup
        219, // 鳟鱼汤 / Trout Soup
        220, // 巧克力蛋糕 / Chocolate Cake
        221, // 粉色蛋糕 / Pink Cake
        222, // 大黄派 / Rhubarb Pie
        223, // 饼干 / Cookies
        224, // 意大利面 / Spaghetti
        225, // 炸鳗鱼 / Fried Eel
        226, // 辣鳗鱼 / Spicy Eel
        227, // 生鱼片 / Sashimi
        228, // 寿司卷 / Maki Roll
        229, // 玉米饼 / Tortilla
        230, // 红盘 / Red Plate
        231, // 茄子帕尔马干酪 / Eggplant Parmesan
        232, // 米布丁 / Rice Pudding
        233, // 冰淇淋 / Ice Cream
        234, // 蓝莓馅饼 / Blueberry Tart
        235, // 秋季丰收 / Autumn's Bounty
        236, // 南瓜汤 / Pumpkin Soup
        237, // 超级餐 / Super Meal
        238, // 蔓越莓酱 / Cranberry Sauce
        239, // 填料 / Stuffing
        240, // 农夫午餐 / Farmer's Lunch
        241, // 生存汉堡 / Survival Burger
        242, // 海洋之菜 / Dish O' The Sea
        243, // 矿工美食 / Miner's Treat
        244, // 根菜拼盘 / Roots Platter
        245, // 糖 / Sugar
        246, // 小麦粉 / Wheat Flour
        247, // 油 / Oil
        248, // 大蒜 / Garlic
        250, // 羽衣甘蓝 / Kale
        251, // 茶树苗 / Tea Sapling
        252, // 大黄 / Rhubarb
        253, // 三倍浓缩咖啡 / Triple Shot Espresso
        254, // 甜瓜 / Melon
        256, // 番茄 / Tomato
        257, // 羊肚菌 / Morel
        258, // 蓝莓 / Blueberry
        259, // 蕨菜 / Fiddlehead Fern
        260, // 辣椒 / Hot Pepper
        262, // 小麦 / Wheat
        264, // 萝卜 / Radish
        265, // 海沫布丁 / Seafoam Pudding
        266, // 红甘蓝 / Red Cabbage
        267, // 比目鱼 / Flounder
        268, // 杨桃 / Starfruit
        269, // 午夜鲤鱼 / Midnight Carp
        270, // 玉米 / Corn
        271, // 糙米 / Unmilled Rice
        272, // 茄子 / Eggplant
        273, // 稻苗 / Rice Shoot
        274, // 洋蓟 / Artichoke
        275, // 古物宝箱 / Artifact Trove
        276, // 南瓜 / Pumpkin
        277, // 枯萎花束 / Wilted Bouquet
        278, // 小白菜 / Bok Choy
        279, // 魔法岩石糖 / Magic Rock Candy
        280, // 山药 / Yam
        281, // 鸡油菌 / Chanterelle
        282, // 蔓越莓 / Cranberries
        283, // 冬青 / Holly
        284, // 甜菜 / Beet
        286, // 樱桃炸弹 / Cherry Bomb
        287, // 炸弹 / Bomb
        288, // 超级炸弹 / Mega Bomb
        289, // 鸵鸟蛋 / Ostrich Egg
        
        // 金属锭 / Metal Bars (334-338)
        334, // 铜锭 / Copper Bar
        335, // 铁锭 / Iron Bar
        336, // 金锭 / Gold Bar
        337, // 铱锭 / Iridium Bar
        // Note: Refined Quartz (338) is already included above
        
        // 肥料和土壤 / Fertilizers and Soil (368-372)
        368, // 基础肥料 / Basic Fertilizer
        369, // 优质肥料 / Quality Fertilizer
        370, // 基础保水土壤 / Basic Retaining Soil
        371, // 优质保水土壤 / Quality Retaining Soil
        372, // 蛤蜊 / Clam
        
        // 海滩物品和农产品 / Beach Items and Agricultural Products (392-433)
        392, // 鹦鹉螺壳 / Nautilus Shell
        393, // 珊瑚 / Coral
        394, // 彩虹贝壳 / Rainbow Shell
        395, // 咖啡 / Coffee
        396, // 香料浆果 / Spice Berry
        397, // 海胆 / Sea Urchin
        398, // 葡萄 / Grape
        399, // 春葱 / Spring Onion
        400, // 草莓 / Strawberry
        401, // 稻草地板 / Straw Floor
        402, // 甜豌豆 / Sweet Pea
        403, // 野外零食 / Field Snack
        404, // 普通蘑菇 / Common Mushroom
        405, // 木制路径 / Wood Path
        406, // 野李子 / Wild Plum
        407, // 砾石路径 / Gravel Path
        408, // 榛子 / Hazelnut
        409, // 水晶路径 / Crystal Path
        410, // 黑莓 / Blackberry
        411, // 鹅卵石路径 / Cobblestone Path
        412, // 冬根 / Winter Root
        413, // 蓝色史莱姆蛋 / Blue Slime Egg
        414, // 水晶果 / Crystal Fruit
        415, // 踏脚石路径 / Stepping Stone Path
        416, // 雪山药 / Snow Yam
        417, // 甜宝石浆果 / Sweet Gem Berry
        418, // 番红花 / Crocus
        419, // 醋 / Vinegar
        420, // 红蘑菇 / Red Mushroom
        421, // 向日葵 / Sunflower
        422, // 紫蘑菇 / Purple Mushroom
        423, // 大米 / Rice
        424, // 奶酪 / Cheese
        425, // 仙女种子 / Fairy Seeds
        426, // 山羊奶酪 / Goat Cheese
        427, // 郁金香球茎 / Tulip Bulb
        428, // 布料 / Cloth
        429, // 爵士种子 / Jazz Seeds
        430, // 松露 / Truffle
        431, // 向日葵种子 / Sunflower Seeds
        432, // 松露油 / Truffle Oil
        433, // 咖啡豆 / Coffee Bean
        
        // 肥料和种子 / Fertilizers and Seeds (465-499)
        465, // 生长激素 / Speed-Gro
        466, // 豪华生长激素 / Deluxe Speed-Gro
        472, // 防风草种子 / Parsnip Seeds
        473, // 豆子种子 / Bean Starter
        474, // 花椰菜种子 / Cauliflower Seeds
        475, // 土豆种子 / Potato Seeds
        476, // 大蒜种子 / Garlic Seeds
        477, // 羽衣甘蓝种子 / Kale Seeds
        478, // 大黄种子 / Rhubarb Seeds
        479, // 甜瓜种子 / Melon Seeds
        480, // 番茄种子 / Tomato Seeds
        481, // 蓝莓种子 / Blueberry Seeds
        482, // 辣椒种子 / Pepper Seeds
        483, // 小麦种子 / Wheat Seeds
        484, // 萝卜种子 / Radish Seeds
        485, // 红甘蓝种子 / Red Cabbage Seeds
        486, // 杨桃种子 / Starfruit Seeds
        487, // 玉米种子 / Corn Seeds
        488, // 茄子种子 / Eggplant Seeds
        489, // 洋蓟种子 / Artichoke Seeds
        490, // 南瓜种子 / Pumpkin Seeds
        491, // 小白菜种子 / Bok Choy Seeds
        492, // 山药种子 / Yam Seeds
        493, // 蔓越莓种子 / Cranberry Seeds
        494, // 甜菜种子 / Beet Seeds
        495, // 春季种子 / Spring Seeds
        496, // 夏季种子 / Summer Seeds
        497, // 秋季种子 / Fall Seeds
        498, // 冬季种子 / Winter Seeds
        499, // 远古种子 / Ancient Seeds
        
        // 晶洞和矿物 / Geodes and Minerals (535-578)
        535, // 晶洞 / Geode
        536, // 冰冻晶洞 / Frozen Geode
        537, // 岩浆晶洞 / Magma Geode
        // Note: Minerals 538-578 are already included in the original list above
        
        // 食物和饮料 / Food and Beverages (604-618)
        604, // 李子布丁 / Plum Pudding
        605, // 洋蓟蘸酱 / Artichoke Dip
        606, // 炒菜 / Stir Fry
        607, // 烤榛子 / Roasted Hazelnuts
        608, // 南瓜派 / Pumpkin Pie
        609, // 萝卜沙拉 / Radish Salad
        610, // 水果沙拉 / Fruit Salad
        611, // 黑莓馅饼 / Blackberry Cobbler
        612, // 蔓越莓糖果 / Cranberry Candy
        613, // 苹果 / Apple
        614, // 绿茶 / Green Tea
        618, // 意式烤面包 / Bruschetta
        
        // 水果、工具和食物 / Fruits, Tools and Food (634-651)
        634, // 杏子 / Apricot
        635, // 橙子 / Orange
        636, // 桃子 / Peach
        637, // 石榴 / Pomegranate
        638, // 樱桃 / Cherry
        645, // 铱制洒水器 / Iridium Sprinkler
        648, // 凉拌卷心菜 / Coleslaw
        649, // 蕨菜烩饭 / Fiddlehead Risotto
        651, // 罂粟籽松饼 / Poppyseed Muffin
        
        // 传送图腾 / Warp Totems 
        681, // 雨图腾 / Rain Totem
        688, // 农场传送图腾 / Warp Totem: Farm
        689, // 山区传送图腾 / Warp Totem: Mountains
        690, // 海滩传送图腾 / Warp Totem: Beach
        
        // 鱼类 / Fish (698-702)
        698, // 鲟鱼 / Sturgeon
        699, // 虎鳟鱼 / Tiger Trout
        700, // 牛头鱼 / Bullhead
        701, // 罗非鱼 / Tilapia
        702, // 白鲑 / Chub
        
        // 鱼类 / Fish (704-708)
        704, // 剑鱼 / Dorado
        705, // 长鳍金枪鱼 / Albacore
        706, // 西鲱 / Shad
        707, // 鳕鱼 / Lingcod
        708, // 大比目鱼 / Halibut
        
        // 海鲜、树液和食物 / Seafood, Tree Products and Food (715-734)
        715, // 龙虾 / Lobster
        716, // 小龙虾 / Crayfish
        717, // 螃蟹 / Crab
        718, // 鸟蛤 / Cockle
        719, // 贻贝 / Mussel
        720, // 虾 / Shrimp
        721, // 蜗牛 / Snail
        722, // 海螺 / Periwinkle
        723, // 牡蛎 / Oyster
        724, // 枫糖浆 / Maple Syrup
        725, // 橡树树脂 / Oak Resin
        726, // 松焦油 / Pine Tar
        727, // 杂烩汤 / Chowder
        728, // 鱼汤 / Fish Stew
        729, // 蜗牛菜 / Escargot
        730, // 龙虾浓汤 / Lobster Bisque
        731, // 枫糖条 / Maple Bar
        732, // 蟹饼 / Crab Cakes
        733, // 虾鸡尾酒 / Shrimp Cocktail
        734, // 木跳鱼 / Woodskip
        
        // 特殊物品 / Special Items (749, 787, 802, 803, 805, 815, 819)
        749, // 万能晶洞 / Omni Geode
        787, // 电池包 / Battery Pack
        802, // 仙人掌种子 / Cactus Seeds
        803, // 铱制牛奶 / Iridium Milk
        805, // 树木肥料 / Tree Fertilizer
        815, // 茶叶 / Tea Leaves
        819, // 万能晶洞石 / Omni Geode Stone
        
        // 热带水果和鱼类 / Tropical Fruits and Fish (832, 833, 834, 836, 837, 838)
        832, // 菠萝 / Pineapple
        833, // 菠萝种子 / Pineapple Seeds
        834, // 芒果 / Mango
        836, // 黄貂鱼 / Stingray
        837, // 狮子鱼 / Lionfish
        838, // 蓝盘鱼 / Blue Discus
        
        // Qi物品 / Qi Items (886, 889, 890)
        // Note: ID 886 does not exist in items.txt
        889, // Qi果实 / Qi Fruit
        890, // Qi豆 / Qi Bean
        
        // 高级肥料 / Advanced Fertilizers (918, 919)
        918, // 超级生长激素 / Hyper Speed-Gro
        919  // 豪华肥料 / Deluxe Fertilizer
    };

    // 鱼类ID集合 / Fish ID set
    private static readonly HashSet<int> FishItemIds = new()
    {
        128, // 河豚 / Pufferfish
        129, // 凤尾鱼 / Anchovy
        130, // 金枪鱼 / Tuna
        131, // 沙丁鱼 / Sardine
        132, // 鲤科鱼 / Bream
        136, // 大口黑鲈 / Largemouth Bass
        137, // 小口黑鲈 / Smallmouth Bass
        138, // 虹鳟鱼 / Rainbow Trout
        139, // 鲑鱼 / Salmon
        140, // 梭子鱼 / Walleye
        141, // 鲈鱼 / Perch
        142, // 鲤鱼 / Carp
        143, // 鲶鱼 / Catfish
        144, // 梭鱼 / Pike
        145, // 太阳鱼 / Sunfish
        146, // 红鲻鱼 / Red Mullet
        147, // 鲱鱼 / Herring
        148, // 鳗鱼 / Eel
        149, // 章鱼 / Octopus
        150, // 红鲷鱼 / Red Snapper
        151, // 鱿鱼 / Squid
        154, // 海参 / Sea Cucumber
        155, // 超级海参 / Super Cucumber
        156, // 鬼鱼 / Ghostfish
        158, // 石鱼 / Stonefish
        159, // 绯红鱼 / Crimsonfish
        160, // 琵琶鱼 / Angler
        161, // 冰鳕鱼 / Ice Pip
        162, // 熔岩鳗鱼 / Lava Eel
        163, // 传说鱼 / Legend
        164, // 沙鱼 / Sandfish
        165, // 蝎子鲤鱼 / Scorpion Carp
        267, // 比目鱼 / Flounder
        269, // 午夜鲤鱼 / Midnight Carp
        682, // 变异鲤鱼 / Mutant Carp
        698, // 鲟鱼 / Sturgeon
        699, // 虎鳟鱼 / Tiger Trout
        700, // 牛头鱼 / Bullhead
        701, // 罗非鱼 / Tilapia
        702, // 白鲑 / Chub
        704, // 剑鱼 / Dorado
        705, // 长鳍金枪鱼 / Albacore
        706, // 西鲱 / Shad
        707, // 鳕鱼 / Lingcod
        708, // 大比目鱼 / Halibut
        715, // 龙虾 / Lobster
        716, // 小龙虾 / Crayfish
        717, // 螃蟹 / Crab
        718, // 鸟蛤 / Cockle
        719, // 贻贝 / Mussel
        720, // 虾 / Shrimp
        721, // 蜗牛 / Snail
        722, // 海螺 / Periwinkle
        723, // 牡蛎 / Oyster
        734, // 木跳鱼 / Woodskip
        775, // 冰川鱼 / Glacierfish
        795, // 虚空鲑鱼 / Void Salmon
        796, // 史莱姆鱼 / Slimejack
        798, // 午夜鱿鱼 / Midnight Squid
        799, // 幽灵鱼 / Spook Fish
        800, // 水滴鱼 / Blobfish
        836, // 黄貂鱼 / Stingray
        837, // 狮子鱼 / Lionfish
        838, // 蓝盘鱼 / Blue Discus
        898, // 绯红鱼之子 / Son of Crimsonfish
        899, // 安格勒女士 / Ms. Angler
        900, // 传说鱼II / Legend II
        901, // 放射性鲤鱼 / Radioactive Carp
        902  // 小冰川鱼 / Glacierfish Jr.
    };

    // 防递归锁
    private bool isGivingBonus = false;
    // Suppress CS8618: Config is initialized in Entry() by SMAPI
    private ModConfig Config = null!;
    private int EffectiveMultiplier =>
        Config.Multiplier < 3 ? 3 :
        Config.Multiplier > 100 ? 100 :
        Config.Multiplier;

    /// <summary>
    /// 检查物品是否为鱼类 / Check if an item is a fish
    /// </summary>
    /// <param name="item">要检查的物品 / Item to check</param>
    /// <returns>如果是鱼类返回true，否则返回false / Returns true if the item is a fish, false otherwise</returns>
    private static bool IsFish(SObject item)
    {
        return item != null && FishItemIds.Contains(item.ParentSheetIndex);
    }
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
            3, 100 // changed from 50 to 100
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
            if (config.Multiplier < 100) // changed from 50 to 100
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
