using System.Collections.Generic;
using StardewValley;
using SObject = StardewValley.Object;

namespace modtools;

/// <summary>
/// 物品ID定义类 / Item ID definitions
/// </summary>
public static class ItemDefinitions
{
    // 资源类物品ID白名单 / Resource item ID whitelist
    public static readonly HashSet<string> ResourceItemIds = new()
    {
        // ===== 基础资源 / Basic Resources =====
        // 木材和石头 / Wood and Stone
        "388", // 木头 / Wood
        "390", // 石头 / Stone
        "709", // 硬木 / Hardwood
        "92",  // 树液 / Sap
        
        // 矿石 / Ores
        "378", // 铜矿 / Copper Ore
        "380", // 铁矿 / Iron Ore
        "384", // 金矿 / Gold Ore
        "386", // 铱矿 / Iridium Ore
        "382", // 煤炭 / Coal
        "909", // 放射性矿石 / Radioactive Ore
        
        // 金属锭 / Metal Bars
        "334", // 铜锭 / Copper Bar
        "335", // 铁锭 / Iron Bar
        "336", // 金锭 / Gold Bar
        "337", // 铱锭 / Iridium Bar
        "910", // 放射性锭 / Radioactive Bar
        
        // 其他基础材料 / Other Basic Materials
        "309", // 橡子 / Acorn
        "310", // 枫树种子 / Maple Seed
        "311", // 松果 / Pine Cone
        "771", // 纤维 / Fiber
        "330", // 粘土 / Clay
        "80",  // 石英 / Quartz
        "Moss", // 苔藓 / Moss
        "MossySeed", // 苔藓种子 / Mossy Seed
        "338", // 精炼石英 / Refined Quartz
        "766", // 史莱姆 / Slime
        "767", // 蝙蝠翅膀 / Bat Wing
        "684", // 虫肉 / Bug Meat
        "768", // 太阳精华 / Solar Essence
        "769", // 虚空精华 / Void Essence
        "772", // 大蒜油 / Oil of Garlic
        "773", // 生命药水 / Life Elixir
        
        // ===== 宝石 / Gems =====
        "60",  // 绿宝石 / Emerald
        "62",  // 海蓝宝石 / Aquamarine
        "64",  // 红宝石 / Ruby
        "66",  // 紫水晶 / Amethyst
        "68",  // 黄玉 / Topaz
        "70",  // 翡翠 / Jade
        "72",  // 钻石 / Diamond
        "74",  // 五彩碎片 / Prismatic Shard
        "82",  // 火石英 / Fire Quartz
        "84",  // 冰冻泪滴 / Frozen Tear
        "86",  // 地晶 / Earth Crystal
        
        // ===== 矿物 / Minerals =====
        "538", // 铝矿 / Alamite
        "539", // 铋矿 / Bixite
        "540", // 重晶石 / Baryte
        "541", // 蓝晶石 / Aerinite
        "542", // 方解石 / Calcite
        "543", // 白云石 / Dolomite
        "544", // 钙长石 / Esperite
        "545", // 氟磷灰石 / Fluorapatite
        "546", // 宝石矿 / Geminite
        "547", // 日光榴石 / Helvite
        "548", // 蓝方石 / Jamborite
        "549", // 绿帘石 / Jagoite
        "550", // 蓝晶石 / Kyanite
        "551", // 月长石 / Lunarite
        "552", // 孔雀石 / Malachite
        "553", // 海王石 / Neptunite
        "554", // 柠檬石 / Lemon Stone
        "555", // 猫眼石 / Nekoite
        "556", // 雌黄 / Orpiment
        "557", // 石化史莱姆 / Petrified Slime
        "558", // 雷蛋 / Thunder Egg
        "559", // 黄铁矿 / Pyrite
        "560", // 海洋石 / Ocean Stone
        "561", // 幽灵水晶 / Ghost Crystal
        "562", // 虎眼石 / Tigerseye
        "563", // 碧玉 / Jasper
        "564", // 蛋白石 / Opal
        "565", // 火蛋白石 / Fire Opal
        "566", // 天青石 / Celestine
        "567", // 大理石 / Marble
        "568", // 砂岩 / Sandstone
        "569", // 花岗岩 / Granite
        "570", // 玄武岩 / Basalt
        "571", // 石灰岩 / Limestone
        "572", // 皂石 / Soapstone
        "573", // 赤铁矿 / Hematite
        "574", // 泥岩 / Mudstone
        "575", // 黑曜石 / Obsidian
        "576", // 板岩 / Slate
        "577", // 仙石 / Fairy Stone
        "578", // 星之碎片 / Star Shards
        
        // ===== 晶洞 / Geodes =====
        "535", // 晶洞 / Geode
        "536", // 冰冻晶洞 / Frozen Geode
        "537", // 岩浆晶洞 / Magma Geode
        "749", // 万能晶洞 / Omni Geode
        "819", // 万能晶洞石 / Omni Geode Stone
        
        // ===== 农作物和种子 / Crops and Seeds =====
        // 基础农作物 / Basic Crops
        "192", // 土豆 / Potato
        "248", // 大蒜 / Garlic
        "250", // 羽衣甘蓝 / Kale
        "252", // 大黄 / Rhubarb
        "254", // 甜瓜 / Melon
        "256", // 番茄 / Tomato
        "258", // 蓝莓 / Blueberry
        "260", // 辣椒 / Hot Pepper
        "262", // 小麦 / Wheat
        "264", // 萝卜 / Radish
        "266", // 红甘蓝 / Red Cabbage
        "268", // 杨桃 / Starfruit
        "270", // 玉米 / Corn
        "271", // 糙米 / Unmilled Rice
        "272", // 茄子 / Eggplant
        "273", // 稻苗 / Rice Shoot
        "274", // 洋蓟 / Artichoke
        "276", // 南瓜 / Pumpkin
        "278", // 小白菜 / Bok Choy
        "280", // 山药 / Yam
        "284", // 甜菜 / Beet
        "398", // 葡萄 / Grape
        "400", // 草莓 / Strawberry
        "423", // 大米 / Rice
        "454", // 远古水果 / Ancient Fruit
        "304", // 啤酒花 / Hops
        
        // 新作物 / New Crops
        "Carrot", // 胡萝卜 / Carrot
        "Broccoli", // 西兰花 / Broccoli
        "Powdermelon", // 粉瓜 / Powdermelon
        "SummerSquash", // 夏南瓜 / Summer Squash
        
        // 种子 / Seeds
        "472", // 防风草种子 / Parsnip Seeds
        "473", // 豆子种子 / Bean Starter
        "474", // 花椰菜种子 / Cauliflower Seeds
        "475", // 土豆种子 / Potato Seeds
        "476", // 大蒜种子 / Garlic Seeds
        "477", // 羽衣甘蓝种子 / Kale Seeds
        "478", // 大黄种子 / Rhubarb Seeds
        "479", // 甜瓜种子 / Melon Seeds
        "480", // 番茄种子 / Tomato Seeds
        "481", // 蓝莓种子 / Blueberry Seeds
        "482", // 辣椒种子 / Pepper Seeds
        "483", // 小麦种子 / Wheat Seeds
        "484", // 萝卜种子 / Radish Seeds
        "485", // 红甘蓝种子 / Red Cabbage Seeds
        "486", // 杨桃种子 / Starfruit Seeds
        "487", // 玉米种子 / Corn Seeds
        "488", // 茄子种子 / Eggplant Seeds
        "489", // 洋蓟种子 / Artichoke Seeds
        "490", // 南瓜种子 / Pumpkin Seeds
        "491", // 小白菜种子 / Bok Choy Seeds
        "492", // 山药种子 / Yam Seeds
        "493", // 蔓越莓种子 / Cranberry Seeds
        "494", // 甜菜种子 / Beet Seeds
        "495", // 春季种子 / Spring Seeds
        "496", // 夏季种子 / Summer Seeds
        "497", // 秋季种子 / Fall Seeds
        "498", // 冬季种子 / Winter Seeds
        "347", // 稀有种子 / Rare Seed
        "499", // 远古种子 / Ancient Seeds
        "802", // 仙人掌种子 / Cactus Seeds
        "833", // 菠萝种子 / Pineapple Seeds
        "MysticTreeSeed", // 神秘树种子 / Mystic Tree Seed
        
        // 热带水果 / Tropical Fruits
        "88",  // 椰子 / Coconut
        "90",  // 仙人掌果 / Cactus Fruit
        "91",  // 香蕉 / Banana
        "832", // 菠萝 / Pineapple
        "834", // 芒果 / Mango
        
        // 树果 / Tree Fruits
        "634", // 杏子 / Apricot
        "635", // 橙子 / Orange
        "636", // 桃子 / Peach
        "637", // 石榴 / Pomegranate
        "638", // 樱桃 / Cherry
        "613", // 苹果 / Apple
        
        // ===== 农产品 / Agricultural Products =====
        // 动物产品 / Animal Products
        "174", // 大鸡蛋 / Large Egg
        "176", // 鸡蛋 / Egg
        "180", // 鸡蛋 / Egg
        "182", // 大鸡蛋 / Large Egg
        "184", // 牛奶 / Milk
        "186", // 大牛奶 / Large Milk
        "424", // 奶酪 / Cheese
        "426", // 山羊奶酪 / Goat Cheese
        "436", // 羊奶 / Goat Milk
        "438", // 大羊奶 / Large Goat Milk
        "803", // 铱制牛奶 / Iridium Milk
        "289", // 鸵鸟蛋 / Ostrich Egg
        "305", // 虚空蛋 / Void Egg
        "440", // 羊毛 / Wool
        "442", // 鸭蛋 / Duck Egg
        "444", // 鸭毛 / Duck Feather
        
        // 其他农产品 / Other Agricultural Products
        "178", // 干草 / Hay
        "188", // 青豆 / Green Bean
        "190", // 花椰菜 / Cauliflower
        "430", // 松露 / Truffle
        "340", // 蜂蜜 / Honey
        "432", // 松露油 / Truffle Oil
        
        // ===== 鱼类 / Fish =====
        // 基础鱼类 / Basic Fish
        "128", // 河豚 / Pufferfish
        "129", // 凤尾鱼 / Anchovy
        "130", // 金枪鱼 / Tuna
        "131", // 沙丁鱼 / Sardine
        "132", // 鲤科鱼 / Bream
        "136", // 大口黑鲈 / Largemouth Bass
        "137", // 小口黑鲈 / Smallmouth Bass
        "138", // 虹鳟鱼 / Rainbow Trout
        "139", // 鲑鱼 / Salmon
        "140", // 梭子鱼 / Walleye
        "141", // 鲈鱼 / Perch
        "142", // 鲤鱼 / Carp
        "143", // 鲶鱼 / Catfish
        "144", // 梭鱼 / Pike
        "145", // 太阳鱼 / Sunfish
        "146", // 红鲻鱼 / Red Mullet
        "147", // 鲱鱼 / Herring
        "148", // 鳗鱼 / Eel
        "149", // 章鱼 / Octopus
        "150", // 红鲷鱼 / Red Snapper
        "151", // 鱿鱼 / Squid
        "154", // 海参 / Sea Cucumber
        "155", // 超级海参 / Super Cucumber
        "156", // 鬼鱼 / Ghostfish
        "158", // 石鱼 / Stonefish
        "159", // 绯红鱼 / Crimsonfish
        "160", // 琵琶鱼 / Angler
        "161", // 冰鳕鱼 / Ice Pip
        "162", // 熔岩鳗鱼 / Lava Eel
        "163", // 传说鱼 / Legend
        "164", // 沙鱼 / Sandfish
        "165", // 蝎子鲤鱼 / Scorpion Carp
        
        // 高级鱼类 / Advanced Fish
        "267", // 比目鱼 / Flounder
        "269", // 午夜鲤鱼 / Midnight Carp
        "698", // 鲟鱼 / Sturgeon
        "699", // 虎鳟鱼 / Tiger Trout
        "700", // 牛头鱼 / Bullhead
        "701", // 罗非鱼 / Tilapia
        "702", // 白鲑 / Chub
        "704", // 剑鱼 / Dorado
        "705", // 长鳍金枪鱼 / Albacore
        "706", // 西鲱 / Shad
        "707", // 鳕鱼 / Lingcod
        "708", // 大比目鱼 / Halibut
        "734", // 木跳鱼 / Woodskip
        
        // 热带鱼类 / Tropical Fish
        "836", // 黄貂鱼 / Stingray
        "837", // 狮子鱼 / Lionfish
        "838", // 蓝盘鱼 / Blue Discus
        
        // 传奇鱼类 / Legendary Fish
        "775", // 冰川鱼 / Glacierfish
        "795", // 虚空鲑鱼 / Void Salmon
        "796", // 史莱姆鱼 / Slimejack
        "798", // 午夜鱿鱼 / Midnight Squid
        "799", // 幽灵鱼 / Spook Fish
        "800", // 水滴鱼 / Blobfish
        "898", // 绯红鱼之子 / Son of Crimsonfish
        "899", // 安格勒女士 / Ms. Angler
        "900", // 传说鱼II / Legend II
        "901", // 放射性鲤鱼 / Radioactive Carp
        "902", // 小冰川鱼 / Glacierfish Jr.
        
        // ===== 海鲜 / Seafood =====
        "715", // 龙虾 / Lobster
        "716", // 小龙虾 / Crayfish
        "717", // 螃蟹 / Crab
        "718", // 鸟蛤 / Cockle
        "719", // 贻贝 / Mussel
        "720", // 虾 / Shrimp
        "721", // 蜗牛 / Snail
        "722", // 海螺 / Periwinkle
        "723", // 牡蛎 / Oyster
        "372", // 蛤蜊 / Clam
        
        // ===== 海滩物品 / Beach Items =====
        "392", // 鹦鹉螺壳 / Nautilus Shell
        "393", // 珊瑚 / Coral
        "394", // 彩虹贝壳 / Rainbow Shell
        "396", // 香料浆果 / Spice Berry
        "397", // 海胆 / Sea Urchin
        "152", // 海藻 / Seaweed
        "153", // 绿藻 / Green Algae
        "157", // 白藻 / White Algae
        
        // ===== 采集物品 / Foraged Items =====
        "18",  // 水仙花 / Daffodil
        "20",  // 韭葱 / Leek
        "22",  // 蒲公英 / Dandelion
        "24",  // 防风草 / Parsnip
        "78",  // 洞穴胡萝卜 / Cave Carrot
        "404", // 普通蘑菇 / Common Mushroom
        "406", // 野李子 / Wild Plum
        "408", // 榛子 / Hazelnut
        "410", // 黑莓 / Blackberry
        "412", // 冬根 / Winter Root
        "414", // 水晶果 / Crystal Fruit
        "416", // 雪山药 / Snow Yam
        "417", // 甜宝石浆果 / Sweet Gem Berry
        "418", // 番红花 / Crocus
        "420", // 红蘑菇 / Red Mushroom
        "421", // 向日葵 / Sunflower
        "422", // 紫蘑菇 / Purple Mushroom
        "425", // 仙女种子 / Fairy Seeds
        "427", // 郁金香球茎 / Tulip Bulb
        "429", // 爵士种子 / Jazz Seeds
        "431", // 向日葵种子 / Sunflower Seeds
        "433", // 咖啡豆 / Coffee Bean
        "257", // 羊肚菌 / Morel
        "259", // 蕨菜 / Fiddlehead Fern
        "281", // 鸡油菌 / Chanterelle
        "282", // 蔓越莓 / Cranberries
        "283", // 冬青 / Holly
        "296", // 鲑鱼莓 / Salmonberry
        "376", // 罂粟花 / Poppy
        "396", // 香料浆果 / Spice Berry
        "398", // 葡萄 / Grape
        "399", // 春葱 / Spring Onion
        "402", // 甜豌豆 / Sweet Pea
        "406", // 野李子 / Wild Plum
        "408", // 榛子 / Hazelnut
        "410", // 黑莓 / Blackberry
        "414", // 水晶果 / Crystal Fruit
        "418", // 番红花 / Crocus
        "421", // 向日葵 / Sunflower
        "815", // 茶叶 / Tea Leaves
        
        // ===== 树液产品 / Tree Products =====
        "724", // 枫糖浆 / Maple Syrup
        "725", // 橡树树脂 / Oak Resin
        "726", // 松焦油 / Pine Tar
        
        // ===== 肥料 / Fertilizers =====
        "368", // 基础肥料 / Basic Fertilizer
        "369", // 优质肥料 / Quality Fertilizer
        "370", // 基础保水土壤 / Basic Retaining Soil
        "371", // 优质保水土壤 / Quality Retaining Soil
        "465", // 生长激素 / Speed-Gro
        "466", // 豪华生长激素 / Deluxe Speed-Gro
        "805", // 树木肥料 / Tree Fertilizer
        "918", // 超级生长激素 / Hyper Speed-Gro
        "919", // 豪华肥料 / Deluxe Fertilizer
        
        // ===== 食物和饮料 / Food and Beverages =====
        // 烹饪食物 / Cooked Foods
        "194", // 煎蛋 / Fried Egg
        "195", // 煎蛋卷 / Omelet
        "196", // 沙拉 / Salad
        "197", // 奶酪花椰菜 / Cheese Cauliflower
        "198", // 烤鱼 / Baked Fish
        "199", // 防风草汤 / Parsnip Soup
        "200", // 蔬菜杂烩 / Vegetable Medley
        "201", // 完整早餐 / Complete Breakfast
        "202", // 炸鱿鱼 / Fried Calamari
        "203", // 奇怪面包 / Strange Bun
        "204", // 幸运午餐 / Lucky Lunch
        "205", // 炸蘑菇 / Fried Mushroom
        "206", // 披萨 / Pizza
        "207", // 豆子火锅 / Bean Hotpot
        "208", // 糖渍山药 / Glazed Yams
        "209", // 鲤鱼惊喜 / Carp Surprise
        "210", // 薯饼 / Hashbrowns
        "211", // 煎饼 / Pancakes
        "212", // 鲑鱼晚餐 / Salmon Dinner
        "213", // 鱼卷饼 / Fish Taco
        "214", // 脆皮鲈鱼 / Crispy Bass
        "215", // 辣椒爆米花 / Pepper Poppers
        "216", // 面包 / Bread
        "218", // 冬阴功汤 / Tom Kha Soup
        "219", // 鳟鱼汤 / Trout Soup
        "220", // 巧克力蛋糕 / Chocolate Cake
        "221", // 粉色蛋糕 / Pink Cake
        "222", // 大黄派 / Rhubarb Pie
        "223", // 饼干 / Cookies
        "224", // 意大利面 / Spaghetti
        "225", // 炸鳗鱼 / Fried Eel
        "226", // 辣鳗鱼 / Spicy Eel
        "227", // 生鱼片 / Sashimi
        "228", // 寿司卷 / Maki Roll
        "229", // 玉米饼 / Tortilla
        "230", // 红盘 / Red Plate
        "231", // 茄子帕尔马干酪 / Eggplant Parmesan
        "232", // 米布丁 / Rice Pudding
        "233", // 冰淇淋 / Ice Cream
        "234", // 蓝莓馅饼 / Blueberry Tart
        "235", // 秋季丰收 / Autumn's Bounty
        "236", // 南瓜汤 / Pumpkin Soup
        "237", // 超级餐 / Super Meal
        "238", // 蔓越莓酱 / Cranberry Sauce
        "239", // 填料 / Stuffing
        "240", // 农夫午餐 / Farmer's Lunch
        "241", // 生存汉堡 / Survival Burger
        "242", // 海洋之菜 / Dish O' The Sea
        "243", // 矿工美食 / Miner's Treat
        "244", // 根菜拼盘 / Roots Platter
        "265", // 海沫布丁 / Seafoam Pudding
        "648", // 凉拌卷心菜 / Coleslaw
        "649", // 蕨菜烩饭 / Fiddlehead Risotto
        "651", // 罂粟籽松饼 / Poppyseed Muffin
        "727", // 杂烩汤 / Chowder
        "728", // 鱼汤 / Fish Stew
        "729", // 蜗牛菜 / Escargot
        "730", // 龙虾浓汤 / Lobster Bisque
        "731", // 枫糖条 / Maple Bar
        "732", // 蟹饼 / Crab Cakes
        "733", // 虾鸡尾酒 / Shrimp Cocktail
        "604", // 李子布丁 / Plum Pudding
        "605", // 洋蓟蘸酱 / Artichoke Dip
        "606", // 炒菜 / Stir Fry
        "607", // 烤榛子 / Roasted Hazelnuts
        "608", // 南瓜派 / Pumpkin Pie
        "609", // 萝卜沙拉 / Radish Salad
        "610", // 水果沙拉 / Fruit Salad
        "611", // 黑莓馅饼 / Blackberry Cobbler
        "612", // 蔓越莓糖果 / Cranberry Candy
        "618", // 意式烤面包 / Bruschetta
        "SeaJelly", // 海果冻 / Sea Jelly
        "CaveJelly", // 洞穴果冻 / Cave Jelly
        "RiverJelly", // 河流果冻 / River Jelly
        
        // 饮料 / Beverages
        "395", // 咖啡 / Coffee
        "253", // 三倍浓缩咖啡 / Triple Shot Espresso
        "303", // 淡啤酒 / Pale Ale
        "306", // 蛋黄酱 / Mayonnaise
        "307", // 鸭蛋黄酱 / Duck Mayonnaise
        "308", // 虚空蛋黄酱 / Void Mayonnaise
        "342", // 腌菜 / Pickles
        "346", // 啤酒 / Beer
        "348", // 葡萄酒 / Wine
        "349", // 能量补剂 / Energy Tonic
        "350", // 果汁 / Juice
        "351", // 肌肉修复剂 / Muscle Remedy
        "445", // 鱼子酱 / Caviar
        "447", // 陈年鱼子 / Aged Roe
        "456", // 藻类汤 / Algae Soup
        "457", // 淡肉汤 / Pale Broth
        "459", // 蜂蜜酒 / Mead
        "614", // 绿茶 / Green Tea
        "Raisins", // 葡萄干 / Raisins
        "DriedFruit", // 干果 / Dried Fruit
        "DriedMushrooms", // 干蘑菇 / Dried Mushrooms
        "StardropTea", // 星之果实茶 / Stardrop Tea
        
        // 调味料 / Condiments
        "245", // 糖 / Sugar
        "246", // 小麦粉 / Wheat Flour
        "247", // 油 / Oil
        "419", // 醋 / Vinegar
        
        // 零食 / Snacks
        "403", // 野外零食 / Field Snack
        "279", // 魔法岩石糖 / Magic Rock Candy
        
        // ===== 工具和装备 / Tools and Equipment =====
        "645", // 铱制洒水器 / Iridium Sprinkler
        "787", // 电池包 / Battery Pack
        
        // ===== 传送图腾 / Warp Totems =====
        "261", // 沙漠传送图腾 / Warp Totem: Desert
        "681", // 雨图腾 / Rain Totem
        "688", // 农场传送图腾 / Warp Totem: Farm
        "689", // 山区传送图腾 / Warp Totem: Mountains
        "690", // 海滩传送图腾 / Warp Totem: Beach
        
        // ===== 爆炸物 / Explosives =====
        "286", // 樱桃炸弹 / Cherry Bomb
        "287", // 炸弹 / Bomb
        "288", // 超级炸弹 / Mega Bomb
        
        // ===== 特殊物品 / Special Items =====
        "107", // 恐龙蛋 / Dinosaur Egg
        "275", // 古物宝箱 / Artifact Trove
        "277", // 枯萎花束 / Wilted Bouquet
        "413", // 蓝色史莱姆蛋 / Blue Slime Egg
        "889", // Qi果实 / Qi Fruit
        "341", // 茶具 / Tea Set
        "373", // 金南瓜 / Golden Pumpkin
        "413", // 蓝色史莱姆蛋 / Blue Slime Egg
        "434", // 星之果实 / Stardrop
        "437", // 红色史莱姆蛋 / Red Slime Egg
        "439", // 紫色史莱姆蛋 / Purple Slime Egg
        "446", // 兔脚 / Rabbit's Foot
        "458", // 花束 / Bouquet
        "876", // 五彩果冻 / Prismatic Jelly
        "879", // 怪物麝香 / Monster Musk
        "881", // 骨头碎片 / Bone Fragment
        "890", // Qi豆 / Qi Bean
        "SmokedFish", // 熏鱼 / Smoked Fish
        "GoldenMysteryBox", // 金色神秘盒 / Golden Mystery Box
        "872", // 仙女粉尘 / Fairy Dust
        "PrizeTicket", // 奖品券 / Prize Ticket
        "TreasureTotem", // 宝藏图腾 / Treasure Totem
        
        // ===== 装饰和路径 / Decorations and Paths =====
        "401", // 稻草地板 / Straw Floor
        "402", // 甜豌豆 / Sweet Pea
        "405", // 木制路径 / Wood Path
        "407", // 砾石路径 / Gravel Path
        "409", // 水晶路径 / Crystal Path
        "411", // 鹅卵石路径 / Cobblestone Path
        "415", // 踏脚石路径 / Stepping Stone Path
        "428", // 布料 / Cloth
    };

