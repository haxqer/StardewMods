# ModTools - 星露谷物语工具合集模组

一个功能全面的星露谷物语工具合集模组，包含资源倍数、钓鱼增强、时间控制和各种便利性改进。

[English Documentation / 英文版说明](README.md)

## 功能特性

### 🎯 资源倍数系统
- **可调节倍数**: 资源获取倍数可在3-50倍之间调节
- **支持物品**: 木材、石头、矿石、宝石、矿物等所有资源类物品
- **智能检测**: 自动检测资源类物品并应用倍数效果

### 🎣 钓鱼增强功能
#### 难度调整
- **简单钓鱼模式**: 启用更简单的钓鱼运动模式
- **难度倍数**: 调节钓鱼难度倍数 (0.1-2.0)
- **难度调整**: 添加难度调整值 (-50 到 +50)
- **钓鱼条高度**: 自定义钓鱼条高度 (60-568)

#### 便利功能
- **总是完美**: 总是获得完美捕获
- **总是宝藏**: 钓鱼时总是找到宝藏
- **瞬间捕获**: 瞬间捕获鱼类 (跳过小游戏)
- **瞬间宝藏**: 瞬间捕获宝藏
- **无限鱼饵**: 鱼饵永不消耗
- **无限鱼钩**: 鱼钩永不消耗
- **自动上钩**: 鱼类咬钩时自动上钩

#### 鱼类行为
- **鱼类移动速度**: 调节鱼类在小游戏中的移动速度 (0.1-1.0)
- **更大钓鱼条**: 使钓鱼条更大，更容易捕获

### ⏰ 时间控制系统
- **时间流速**: 调节时间流逝速度 (0.1-2.0倍)
- **慢速模式**: 时间流逝更慢，有更多时间完成任务
- **快速模式**: 时间流逝更快，快速推进游戏进度

## 安装说明

### 前置要求
- [SMAPI](https://smapi.io/) 3.0 或更高版本
- [Stardew Valley](https://www.stardewvalley.net/) 1.6 或更高版本

### 安装步骤
1. 下载最新版本的 ModTools
2. 解压到 `Stardew Valley/Mods/` 目录
3. 运行 SMAPI 启动游戏
4. 模组会自动加载

## 配置说明

### 方法一：游戏内配置菜单 (推荐)
1. 在游戏中按 `K` 键打开自定义配置菜单
2. 调整各项设置
3. 设置会自动保存

### 方法二：Generic Mod Config Menu (GMCM)
如果安装了 GMCM 模组，可以在模组设置中找到 ModTools 的配置选项。

### 方法三：配置文件
直接编辑 `config.json` 文件：
```json
{
  "Multiplier": 3,
  "EasierFishing": false,
  "FishDifficultyMultiplier": 0.5,
  "FishDifficultyAdditive": -20.0,
  "LargerFishingBar": true,
  "FishingBarHeight": 400,
  "SlowerFishMovement": false,
  "FishMovementSpeedMultiplier": 0.7,
  "AlwaysPerfect": false,
  "AlwaysFindTreasure": false,
  "InstantCatchFish": false,
  "InstantCatchTreasure": false,
  "InfiniteTackle": true,
  "InfiniteBait": true,
  "AutoHook": false,
  "TimeRateMultiplier": 1.0,
  "ReloadKey": "F5",
  "ConfigMenuKey": "K"
}
```

## 快捷键

- **F5**: 重新加载配置文件
- **K**: 打开自定义配置菜单

## 使用建议

### 新手玩家
- 建议将资源倍数设置为 3-5 倍
- 启用"简单钓鱼"和"更大钓鱼条"
- 开启"无限鱼饵"和"无限鱼钩"

### 老手玩家
- 可以根据需要调整资源倍数
- 使用时间控制功能来管理游戏节奏
- 根据需要开启各种便利功能

### 挑战模式
- 关闭所有便利功能
- 使用正常或更高的钓鱼难度
- 保持正常时间流速

## 兼容性

### 已测试兼容的模组
- Generic Mod Config Menu (GMCM)
- 大多数资源相关模组
- 大多数钓鱼相关模组

### 已知问题
- 与某些修改钓鱼机制的模组可能存在冲突
- 建议在使用前备份存档

## 故障排除

### 常见问题

**Q: 模组没有生效？**
A: 检查 SMAPI 是否正确安装，查看 SMAPI 日志确认模组已加载。

**Q: 钓鱼功能异常？**
A: 检查是否有其他钓鱼相关模组冲突，尝试禁用其他钓鱼模组。

**Q: 配置菜单打不开？**
A: 确认按的是 `K` 键，检查是否有按键冲突。

**Q: 资源倍数不生效？**
A: 确认物品在支持列表中，检查配置文件中的倍数设置。

### 日志查看
如果遇到问题，请查看 SMAPI 日志文件：
- Windows: `%APPDATA%/StardewValley/ErrorLogs/`
- macOS: `~/.config/StardewValley/ErrorLogs/`
- Linux: `~/.config/StardewValley/ErrorLogs/`

## 更新日志

### v1.0.0
- 初始版本发布
- 基础资源倍数功能
- 钓鱼增强功能
- 时间控制功能
- 多语言支持 (英文/中文)

## 贡献

欢迎提交问题报告和功能建议！

## 许可证

本项目采用 MIT 许可证。

## 致谢

感谢 ConcernedApe 创造了如此优秀的游戏，以及 SMAPI 团队提供的模组开发框架。

---

**注意**: 使用模组可能影响游戏平衡性，建议根据个人喜好调整设置。请定期备份存档。 