    // 鱼类ID集合 / Fish ID set
    public static readonly HashSet<string> FishItemIds = new()
    {
        "128", // 河豚 / Pufferfish
        "129", // 凤尾鱼 / Anchovy
        "130", // 金枪鱼 / Tuna
        "131", // 沙丁鱼 / Sardine
        "132", // 鲤科鱼 / Bream
        "136", // 大口黑鲈 / Largemouth Bass
        "137", // 小口黑鲈 / Smallmouth Bass
        "138", // 虹鳟鱼 / Rainbow Trout
        "139", // 鲑鱼 / Salmon
        "140", // 梭子鱼 / Walleye
        "141", // 鲈鱼 / Perch
        "142", // 鲤鱼 / Carp
        "143", // 鲶鱼 / Catfish
        "144", // 梭鱼 / Pike
        "145", // 太阳鱼 / Sunfish
        "146", // 红鲻鱼 / Red Mullet
        "147", // 鲱鱼 / Herring
        "148", // 鳗鱼 / Eel
        "149", // 章鱼 / Octopus
        "150", // 红鲷鱼 / Red Snapper
        "151", // 鱿鱼 / Squid
        "154", // 海参 / Sea Cucumber
        "155", // 超级海参 / Super Cucumber
        "156", // 鬼鱼 / Ghostfish
        "158", // 石鱼 / Stonefish
        "159", // 绯红鱼 / Crimsonfish
        "160", // 琵琶鱼 / Angler
        "161", // 冰鳕鱼 / Ice Pip
        "162", // 熔岩鳗鱼 / Lava Eel
        "163", // 传说鱼 / Legend
        "164", // 沙鱼 / Sandfish
        "165", // 蝎子鲤鱼 / Scorpion Carp
        "267", // 比目鱼 / Flounder
        "269", // 午夜鲤鱼 / Midnight Carp
        "682", // 变异鲤鱼 / Mutant Carp
        "698", // 鲟鱼 / Sturgeon
        "699", // 虎鳟鱼 / Tiger Trout
        "700", // 牛头鱼 / Bullhead
        "701", // 罗非鱼 / Tilapia
        "702", // 白鲑 / Chub
        "704", // 剑鱼 / Dorado
        "705", // 长鳍金枪鱼 / Albacore
        "706", // 西鲱 / Shad
        "707", // 鳕鱼 / Lingcod
        "708", // 大比目鱼 / Halibut
        "715", // 龙虾 / Lobster
        "716", // 小龙虾 / Crayfish
        "717", // 螃蟹 / Crab
        "718", // 鸟蛤 / Cockle
        "719", // 贻贝 / Mussel
        "720", // 虾 / Shrimp
        "721", // 蜗牛 / Snail
        "722", // 海螺 / Periwinkle
        "723", // 牡蛎 / Oyster
        "734", // 木跳鱼 / Woodskip
        "775", // 冰川鱼 / Glacierfish
        "795", // 虚空鲑鱼 / Void Salmon
        "796", // 史莱姆鱼 / Slimejack
        "798", // 午夜鱿鱼 / Midnight Squid
        "799", // 幽灵鱼 / Spook Fish
        "800", // 水滴鱼 / Blobfish
        "836", // 黄貂鱼 / Stingray
        "837", // 狮子鱼 / Lionfish
        "838", // 蓝盘鱼 / Blue Discus
        "898", // 绯红鱼之子 / Son of Crimsonfish
        "899", // 安格勒女士 / Ms. Angler
        "900", // 传说鱼II / Legend II
        "901", // 放射性鲤鱼 / Radioactive Carp
        "902", // 小冰川鱼 / Glacierfish Jr.
        "Goby"  // 虾虎鱼 / Goby
    };

    /// <summary>
    /// 检查物品是否为鱼类 / Check if an item is a fish
    /// </summary>
    /// <param name="item">要检查的物品 / Item to check</param>
    /// <returns>如果是鱼类返回true，否则返回false / Returns true if the item is a fish, false otherwise</returns>
    public static bool IsFish(SObject item)
    {
        if (item == null)
            return false;
            
        // 使用QualifiedItemId判断，以(O)开头且是鱼类ID的都是鱼类 / Use QualifiedItemId to check, items starting with (O) and in fish list are fish
        return item.QualifiedItemId.StartsWith("(O)") && FishItemIds.Contains(item.ItemId);
    }

    /// <summary>
    /// 检查物品是否为资源类物品 / Check if an item is a resource item
    /// </summary>
    /// <param name="item">要检查的物品 / Item to check</param>
    /// <returns>如果是资源类物品返回true，否则返回false / Returns true if the item is a resource item, false otherwise</returns>
    public static bool IsResourceItem(SObject item)
    {
        if (item == null)
            return false;
            
        // 使用QualifiedItemId判断，以(O)开头的都是资源 / Use QualifiedItemId to check, items starting with (O) are resources
        // return item.QualifiedItemId.StartsWith("(O)") && ResourceItemIds.Contains(item.ItemId);ith (O) are resources
        // return item.QualifiedItemId.StartsWith("(O)") && IsStackable(item);
        return IsStackable(item);
    }

    /// <summary>
    /// 检查物品是否可堆叠 / Check if an item is stackable
    /// </summary>
    /// <param name="item">要检查的物品 / Item to check</param>
    /// <returns>如果物品可堆叠返回true，否则返回false / Returns true if the item is stackable, false otherwise</returns>
    public static bool IsStackable(Item item)
    {
        if (item == null)
            return false;
            
        // 如果最大堆叠数量大于1，则物品可堆叠 / If maximum stack size is greater than 1, item is stackable
        return item.maximumStackSize() > 1;
    }

    /// <summary>
    /// 获取物品的最大堆叠数量 / Get the maximum stack size of an item
    /// </summary>
    /// <param name="item">要检查的物品 / Item to check</param>
    /// <returns>物品的最大堆叠数量 / Maximum stack size of the item</returns>
    public static int GetMaximumStackSize(Item item)
    {
        if (item == null)
            return 0;
            
        return item.maximumStackSize();
    }
} 